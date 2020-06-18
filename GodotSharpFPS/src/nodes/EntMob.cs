using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
    public class EntMob : Spatial, IActor, IActorProvider
    {
        private KinematicWrapper _body;
        private bool _dead = false;
        private int _health = 100;
        public IActor GetActor() => this;

        public override void _Ready()
        {
            // find Godot scene nodes
            _body = GetNode<KinematicWrapper>("actor_base");
            _body.actor = this;
        }
        public TouchResponseData ActorTouch(TouchData touchData)
        {
            Console.WriteLine($"Mob hit for {touchData.damage}");
            if (_dead)
            {
                return TouchResponseData.empty;
            }
            _health -= touchData.damage;

            TouchResponseData result;
            result.damageTaken = touchData.damage;
            if (_health <= 0)
            {
                result.responseType = TouchResponseType.Killed;
                _dead = true;
                QueueFree();
            }
            else
            {
                result.responseType = TouchResponseType.Damaged;
            }
            return result;
        }
    }
}
