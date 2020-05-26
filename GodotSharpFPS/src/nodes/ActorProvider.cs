using Godot;

namespace GodotSharpFps.src.nodes
{
    public class ActorProvider : Node, IActorProvider
    {
        // if not set, assumes parent is the actor
        private IActor _actor = null;
        public IActor GetActor()
        {
            if (_actor == null)
            {
                // Direct cast to force an exception here
                // rather than in the caller.
                return (IActor)GetParent();
            }
            return _actor;
        }
        public void SetActor(IActor actor) { _actor = actor; }
    }
}
