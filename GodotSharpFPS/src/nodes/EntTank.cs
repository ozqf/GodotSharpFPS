using Godot;
using System;
using System.Collections.Generic;

namespace GodotSharpFps.src.nodes
{
	public class EntTank : Spatial
	{
		public enum MoveState { None, AwaitTrigger, Pending, Turning, Moving };

		[Export]
		public string firstPathName = string.Empty;

		private Transform _lookFrom, _moveFrom, _moveTo;
		private KinematicWrapper _body;
		private float _moveTime = 0;
		private MoveState _moveState = MoveState.AwaitTrigger;

		private float _turnTimeTotal = 2;
		private float _moveTimeTotal = 4;

		private Vector3 _bodyOffset = Vector3.Zero;

		private List<EntTurret> _turrets = new List<EntTurret>();

		public override void _Ready()
		{
			_body = GetNode<KinematicWrapper>("body");
			_bodyOffset = _body.GlobalTransform.origin - GlobalTransform.origin;
			ZqfGodotUtils.AddChildNodeToList(this, _turrets, "body/display/turret_a");
			ZqfGodotUtils.AddChildNodeToList(this, _turrets, "body/display/turret_b");
			Console.WriteLine($"Tank has {_turrets.Count} turrets");
		}

		public void OnTrigger()
		{
			if (_moveState == MoveState.AwaitTrigger)
			{
				Console.WriteLine($"Boss Tank Trigger!");
				_moveState = MoveState.Pending;
				for (int i = 0; i < _turrets.Count; ++i)
				{
					_turrets[i].StartTurret();
				}
			}
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
			// final destination
			_moveTo = path.GlobalTransform;
			_moveTo.origin += _bodyOffset; // janky fix because body is not on the floor
			// turn start == current
			_lookFrom = _body.GlobalTransform;
			// finished turn == move start
			_moveFrom = _lookFrom.LookingAt(_moveTo.origin, _lookFrom.basis.y);
			// overwrite move to dir with look at dir,
			_moveTo.basis = _moveFrom.basis;

			//Console.WriteLine($"Look from: {_lookFrom}");
			//Console.WriteLine($"Move from: {_moveFrom}");
			//Console.WriteLine($"Move To: {_moveTo}");

			_moveTime = 0;
			_moveState = MoveState.Turning;
		}

		public override void _PhysicsProcess(float delta)
		{
			Transform lerpT;
			switch (_moveState)
			{
				case MoveState.Pending:
					StartMove();
					break;
				case MoveState.Turning:
					lerpT = _lookFrom.InterpolateWith(_moveFrom, _moveTime);
					_body.GlobalTransform = lerpT;

					if (_turnTimeTotal <= 0) { _turnTimeTotal = 2; }
					_moveTime += (delta / _turnTimeTotal);
					if (_moveTime >= 1)
					{
						_moveState = MoveState.Moving;
						_moveTime = 0;
					}
					break;
				case MoveState.Moving:
					// lerp
					lerpT = _moveFrom.InterpolateWith(_moveTo, _moveTime);
					// directly lerp rotation and position
					_body.GlobalTransform = lerpT;

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
					if (_moveTime >= 1)
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
