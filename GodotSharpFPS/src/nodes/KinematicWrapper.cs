using Godot;

namespace GodotSharpFps.src.nodes
{
    public class KinematicWrapper : KinematicBody, IActorProvider
    {
        public IActor actor { get; set; }
        public IActor GetActor()
        {
            return actor;
        }
    }
}
