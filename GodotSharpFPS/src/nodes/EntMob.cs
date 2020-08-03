using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
	public class EntMob : Spatial, IActor, IActorProvider
	{
		private KinematicWrapper _body;
		private ProjectileDef _prjDef;
		private bool _dead = false;
		private int _health = 100;
		private int _targetActorId = Game.NullActorId;
		private float _attackTick = 0;
		public IActor GetActor() => this;

		private int _entId = 0;

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
			_prjDef.launchSpeed = 10;
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
			if (_dead)
			{
				return TouchResponseData.empty;
			}
			_health -= touchData.damage;

			TouchResponseData result;
			result.damageTaken = touchData.damage;
			if (_health <= 0)
			{
				result.responseType = TouchResponseType.Killed;
				_dead = true;
				Main.i.game.DeregisterActor(this);
				QueueFree();
			}
			else
			{
				result.responseType = TouchResponseType.Damaged;
			}
			return result;
		}

		//private float yaw = 0;
		public override void _Process(float delta)
		{
			base._Process(delta);
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
			RotationDegrees = new Vector3(0, yawDeg, 0);

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

			//if (yaw != yawDeg)
			//{
			//    yaw = yawDeg;
			//    Console.WriteLine($"Yaw {yaw} between {self} and {tar}");
			//}
		}
	}
}
