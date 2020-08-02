﻿
using Godot;

namespace GodotSharpFps.src
{
    public enum Team { None, Player, Mobs, NonCombatant };

    public enum TouchType
    {
        None, Solid, Bullet, Thrown, Explosion
    }

    public enum TouchResponseType
    {
        None, Damaged, Killed
    }

    public struct TouchData
    {
        public TouchType touchType;
        public int teamId;
        public int damage;
    }

    public struct TouchResponseData
    {
        public TouchResponseType responseType;
        public int damageTaken;

        public static TouchResponseData empty
        {
            get
            {
                return new TouchResponseData
                {
                    responseType = TouchResponseType.None,
                    damageTaken = 0
                };
            }
        }
    }

    public interface IActor
    {
        Team GetTeam();
        void SetActorId(int newId);
        //void ActorDescription();
        int actorId { get; }
        int ParentActorId { get; set; }
        void ChildActorRemoved(int id);
        Transform GetActorTransform();
        TouchResponseData ActorTouch(TouchData touchData);
    }

    public interface IActorProvider
    {
        IActor GetActor();
    }
}
