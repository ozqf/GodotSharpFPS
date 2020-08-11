using Godot;
using System;
using System.Text;

namespace GodotSharpFps.src.nodes
{
	public class EntMob : Spatial, IActor, IActorProvider
	{
		private MobDef _mobDef;
		private KinematicWrapper _body;
		private ProjectileDef _prjDef;
		private bool _dead = false;
		private int _health = 200;
		private int _targetActorId = Game.NullActorId;
		private float _moveTick = 0;
		private float _attackTick = 0;
		public IActor GetActor() => this;

		private int _entId = 0;

		// Current movement from last frame
		private Vector3 _velocity = new Vector3();
		// external forces that have pushed this mob in the last frame
		private Vector3 _pushAccumulator = new Vector3();
		// intended AI move velocity
		private Vector3 _selfMove = new Vector3();
		private Vector3 _lastSelfDir = new Vector3();
		private StringBuilder _debugSb = new StringBuilder();

		public override void _Ready()
		{
			if (_mobDef == null)
			{
				Console.WriteLine($"EntMob had no mob type set - getting default");
				_mobDef = Main.i.factory.GetMobType(GameFactory.MobType_Humanoid);
			}
			_health = _mobDef.defaultHealth;
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
			_prjDef.launchSpeed = 10;
			_prjDef.prefabPath = GameFactory.Path_PointProjectile;
			_prjDef.moveMode = ProjectileDef.MoveMode.Accel;
			_prjDef.maxSpeed = 150;
			_prjDef.accelPerSecond = 50;

			_debugSb.Clear();
			_debugSb.Append($"EntMob Id {_entId}");
		}

		public void SetMobType(MobDef mobType) { _mobDef = mobType; }

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
		public void ActorTeleport(Transform t) { _body.GlobalTransform = t; }

		public void ChildActorRemoved(int id) { }

		public string GetActorDebugText()
		{
			return _debugSb.ToString();
		}

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
				_pushAccumulator.x += -touchData.hitNormal.x * (15 * _mobDef.pushMultiplier);
				_pushAccumulator.z += -touchData.hitNormal.z * (15 * _mobDef.pushMultiplier);
				Console.WriteLine($"Push: {_pushAccumulator}");
			}
			return result;
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
				if (dist > _mobDef.evadeRange)
				{
					// randomly jink to the side
					if (rand < 0.25)
					{
						yawDeg += 45;
					}
					else if (rand < 0.5)
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
				float radians = Mathf.Deg2Rad(yawDeg);
				_lastSelfDir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
			}
			else
			{
				_moveTick -= delta;
			}

			// calculate self move
			_selfMove = FPSController.CalcVelocityQuakeStyle(
				_velocity, _lastSelfDir, _mobDef.walkSpeed, delta, true, _mobDef.friction, _mobDef.accelForce);

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
		}
	}
}
