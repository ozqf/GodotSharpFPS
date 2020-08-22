using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
	public class EntTank : Spatial
	{
		public enum MoveState { None, Pending, Turning, Moving };

		[Export]
		public string firstPathName = string.Empty;

		private Transform from, to;
		private KinematicWrapper _body;
		private float _moveTime = 0;
		private MoveState _moveState = MoveState.Pending;

		private float _moveTimeTotal = 4;

		private Vector3 _bodyOffset = Vector3.Zero;

		public override void _Ready()
		{
			_body = GetNode<KinematicWrapper>("body");
			_bodyOffset = _body.GlobalTransform.origin - GlobalTransform.origin;
		}

		private void StartMove()
		{
			//Console.WriteLine($"Ent tank start move");
			InfoPath path = Main.i.game.GetPathNode(firstPathName);
			if (path == null)
			{
				_moveState = MoveState.None;
				return;
			}
			from = _body.GlobalTransform;
			to = path.GlobalTransform;
			to.origin += _bodyOffset;
			_moveTime = 0;
			_moveState = MoveState.Moving;
		}

		public override void _PhysicsProcess(float delta)
		{
			switch (_moveState)
			{
				case MoveState.Pending:
					StartMove();
					break;
				case MoveState.Moving:
					// lerp
					Transform lerpT = from.InterpolateWith(to, _moveTime);
					// directly lerp rotation and position
					GlobalTransform = lerpT;

					// just apply position and look at the target
					//Transform realT = GlobalTransform;
					//realT.origin = lerpT.origin;
					//_body.GlobalTransform = realT;
					//LookAt(to.origin, to.basis.y);

					// update move time
					if (_moveTimeTotal <= 0) { _moveTimeTotal = 10; }
					_moveTime += (delta / _moveTimeTotal);
					//Console.WriteLine($"MoveTime {_moveTime}");
					//_moveTime += delta;
					if (_moveTime > 1)
					{
						InfoPath path = Main.i.game.GetPathNode(firstPathName);
						if (path == null)
						{
							_moveState = MoveState.None;
							return;
						}
						_moveState = MoveState.Pending;
						firstPathName = path.nextPathName;
					}
					break;
			}
		}
	}
}
