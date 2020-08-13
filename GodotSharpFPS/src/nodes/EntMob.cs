using Godot;
using System;
using System.Text;

namespace GodotSharpFps.src.nodes
{
	public class EntMob : Spatial, IActor, IActorProvider
	{
		private bool _dead = false;
		private int _health = 200;

		// Public for access by mob think
		public MobDef mobDef;
		public KinematicWrapper body;
		public ProjectileDef prjDef;
		public int targetActorId = Game.NullActorId;
		public float moveTick = 0;
		public float thinkTick = 0;
		public int thinkState = 0;
		public int thinkIndex = MobThink.DefaultThink;

		public IActor GetActor() => this;

		// Current movement from last frame
		public Vector3 velocity = new Vector3();
		// external forces that have pushed this mob in the last frame
		public Vector3 pushAccumulator = new Vector3();
		// intended AI move velocity
		public Vector3 selfMove = new Vector3();
		public Vector3 lastSelfDir = new Vector3();

		private int _entId = 0;
		private StringBuilder _debugSb = new StringBuilder();

		public override void _Ready()
		{
			if (mobDef == null)
			{
				Console.WriteLine($"EntMob had no mob type set - getting default");
				mobDef = Main.i.factory.GetMobType(GameFactory.MobType_Humanoid);
			}
			_health = mobDef.defaultHealth;
			// find Godot scene nodes
			body = GetNode<KinematicWrapper>("actor_base");
			body.actor = this;

			if (_entId == 0)
			{
				// no id previous set, request one
				Console.WriteLine($"EntMob - no id set - registering self");
				_entId = Main.i.game.ReserveActorId(1);
				Main.i.game.RegisterActor(this);
			}
			prjDef = new ProjectileDef();
			prjDef.damage = 15;
			prjDef.launchSpeed = 10;
			prjDef.prefabPath = GameFactory.Path_PointProjectile;
			prjDef.moveMode = ProjectileDef.MoveMode.Accel;
			prjDef.maxSpeed = 150;
			prjDef.accelPerSecond = 50;

			_debugSb.Clear();
			_debugSb.Append($"EntMob Id {_entId}");
		}

		public void SetMobType(MobDef mobType) { mobDef = mobType; }

		public void SetActorId(int newId)
		{
			if (_entId != 0) { throw new Exception($"EntMob already has an actor Id"); }
			_entId = newId;
		}

		public int ParentActorId { get; set; }
		public int actorId { get { return _entId; } }
		public Team GetTeam() { return Team.Mobs; }
		public Transform GetTransformForTarget() { return body.GetTransformForTarget(); }
		public void RemoveActor() { this.QueueFree(); }
		public void ActorTeleport(Transform t) { body.GlobalTransform = t; }

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
					Main.i.mobThink.ApplyStun(this);
				}
				result.responseType = TouchResponseType.Damaged;
				pushAccumulator.x += -touchData.hitNormal.x * (15 * mobDef.pushMultiplier);
				pushAccumulator.z += -touchData.hitNormal.z * (15 * mobDef.pushMultiplier);
				Console.WriteLine($"Push: {pushAccumulator}");
			}
			return result;
		}

		public override void _PhysicsProcess(float delta)
		{
			Main.i.mobThink.UpdateMob(this, delta);
			/*
			IActor actor = Main.i.game.CheckTarget(targetActorId, GetTeam());
			if (actor == null)
			{
				targetActorId = Game.NullActorId;
				return;
			}
			targetActorId = actor.actorId;
			Transform self = GetTransformForTarget();
			Vector3 tar = actor.GetTransformForTarget().origin;
			float yawDeg = ZqfGodotUtils.FlatYawDegreesBetween(
				self.origin, tar);
			body.RotationDegrees = new Vector3(0, yawDeg + 180, 0);
			self = GetTransformForTarget();

			if (attackTick <= 0)
			{
				attackTick = 1;
				PointProjectile prj = Main.i.factory.SpawnPointProjectile();
				prj.Launch(self.origin, -self.basis.z, prjDef, body, Team.Mobs);
			}
			else
			{
				attackTick -= delta;
			}
			
			if (moveTick <= 0)
			{
				float rand = ZqfGodotUtils.Randomf();
				float dist = ZqfGodotUtils.Distance(self.origin, tar);
				moveTick = 1.5f;
				// move toward target
				if (dist > mobDef.evadeRange)
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
					moveTick = 1f;
				}
				else
				{
					// evade either left or right
					// don't add a full 90 degrees as will evade straight out of evade dist
					yawDeg += (rand > 0.5) ? 70 : -70;
					moveTick = 1.5f;
				}
				float radians = Mathf.Deg2Rad(yawDeg);
				lastSelfDir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
			}
			else
			{
				moveTick -= delta;
			}

			// calculate self move
			selfMove = FPSController.CalcVelocityQuakeStyle(
				velocity, lastSelfDir, mobDef.walkSpeed, delta, true, mobDef.friction, mobDef.accelForce);

			velocity = selfMove;
			// apply push forces
			velocity += pushAccumulator;
			pushAccumulator = Vector3.Zero;
			if (velocity.LengthSquared() > Game.MaxActorVelocity * Game.MaxActorVelocity)
			{
				velocity = velocity.Normalized();
				velocity *= Game.MaxActorVelocity;
			}
			Vector3 result = body.MoveAndSlide(velocity);
			*/
		}
	}
}
