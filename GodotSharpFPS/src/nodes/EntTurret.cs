using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.nodes
{
	public class EntTurret : Spatial
	{
		public enum State { Idle, Attacking };
		//[Signal]
		//public delegate void Trigger(string message);

		private State _state = State.Idle;

		private ProjectileDef _prjDef;
		private PatternDef _patternDef;

		private float _tick = 1;
		private float _tickMax = 1.5f;
		private List<Transform> _transforms = null;

		private Transform _idleLocalTransform;

		public override void _Ready()
		{
			_idleLocalTransform = Transform;

			_prjDef = new ProjectileDef();
			_prjDef.damage = 10;
			_prjDef.prefabPath = GameFactory.Path_PointProjectile;

			_prjDef.moveMode = ProjectileDef.MoveMode.Basic;
			_prjDef.launchSpeed = 15;

			//_prjDef.moveMode = ProjectileDef.MoveMode.Accel;
			//_prjDef.launchSpeed = 100;
			//_prjDef.maxSpeed = 100;
			//_prjDef.minSpeed = 10;
			//_prjDef.accelPerSecond = -300;

			_patternDef = new PatternDef();
			_patternDef.count = 8;
			_patternDef.patternType = PatternType.HorizontalLine;
			_patternDef.scale = new Vector3(4, 8, 0);

			_transforms = new List<Transform>(_patternDef.count);

			IActor actorParent = Game.DescendTreeToActor(this);
			if (actorParent != null)
			{
				Console.WriteLine($"Turret found parent actor {actorParent.actorId}");
			}
		}

		public void Trigger(string message)
        {

        }

		public void StartTurret()
        {
			_state = State.Attacking;
        }

		public void StopTurret()
        {
			_state = State.Idle;
			Transform = _idleLocalTransform;
		}

		private void Shoot()
		{
			SpawnPatterns.FillPattern(GlobalTransform, _patternDef, _transforms);
			for (int i = 0; i < _transforms.Count; ++i)
			{
				PointProjectile prj = Main.i.factory.SpawnPointProjectile();
				prj.Launch(_transforms[i].origin, -_transforms[i].basis.z, _prjDef, null, Team.Mobs);
			}
		}

		private void TickAttacking(float delta)
        {
			IActor actor = Main.i.game.CheckTarget(0, Team.Mobs);
			if (actor == null) { return; }

			LookAt(actor.GetTransformForTarget().origin, Vector3.Up);

			if (_tick <= 0)
			{
				_tick = _tickMax;
				Shoot();
			}
			else
			{
				_tick -= delta;
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			switch (_state)
            {
				case State.Attacking:
					TickAttacking(delta);
					break;
            }
		}
	}
}
