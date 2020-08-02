using Godot;
using GodotSharpFps.src.nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src
{
    public class Game
    {
        public const int NullActorId = 0;

        private readonly Main _main;

        private enum State { Limbo, Pregame, Gamplay, GameOver, PostGame };
        private State _state = State.Limbo;

        private int _nextEntId = 1;

        // teehee check this is a .net dictionary and not a godot dictionary...
        private Dictionary<int, IActor> _ents = new Dictionary<int, IActor>();
        private List<PlayerStartNode> _playerStarts = new List<PlayerStartNode>();
        private EntPlayer _player = null;

        public Game(Main main)
        {
            _main = main;
            _main.AddObserver(OnGlobalEvent, this, false, "GameController");
            _main.console.AddCommand("actors", "", "Print actor list", Cmd_PrintActorRegister);
        }

        public void OnGlobalEvent(GlobalEventType type, object obj)
        {
            if (type == GlobalEventType.MapChange)
            {
                // Clear any records of entities as they are about to be freed
                _playerStarts.Clear();
                _ents.Clear();
                _player = null;
                _nextEntId = 1;
                // Enter a 'no game rules' state.
                _state = State.Limbo;
            }
        }

        public void Tick()
        {
            switch (_state)
            {
                case State.Pregame:
                    if (Input.IsActionJustPressed("ui_accept") && _playerStarts.Any())
                    {
                        _state = State.Gamplay;
                        PlayerStartNode node = _playerStarts[0];
                        EntPlayer plyr = _main.factory.SpawnPlayer();
                        _player = plyr;
                        plyr.GlobalTransform = node.GlobalTransform;
                    }
                    break;
            }
        }

        public bool CheckTeamVsTeam(Team attacker, Team victim)
        {
            if (victim == Team.None) { return true; }
            if (victim == Team.NonCombatant) { return false; }
            return attacker != victim;
        }

        public IActor CheckTarget(int currentTargetActorId, Team attacker)
        {
            IActor actor = null;
            // validate current target
            if (currentTargetActorId != NullActorId)
            {
                actor = GetActor(currentTargetActorId);
                // > Check actor is in game and
                // > Check actor team is valid to attack
                // Teams may change - eg during death to keep
                // in game but drop targetting
                if (actor != null && CheckTeamVsTeam(attacker, actor.GetTeam()))
                {
                    // okay!
                    return actor;
                }
            }
            // Acquire a target
            if (attacker == Team.Mobs && _player != null)
            {
                actor = _player;
            }
            return actor;
        }

        #region Register misc static objects

        public void RegisterPlayerStart(PlayerStartNode node)
        {
            Vector3 p = node.GlobalTransform.origin;
            Console.WriteLine($"Register player start at {p}");
            if (_playerStarts.Count == 0)
            {
                _state = State.Pregame;
            }
            _playerStarts.Add(node);
        }

        #endregion

        #region Actor Register

        public bool Cmd_PrintActorRegister(string command, string[] tokens)
        {
            Console.WriteLine($"=== Actor Register ===");
            foreach (int key in _ents.Keys)
            {
                IActor a = _ents[key];
                Console.WriteLine($"{key} - {a} - {a.GetType()}");
            }
            return true;
        }

        public IActor GetActor(int id)
        {
            if (_ents.ContainsKey(id))
            {
                return _ents[id];
            }
            return null;
        }

        public int ReserveActorId(int numIdsToReserve)
        {
            if (numIdsToReserve <= 0) { throw new ArgumentException(nameof(numIdsToReserve)); }
            int result = _nextEntId;
            _nextEntId += numIdsToReserve;
            return result;
        }

        public void RegisterActor(IActor actor)
        {
            if (actor.actorId == 0)
            { actor.SetActorId(ReserveActorId(1)); }
            //if (_ents.ContainsKey(actor.actorId))
            //{ throw new ArgumentException($"Actor Id {actor.actorId} already registered"); }

            Console.WriteLine($"Register actor {actor.actorId} - {actor}");
            _ents.Add(actor.actorId, actor);
        }

        public void DeregisterActor(IActor actor)
        {
            Console.WriteLine($"Deregister actor {actor.actorId}");
            if (actor.ParentActorId != 0)
            {
                Console.WriteLine($"Check parent {actor.ParentActorId}");
                IActor parent = GetActor(actor.ParentActorId);
                // parent might have been removed...
                if (parent != null)
                {
                    parent.ChildActorRemoved(actor.actorId);
                }
            }
            _ents.Remove(actor.actorId);
        }

        #endregion

    }
}
