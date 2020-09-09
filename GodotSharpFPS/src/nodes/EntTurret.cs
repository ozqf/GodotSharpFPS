using Godot;
using System;
using System.Collections.Generic;

namespace GodotSharpFps.src.nodes
{
	public class EntTurret : Spatial, ITouchable
	{
		public delegate void TurretDied(EntTurret turret);

		private TurretDied _diedCallback;

		public enum State { Idle, Attacking, Dead };
		//[Signal]
		//public delegate void Trigger(string message);

		private State _state = State.Idle;

		private ProjectileDef _prjDef;
		private PatternDef _patternDef;

		private float _tick = 1;
		private float _tickMax = 1.5f;
		private List<Transform> _transforms = null;

		private Transform _idleLocalTransform;

		private RigidBody _body;
		private Health _hpArea;
		private MeshInstance _model;

		public override void _Ready()
		{
			_model = ZqfGodotUtils.GetNodeSafe<MeshInstance>(this, "mlrs_turret/Cube");
			_body = ZqfGodotUtils.GetNodeSafe<RigidBody>(this, "mlrs_turret");
			_hpArea = ZqfGodotUtils.GetNodeSafe<Health>(this, "mlrs_turret/area");
			if (_hpArea != null)
            {
				_hpArea.SetCallbacks(OnHealthChange, OnDeath);
				_hpArea.OverrideStats(Team.Mobs, 400, 400);
			}

			_idleLocalTransform = Transform;

			_prjDef = new ProjectileDef();
			_prjDef.damage = 10;
			_prjDef.prefabPath = GameFactory.Path_PointProjectile;

			_prjDef.speedMode = ProjectileDef.SpeedMode.Basic;
			_prjDef.launchSpeed = 15;

			//_prjDef.moveMode = ProjectileDef.MoveMode.Accel;
			//_prjDef.launchSpeed = 100;
			//_prjDef.maxSpeed = 100;
			//_prjDef.minSpeed = 10;
			//_prjDef.accelPerSecond = -300;

			_patternDef = new PatternDef();
			_patternDef.count = 8;
			_patternDef.patternType = PatternType.HorizontalLine;
			_patternDef.scale = new Vector3(4, 4, 0);

			_transforms = new List<Transform>(_patternDef.count);

			IActor actorParent = Game.DescendTreeToActor(this);
			if (actorParent != null)
			{
				Console.WriteLine($"Turret found parent actor {actorParent.actorId}");
			}
		}

		public void Resurrect()
        {
			if (_hpArea != null)
            {
				_hpArea.Resurrect();
            }
			if (_model != null)
            {
				_model.Show();
            }
        }

		public void SetDeathCallback(TurretDied msg)
        {
			_diedCallback = msg;
		}

		// body callbacks
		public void OnHealthChange(int current, int change, TouchData data)
		{
			Console.WriteLine($"Hurt turret");
		}

		public void OnDeath()
        {
			if (_state == State.Dead) { return; }
			_state = State.Dead;
			// - disable display here
			// - create debris
			if (_model != null)
			{
				_model.Hide();
			}

			Console.WriteLine($"Spawn MLRS debris");
			Debris debris = Main.i.factory.Spawn<Debris>(GameFactory.Path_MLRS_Debris);
			debris.GlobalTransform = GlobalTransform;
			debris.LaunchUp();
			ZqfGodotUtils.SwapSpatialParent(debris, Main.i.game.root);

			if (_diedCallback != null)
            {
				_diedCallback.Invoke(this);
            }
		}

		public void Trigger(string message)
        {

        }

		public void StartTurret()
        {
			if (_state == State.Dead)
            {
				Resurrect();
			}
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

		public TouchResponseData Touch(TouchData touchData)
        {
			Console.WriteLine($"Touch turret");
			return TouchResponseData.empty;
        }
	}
}
