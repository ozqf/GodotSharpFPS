using Godot;
using System;
using System.Text;

namespace GodotSharpFps.src
{
	public class FPSController
	{
		const float PITCH_CAP_DEGREES = 89;

		const float KEYBOARD_TURN_DEGREES_PER_SECOND = 135;

		public const float MOUSE_SENSITIVITY = 0.1f;
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
		public string debugStr { get { return debugSb.ToString(); } }

		public FPSController(KinematicBody body, Spatial head)
		{
			_body = body;
			_head = head;
		}

		public void ProcessMouseMotion(InputEventMouseMotion motion, Vector2 screenRatio)
		{
			float sensitivity = MOUSE_SENSITIVITY;

			Vector2 ratio = Main.i.GetWindowToScreenRatio();
			float mouseMoveX = motion.Relative.x * sensitivity * ratio.x;
			// flip as we want moving mouse to the right to rotate left
			mouseMoveX = -mouseMoveX;
			yaw += mouseMoveX;

			float mouseMoveY = motion.Relative.y * sensitivity * ratio.y;
			pitch += mouseMoveY;
		}

		private void ApplyRotations()
		{
			// horizontal
			// clamp between 0 - 360
			while (yaw > 360) { yaw -= 360; }
			while (yaw < 0) { yaw += 360; }
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
			//Console.WriteLine($"Yaw {yaw} / Pitch {pitch}");
		}

		// Super basic test
		public void ProcessMovement(FPSInput input, float delta)
    	{
			debugSb.Clear();

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
			lastMove = _body.GlobalTransform.origin - prevPosition;
			lastDelta = delta;

			debugSb.Append($"Pitch {pitch}\nyaw {yaw}\n");
			debugSb.Append($"Prev delta {lastDelta}\n");
			debugSb.Append($"Prev move {lastMove}\n");
			debugSb.Append($"Velocity {velocity}\n");
			debugSb.Append($"Run push {runPush}\n");
			debugSb.Append($"Move spd {MOVE_SPEED} accel {MOVE_ACCELERATION}\n");
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
