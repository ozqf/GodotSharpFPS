using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
    public class TriggerTouch : Area
    {
        [Signal]
        public delegate void Triggered();

        [Export]
        public string triggerTarget = string.Empty;
        [Export]
        public string message = string.Empty;

        //private string[] emitArgs = new string[1];

        public override void _Ready()
        {
            base._Ready();
            Connect("body_entered", this, "OnBodyEntered");
            //emitArgs[0] = message;
        }

        public void OnBodyEntered(Node body)
        {
            IActor actor = Game.ExtractActor(body);
            if (actor == null)
            {
                Console.WriteLine($"Touch trigger - no actor");
                return;
            }
            
            //Console.WriteLine($"Touch trigger actor: {actor.actorId}");
            //EmitSignal(nameof(Triggered), emitArgs);
            EmitSignal(nameof(Triggered));
        }
    }
}
