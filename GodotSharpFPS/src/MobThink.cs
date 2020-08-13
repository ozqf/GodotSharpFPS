using Godot;
using GodotSharpFps.src.nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src
{
    public interface IMobThinker
    {
        void Think(EntMob mob, float delta);
    }

	public class MobThinkStunned : IMobThinker
	{
		public void Think(EntMob mob, float delta)
		{
			Vector3 zero = new Vector3();
			// calculate self move
			mob.selfMove = FPSController.CalcVelocityQuakeStyle(
				mob.velocity, zero, mob.mobDef.walkSpeed, delta, true, mob.mobDef.friction, mob.mobDef.accelForce);
			mob.velocity = mob.selfMove;
			mob.velocity += mob.pushAccumulator;
			mob.pushAccumulator = Vector3.Zero;
			if (mob.velocity.LengthSquared() > Game.MaxActorVelocity * Game.MaxActorVelocity)
			{
				mob.velocity = mob.velocity.Normalized();
				mob.velocity *= Game.MaxActorVelocity;
			}
			Vector3 result = mob.body.MoveAndSlide(mob.velocity);
			if (mob.thinkTick <= 0) { mob.thinkIndex = mob.mobDef.thinkIndexBase; }
			else { mob.thinkTick -= delta; }
		}
	}

	public class MobThinkDefault : IMobThinker
    {
		private MobThink _mobThink;
		public MobThinkDefault(MobThink mobThink)
		{
			_mobThink = mobThink;
		}

        public void Think(EntMob mob, float delta)
        {
			int stunAccum = mob.stunAccumulator;
			mob.stunAccumulator = 0;
			if (stunAccum >= mob.mobDef.stunThreshold)
			{
				// switch to stun and update with that instead
				_mobThink.ApplyStun(mob);
				_mobThink.UpdateMob(mob, delta);
				return;
			}

			IActor actor = Main.i.game.CheckTarget(mob.targetActorId, mob.GetTeam());
			if (actor == null)
			{
				mob.targetActorId = Game.NullActorId;
				return;
			}
			mob.targetActorId = actor.actorId;
			Transform self = mob.GetTransformForTarget();
			Vector3 tar = actor.GetTransformForTarget().origin;
			float yawDeg = ZqfGodotUtils.FlatYawDegreesBetween(
				self.origin, tar);
			mob.body.RotationDegrees = new Vector3(0, yawDeg + 180, 0);
			self = mob.GetTransformForTarget();
			Vector3 toTar = (tar - self.origin).Normalized();

			if (mob.thinkTick <= 0)
			{
				mob.thinkTick = 1;
				PointProjectile prj = Main.i.factory.SpawnPointProjectile();
				prj.Launch(self.origin, toTar, mob.prjDef, mob.body, Team.Mobs);
			}
			else
			{
				mob.thinkTick -= delta;
			}

			if (mob.moveTick <= 0)
			{
				float rand = ZqfGodotUtils.Randomf();
				float dist = ZqfGodotUtils.Distance(self.origin, tar);
				mob.moveTick = 1.5f;
				// move toward target
				if (dist > mob.mobDef.evadeRange)
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
					mob.moveTick = 1f;
				}
				else
				{
					// evade either left or right
					// don't add a full 90 degrees as will evade straight out of evade dist
					yawDeg += (rand > 0.5) ? 70 : -70;
					mob.moveTick = 1.5f;
				}
				float radians = Mathf.Deg2Rad(yawDeg);
				mob.lastSelfDir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
			}
			else
			{
				mob.moveTick -= delta;
			}
			//////////////////////////////////////
			// Move

			// calculate self move
			mob.selfMove = FPSController.CalcVelocityQuakeStyle(
				mob.velocity, mob.lastSelfDir, mob.mobDef.walkSpeed, delta, true, mob.mobDef.friction, mob.mobDef.accelForce);

			mob.velocity = mob.selfMove;
			// apply push forces
			mob.velocity += mob.pushAccumulator;
			mob.pushAccumulator = Vector3.Zero;
			if (mob.velocity.LengthSquared() > Game.MaxActorVelocity * Game.MaxActorVelocity)
			{
				mob.velocity = mob.velocity.Normalized();
				mob.velocity *= Game.MaxActorVelocity;
			}
			Vector3 result = mob.body.MoveAndSlide(mob.velocity);
		}
    }

    public class MobThink
    {
        public const int DefaultThink = 0;
		public const int StunThink = 1;
		public const int LastThinker = 2;

        private IMobThinker[] _thinkers = new IMobThinker[LastThinker];

        public MobThink()
        {
            _thinkers[DefaultThink] = new MobThinkDefault(this);
			_thinkers[StunThink] = new MobThinkStunned();
		}

		public void ApplyStun(EntMob mob)
		{
			Console.WriteLine($"Mob - apply stun");
			mob.thinkIndex = StunThink;
			mob.thinkTick = mob.mobDef.stuntime;
		}

        public void UpdateMob(EntMob mob, float delta)
        {

			if (mob.thinkIndex < 0) { mob.thinkIndex = 0; }
			else if (mob.thinkIndex >= LastThinker) { mob.thinkIndex = 0; }
			_thinkers[mob.thinkIndex].Think(mob, delta);
		}
    }
}
