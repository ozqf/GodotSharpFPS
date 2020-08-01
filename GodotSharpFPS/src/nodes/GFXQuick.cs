using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.nodes
{
    public class GFXQuick : Spatial
    {
        private float _tick;
        private bool _dead;

        public void Spawn(Vector3 origin)
        {
            _dead = false;
            _tick = 1;
            Transform t = GlobalTransform;
            t.origin = origin;
            GlobalTransform = t;
        }

        private void Die()
        {
            if (_dead) { return; }
            _dead = true;
            QueueFree();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_tick <= 0)
            {
                Die();
            }
            else
            {
                _tick -= delta;
            }
        }
    }
}
