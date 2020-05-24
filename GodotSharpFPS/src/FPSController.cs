using Godot;
using System;
using System.Text;

namespace GodotSharpFps.src
{
	public class FPSController
	{
		const float PITCH_CAP_DEGREES = 89;

		const float KEYBOARD_TURN_DEGREES_PER_SECOND = 135;

		const float MOUSE_SENSITIVITY = 0.15f;
		const float MOVE_SPEED = 15;
		const float MOVE_ACCELERATION = 150;
		const float MOVE_PUSH_STRENGTH = 0.2f;
		const float GROUND_FRICTION = 10;
		const float MOVE_AIR_MULTIPLIER = 0.3f;
		const float GRAVITY_METRES_PER_SECOND = 25;
		const float JUMP_METRES_PER_SECOND = 10;

		private KinematicBody _body = null;
		private Spatial _head = null;

		private Vector3 velocity = Vector3.Zero;
		private Vector3 lastMove = Vector3.Zero;
		private float lastDelta = 0;

		private Vector3 externalPushAccumulator = Vector3.Zero;

		private float yaw = 0;
		private float pitch = 0;

		private StringBuilder debugSb = new StringBuilder(1024);

		public FPSController(KinematicBody body, Spatial head)
		{
			_body = body;
			_head = head;
		}

		private void ApplyRotations()
		{
			// horizontal
			Vector3 rot = _body.RotationDegrees;
			rot.y = yaw;
			_body.RotationDegrees = rot;

			Vector3 headRot = _head.RotationDegrees;
			// vertical
			// clamp
			if (pitch > PITCH_CAP_DEGREES) { pitch = PITCH_CAP_DEGREES; }
			else if (pitch < -PITCH_CAP_DEGREES) { pitch = -PITCH_CAP_DEGREES; }
			headRot.x = pitch;
			_head.RotationDegrees = headRot;
			Console.WriteLine($"Yaw {yaw} / Pitch {pitch}");
		}

		// Super basic test
		public void ProcessMovement(FPSInput input, float delta)
    	{
			ApplyRotations();
			//Console.WriteLine($"Buttons {input.buttons} delta {delta}");
			Vector3 inputDir = new Vector3();
			
			if (input.isBitOn(FPSInput.BitMoveForward)) { inputDir.z -= 1; }
			if (input.isBitOn(FPSInput.BitMoveBackward)) { inputDir.z += 1; }
			if (input.isBitOn(FPSInput.BitMoveLeft)) { inputDir.x -= 1; }
			if (input.isBitOn(FPSInput.BitMoveRight)) { inputDir.x += 1; }

			float mouseMoveX = 0; // horizontal turn
			float mouseMoveY = 0; // verticla turn
			if (input.isBitOn(FPSInput.BitLookLeft))
			{ mouseMoveX += KEYBOARD_TURN_DEGREES_PER_SECOND; }
			if (input.isBitOn(FPSInput.BitLookRight))
			{ mouseMoveX -= KEYBOARD_TURN_DEGREES_PER_SECOND; }

			if (input.isBitOn(FPSInput.BitLookUp))
			{ mouseMoveY += KEYBOARD_TURN_DEGREES_PER_SECOND; }
			if (input.isBitOn(FPSInput.BitLookDown))
			{ mouseMoveY -= KEYBOARD_TURN_DEGREES_PER_SECOND; }

			yaw += mouseMoveX * delta;
			pitch += mouseMoveY * delta;

			// convert desired move to world axes
			Transform t = _body.GlobalTransform;
			Vector3 forward = t.basis.z;
			Vector3 left = t.basis.x;

			Vector3 runPush = Vector3.Zero;
			runPush.x += forward.x * inputDir.z;
			runPush.z += forward.z * inputDir.z;
			runPush.x += left.x * inputDir.x;
			runPush.z += left.z * inputDir.x;
			runPush = runPush.Normalized();

			// calculate horizontal move independently.
			Vector3 horizontal = CalcVelocityQuakeStyle(
				velocity, runPush, MOVE_SPEED, delta, true);
			velocity.x = horizontal.x;
			velocity.z = horizontal.z;

			// Apply external push
			Vector3 prevPosition = t.origin;

			// Move!
			Vector3 moveResult = _body.MoveAndSlide(velocity);

			// record move info for next frame
			lastMove = t.origin - prevPosition;
			lastDelta = delta;

			// quick n dirty for testing
			//move *= 5;
			//_body.MoveAndSlide(move, Vector3.Up);
		}

		/// <summary>
		/// Calculate the move that should be performed this frame.
		/// </summary>
		/// <param name="currentVel"></param>
		/// <param name="accelDir"></param>
		/// <param name="maxMoveSpeed"></param>
		/// <param name="delta"></param>
		/// <param name="onGround"></param>
		/// <returns></returns>
		private Vector3 CalcVelocityQuakeStyle(
			Vector3 currentVel,
			Vector3 accelDir,
			float maxMoveSpeed,
			float delta,
			bool onGround)
		{
			// Calculate current velocity per second,
			// (after avoiding divide by zero...)
			// reconstruct it by taking the last position change
			// scaled back by last delta. Appears to be accurate to
			// 4 decimal places or so
			// (...is this a form of verlet intergration?)
			Vector3 previousVel = Vector3.Zero;
			if (lastDelta != 0)
			{
				previousVel = lastMove * (1 / lastDelta);
				// clear vertical, it is handled differently
				previousVel.y = 0;
			}

			float previousSpeed = previousVel.Length();
			// stop dead if slow enough
			if (previousSpeed < 0.001)
			{
				previousVel = Vector3.Zero;
				previousSpeed = 0;
			}

			// Friction
			bool inputOn = (accelDir.Length() > 0);
			// should we apply friction at all?
			if (onGround && previousSpeed > 0 && (inputOn == false || previousSpeed > maxMoveSpeed))
			{
				float speedDrop = previousSpeed * GROUND_FRICTION * delta;
				float frictionScalar = Math.Max(previousSpeed - speedDrop, 0) / previousSpeed;
				// friction only occurs on horizontal!
				previousVel.x *= frictionScalar;
				previousVel.z *= frictionScalar;
			}

			float acceleration = MOVE_ACCELERATION;
			if (!onGround) { acceleration *= MOVE_AIR_MULTIPLIER; }

			// Check applying this push would not exceed the maximum run speed
			// If necessary truncale the velocity so the vector projection does not
			// exceed maximum run speed
			float projectionVelDot = accelDir.Dot(previousVel);
			float accelMagnitude = acceleration * delta;

			if (projectionVelDot + accelMagnitude > maxMoveSpeed)
			{
				// will exceed allowed move speed - truncate
				accelMagnitude = maxMoveSpeed - projectionVelDot;
			}

			// accel magnitude can be pushed into negative, avoid actively
			// reducing speed
			if (accelMagnitude < 0) { accelMagnitude = 0; }

			// apply acceleration
			Vector3 result = Vector3.Zero;
			result.x = previousVel.x + (accelDir.x * accelMagnitude);
			result.z = previousVel.z + (accelDir.z * accelMagnitude);
			return result;
		}


	}
}
