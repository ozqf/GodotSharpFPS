
using Godot;

namespace GodotSharpFps.src
{
    public delegate void HealthChange(int current, int change, TouchData data);
    public delegate void Death();

    public enum Team { None, Player, Mobs, NonCombatant };

    public enum TouchType
    {
        None, Projectile, AreaOfEffect, Melee
    }

    public enum TouchResponseType
    {
        None, Blocked, Damaged, Killed
    }

    public enum DamageType
    {
        Physical, Launch
    }

    public struct TouchData
    {
        public TouchType touchType;
        public Team teamId;
        public int damage;
        public DamageType damageType;
        public Vector3 hitPos;
        public Vector3 hitNormal;
        public int sourceActorId;
    }

    public struct TouchResponseData
    {
        public TouchResponseType responseType;
        public int damageTaken;

        public static TouchResponseData empty
        {
            get
            {
                return new TouchResponseData
                {
                    responseType = TouchResponseType.None,
                    damageTaken = 0
                };
            }
        }

        public static TouchResponseData block
        {
            get
            {
                return new TouchResponseData
                {
                    responseType = TouchResponseType.Blocked,
                    damageTaken = 0
                };
            }
        }
    }

    public struct AttackSource
    {
        public Team team;
        public int actorId;
        public PhysicsBody ignoreBody;
    }

    public interface ITouchable
    {
        TouchResponseData Touch(TouchData touchData);
    }

    public interface ITouchProvider
    {
        ITouchable GetTouchable();
    }

    /// <summary>
    /// Interface for primary game objects and their interactions
    /// </summary>
    public interface IActor
    {
        Team GetTeam();
        void SetActorId(int newId);
        int actorId { get; }
        int ParentActorId { get; set; }
        void ChildActorRemoved(int id);
        Transform GetTransformForTarget();
        void ActorTeleport(Transform newTransform);
        void RemoveActor();
        string GetActorDebugText();
    }

    public interface IActorProvider
    {
        IActor GetActor();
    }
}
