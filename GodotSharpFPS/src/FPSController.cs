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

		private Vector3 _velocity = Vector3.Zero;
		private Vector3 _lastMove = Vector3.Zero;
		private float _lastDelta = 0;

		private Vector3 externalPushAccumulator = Vector3.Zero;

		private float _yaw = 0;
		private float _pitch = 0;

		private StringBuilder _debugSb = new StringBuilder(1024);
		public string debugStr { get { return _debugSb.ToString(); } }

		public FPSController(KinematicBody body, Spatial head)
		{
			_body = body;
			_head = head;
		}

		public void ProcessMouseMotion(InputEventMouseMotion motion, Vector2 screenRatio)
		{
			float sensitivity = MOUSE_SENSITIVITY;

			Vector2 ratio = ZqfGodotUtils.GetWindowToScreenRatio();
			float mouseMoveX = motion.Relative.x * sensitivity * ratio.x;
			// flip as we want moving mouse to the right to rotate left
			mouseMoveX = -mouseMoveX;
			_yaw += mouseMoveX;

			float mouseMoveY = motion.Relative.y * sensitivity * ratio.y;
			_pitch += mouseMoveY;
		}

		private void ApplyRotations()
		{
			// horizontal
			// clamp between 0 - 360
			while (_yaw > 360) { _yaw -= 360; }
			while (_yaw < 0) { _yaw += 360; }
			Vector3 rot = _body.RotationDegrees;
			rot.y = _yaw;
			_body.RotationDegrees = rot;

			Vector3 headRot = _head.RotationDegrees;
			// vertical
			// clamp
			if (_pitch > PITCH_CAP_DEGREES) { _pitch = PITCH_CAP_DEGREES; }
			else if (_pitch < -PITCH_CAP_DEGREES) { _pitch = -PITCH_CAP_DEGREES; }
			headRot.x = _pitch;
			_head.RotationDegrees = headRot;
			//Console.WriteLine($"Yaw {yaw} / Pitch {pitch}");
		}

		// Super basic test
		public void ProcessMovement(FPSInput input, float delta)
    	{
			_debugSb.Clear();

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

			_yaw += mouseMoveX * delta;
			_pitch += mouseMoveY * delta;

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
			Vector3 lastVel = CalcLastFlatVelocity(_lastMove, _lastDelta);
			Vector3 horizontal = CalcVelocityQuakeStyle(
				lastVel, runPush, MOVE_SPEED, delta, true);
			_velocity.x = horizontal.x;
			_velocity.z = horizontal.z;

			// Apply external push
			Vector3 prevPosition = t.origin;

			// Move!
			_body.MoveAndSlide(_velocity);

			// record move info for next frame
			_lastMove = _body.GlobalTransform.origin - prevPosition;
			_lastDelta = delta;

			_debugSb.Append($"FPS: {Engine.GetFramesPerSecond()}");
			_debugSb.Append($"Pitch {_pitch}\nyaw {_yaw}\n");
			_debugSb.Append($"Prev delta {_lastDelta}\n");
			_debugSb.Append($"Prev move {_lastMove}\n");
			_debugSb.Append($"Velocity {_velocity}\n");
			_debugSb.Append($"Run push {runPush}\n");
			_debugSb.Append($"Move spd {MOVE_SPEED} accel {MOVE_ACCELERATION}\n");
		}

		private Vector3 CalcLastFlatVelocity(Vector3 previousMove, float previousDelta)
		{
			// Calculate current velocity per second,
			// (after avoiding divide by zero...)
			// reconstruct it by taking the last position change
			// scaled back by last delta. Appears to be accurate to
			// 4 decimal places or so
			// (...is this a form of verlet intergration?)
			Vector3 previousVel = Vector3.Zero;
			if (_lastDelta != 0)
			{
				previousVel = _lastMove * (1 / _lastDelta);
				// clear vertical, it is handled differently
				previousVel.y = 0;
			}
			return previousVel;
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
		public static Vector3 CalcVelocityQuakeStyle(
			Vector3 previousFlatVel,
			Vector3 accelDir,
			float maxMoveSpeed,
			float delta,
			bool onGround,
			float friction = GROUND_FRICTION,
			float accelForce = MOVE_ACCELERATION,
			float airAccelForce = MOVE_AIR_MULTIPLIER)
		{
			// clear vertical, it is handled differently
			previousFlatVel.y = 0;
			
			float previousSpeed = previousFlatVel.Length();
			// stop dead if slow enough
			if (previousSpeed < 0.001)
			{
				previousFlatVel = Vector3.Zero;
				previousSpeed = 0;
			}

			// Friction
			bool inputOn = (accelDir.Length() > 0);
			// should we apply friction at all?
			if (onGround && previousSpeed > 0 && (inputOn == false || previousSpeed > maxMoveSpeed))
			{
				float speedDrop = previousSpeed * friction * delta;
				float frictionScalar = Math.Max(previousSpeed - speedDrop, 0) / previousSpeed;
				// friction only occurs on horizontal!
				previousFlatVel.x *= frictionScalar;
				previousFlatVel.z *= frictionScalar;
			}

			float acceleration = accelForce;
			if (!onGround) { acceleration *= airAccelForce; }

			// Check applying this push would not exceed the maximum run speed
			// If necessary truncale the velocity so the vector projection does not
			// exceed maximum run speed
			float projectionVelDot = accelDir.Dot(previousFlatVel);
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
			result.x = previousFlatVel.x + (accelDir.x * accelMagnitude);
			result.z = previousFlatVel.z + (accelDir.z * accelMagnitude);
			return result;
		}


	}
}
