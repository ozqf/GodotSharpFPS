using Godot;

namespace GodotSharpFps.src.nodes
{
    public class EntMob : Spatial, IActor, IActorProvider
    {
        public IActor GetActor() => this;

        public TouchResponseData ActorTouch(TouchData touchData)
        {
            return null;
        }
    }
}
