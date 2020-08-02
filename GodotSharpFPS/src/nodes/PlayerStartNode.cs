using Godot;

namespace GodotSharpFps.src.nodes
{
    public class PlayerStartNode : Spatial
    {
        public override void _Ready()
        {
            base._Ready();
            Main.i.game.RegisterPlayerStart(this);
        }
    }
}
