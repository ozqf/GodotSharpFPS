using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
    public class TriggerTouch : Area
    {
        [Export]
        public string triggerTarget = string.Empty;
        public override void _Ready()
        {
            base._Ready();
            Connect("body_entered", this, "OnBodyEntered");

        }

        public void OnBodyEntered(Node body)
        {
            IActor actor = Game.ExtractActor(body);
            if (actor == null) { return; }
            Console.WriteLine($"Touch trigger actor: {actor.actorId}");
        }
    }
}
