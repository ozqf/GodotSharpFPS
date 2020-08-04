
using Godot;

namespace GodotSharpFps.src
{
    public enum Team { None, Player, Mobs, NonCombatant };

    public enum TouchType
    {
        None, Solid, Bullet, Thrown, Explosion, Melee
    }

    public enum TouchResponseType
    {
        None, Damaged, Killed
    }

    public struct TouchData
    {
        public TouchType touchType;
        public Team teamId;
        public int damage;
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
    }

    public struct AttackSource
    {
        public Team team;
        public int actorId;
        public PhysicsBody ignoreBody;
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
        TouchResponseData ActorTouch(TouchData touchData);
        void RemoveActor();
    }

    public interface IActorProvider
    {
        IActor GetActor();
    }
}
