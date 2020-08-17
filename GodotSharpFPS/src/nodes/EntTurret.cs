using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.nodes
{
    public class EntTurret : Spatial
    {
        public override void _PhysicsProcess(float delta)
        {
            IActor actor = Main.i.game.CheckTarget(0, Team.Mobs);
            LookAt(actor.GetTransformForTarget().origin, Vector3.Up);
        }
    }
}
