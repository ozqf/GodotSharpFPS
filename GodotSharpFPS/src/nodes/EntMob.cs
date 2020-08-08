using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
	public class EntMob : Spatial, IActor, IActorProvider
	{
		private KinematicWrapper _body;
		private ProjectileDef _prjDef;
		private bool _dead = false;
		private int _health = 200;
		private int _targetActorId = Game.NullActorId;
		private float _moveTick = 0;
		private float _attackTick = 0;
		private float _evadeRange = 6f;
		public IActor GetActor() => this;

		private int _entId = 0;

		// Current movement from last frame
		private Vector3 _velocity = new Vector3();
		// external forces that have pushed this mob in the last frame
		private Vector3 _pushAccumulator = new Vector3();
		// intended AI move velocity
		private Vector3 _selfMove = new Vector3();
		private Vector3 _lastSelfDir = new Vector3();
		private float _walkSpeed = 10;

		public override void _Ready()
		{
			// find Godot scene nodes
			_body = GetNode<KinematicWrapper>("actor_base");
			_body.actor = this;

			if (_entId == 0)
			{
				// no id previous set, request one
				Console.WriteLine($"EntMob - no id set - registering self");
				_entId = Main.i.game.ReserveActorId(1);
				Main.i.game.RegisterActor(this);
			}
			_prjDef = new ProjectileDef();
			_prjDef.damage = 15;
			_prjDef.launchSpeed = 15;
			_prjDef.prefabPath = GameFactory.Path_PointProjectile;
		}

		public void SetActorId(int newId)
		{
			if (_entId != 0) { throw new Exception($"EntMob already has an actor Id"); }
			_entId = newId;
		}

		public int ParentActorId { get; set; }
		public int actorId { get { return _entId; } }
		public Team GetTeam() { return Team.Mobs; }
		public Transform GetTransformForTarget() { return _body.GetTransformForTarget(); }
		public void RemoveActor() { this.QueueFree(); }
		public void ActorTeleport(Transform t)
		{
			_body.GlobalTransform = t;
		}

		public void ChildActorRemoved(int id) { }

		public TouchResponseData ActorTouch(TouchData touchData)
		{
			Console.WriteLine($"Mob hit for {touchData.damage}");
			if (_dead) { return TouchResponseData.empty; }
			_health -= touchData.damage;

			TouchResponseData result;
			result.damageTaken = touchData.damage;
			GFXQuick gfx = Main.i.factory.SpawnGFX(GameFactory.Path_GFXBloodImpact);
			gfx.Spawn(touchData.hitPos, touchData.hitNormal);
			if (_health <= 0)
			{
				result.responseType = TouchResponseType.Killed;
				_dead = true;
				Main.i.game.DeregisterActor(this);
				QueueFree();
			}
			else
			{
				if (touchData.damageType == DamageType.Launch)
				{
					Console.WriteLine($"Mob - Launched!");
				}
				result.responseType = TouchResponseType.Damaged;
				_pushAccumulator.x += -touchData.hitNormal.x * 15;
				_pushAccumulator.z += -touchData.hitNormal.z * 15;
				Console.WriteLine($"Push: {_pushAccumulator}");
			}
			return result;
		}

		//private float yaw = 0;
		public override void _Process(float delta)
		{
			
		}

		public override void _PhysicsProcess(float delta)
		{
			IActor actor = Main.i.game.CheckTarget(_targetActorId, GetTeam());
			if (actor == null)
			{
				_targetActorId = Game.NullActorId;
				return;
			}
			_targetActorId = actor.actorId;
			Transform self = GetTransformForTarget();
			Vector3 tar = actor.GetTransformForTarget().origin;
			float yawDeg = ZqfGodotUtils.FlatYawDegreesBetween(
				self.origin, tar);
			_body.RotationDegrees = new Vector3(0, yawDeg + 180, 0);
			self = GetTransformForTarget();

			if (_attackTick <= 0)
			{
				_attackTick = 1;
				PointProjectile prj = Main.i.factory.SpawnPointProjectile();
				prj.Launch(self.origin, -self.basis.z, _prjDef, _body, Team.Mobs);
			}
			else
			{
				_attackTick -= delta;
			}
			
			if (_moveTick <= 0)
			{
				float rand = ZqfGodotUtils.Randomf();
				float dist = ZqfGodotUtils.Distance(self.origin, tar);
				_moveTick = 1.5f;
				// move toward target
				if (dist > _evadeRange)
				{
					// randomly jink to the side
					if (rand < 0.333)
					{
						yawDeg += 45;
					}
					else if (rand < 0.666)
					{
						yawDeg -= 45;
					}
					// check movement again in:
					_moveTick = 1f;
				}
				else
				{
					// evade either left or right
					// don't add a full 90 degrees as will evade straight out of evade dist
					yawDeg += (rand > 0.5) ? 70 : -70;
					_moveTick = 1.5f;
				}
				
				Console.WriteLine($"Move degrees: {yawDeg}");
				//_selfMove.x = Mathf.Sin(radians) * _walkSpeed;
				//_selfMove.y = 0;
				//_selfMove.z = Mathf.Cos(radians) * _walkSpeed;

				float radians = Mathf.Deg2Rad(yawDeg);
				_lastSelfDir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
				//Vector3 dir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
				//_selfMove = FPSController.CalcVelocityQuakeStyle(
				//	_velocity, dir, _walkSpeed, delta, true, 3, 50);
			}
			else
			{
				_moveTick -= delta;
			}

			// calculate self move
			//Vector3 dir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
			_selfMove = FPSController.CalcVelocityQuakeStyle(
				_velocity, _lastSelfDir, _walkSpeed, delta, true, 4, 25);

			_velocity = _selfMove;
			// apply push forces
			_velocity += _pushAccumulator;
			_pushAccumulator = Vector3.Zero;
			if (_velocity.LengthSquared() > Game.MaxActorVelocity * Game.MaxActorVelocity)
			{
				_velocity = _velocity.Normalized();
				_velocity *= Game.MaxActorVelocity;
			}
			Vector3 result = _body.MoveAndSlide(_velocity);

			//if (yaw != yawDeg)
			//{
			//    yaw = yawDeg;
			//    Console.WriteLine($"Yaw {yaw} between {self} and {tar}");
			//}
		}
	}
}
