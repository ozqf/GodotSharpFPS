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

        private int _entId = 0;

        public override void _Ready()
        {
            // find Godot scene nodes
            _body = GetNode<KinematicWrapper>("actor_base");
            _body.actor = this;

            if (_entId == 0)
            {
                // no id previous set, request one
                Console.WriteLine($"EntMob - no id set - registering self");
                _entId = Main.i.factory.ReserveActorId(1);
                Main.i.factory.RegisterActor(this);
            }
        }

        public void SetActorId(int newId)
        {
            if (_entId != 0) { throw new Exception($"EntMob already has an actor Id"); }
            _entId = newId;
        }

        public int ParentActorId { get; set; }

        public int actorId { get { return _entId; } }

        public void ChildActorRemoved(int id) { }

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
                Main.i.factory.DeregisterActor(this);
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
