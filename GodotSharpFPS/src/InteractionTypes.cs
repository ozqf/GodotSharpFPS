
namespace GodotSharpFps.src
{
    public enum TouchType
    {
        None, Solid, Bullet, Thrown, Explosion
    }

    public enum TouchResponseType
    {
        Damaged, Killed
    }

    public class TouchData
    {
        public TouchType touchType;
        public int teamId;
        public int damage;
    }

    public class TouchResponseData
    {
        public TouchResponseType responseType;
        public int damageTaken;
    }

    public interface IActor
    {
        TouchResponseData ActorTouch(TouchData touchData);
    }

    public interface IActorProvider
    {
        IActor GetActor();
    }
}
