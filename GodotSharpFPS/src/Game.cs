using Godot;
using GodotSharpFps.src.nodes;
using System;
using System.Collections.Generic;
using System.Linq;
// alias .net namespace for dictionary
using DotNet = System.Collections.Generic;

namespace GodotSharpFps.src
{
    public class Game
    {
        public const int NullActorId = 0;
        public const float MaxActorVelocity = 50;

        #region Instance

        private readonly Main _main;

        public enum State { Limbo, Pregame, Gamplay, GameOver, PostGame };
        private State _state = State.Limbo;
        private uint _stateTimestamp = 0;
        private string _mapName = string.Empty;

        private int _nextEntId = 1;

        private bool _godMode = false;

        // teehee check this is a .net dictionary and not a godot dictionary...
        // ...godot dictionaries cannot store non-godot types!
        private DotNet.Dictionary<int, IActor> _ents = new DotNet.Dictionary<int, IActor>();
        private List<PlayerStartNode> _playerStarts = new List<PlayerStartNode>();
        private int _plyrId = NullActorId;
        private int _debugActorId = NullActorId;
        private Dictionary<string, InfoPath> _pathNodes = new Dictionary<string, InfoPath>();

        private Spatial _gameRoot;
        public Spatial root { get { return _gameRoot; } }

        public Game(Main main, Spatial gameRootNode)
        {
            _main = main;
            _gameRoot = gameRootNode;
            _main.AddObserver(OnGlobalEvent, this, false, "GameController");
            _main.console.AddCommand("actors", "", "Print actor list", Cmd_PrintActorRegister);
            _main.console.AddCommand("god", "", "Toggle invulnerable player", Cmd_God);
            _main.console.AddCommand("debugplayer", "", "Mark player as debug actor", Cmd_DebugPlayer);
        }

        public void SetDebugActorId(int id)
        {
            _debugActorId = id;
        }

        public bool GodModeOn() { return _godMode; }

        public State GetGameState() { return _state; }

        private void SetGameState(State newState)
        {
            if (_state != newState)
            {
                _state = newState;
                _stateTimestamp = OS.GetTicksMsec();
                _main.Broadcast(GlobalEventType.GameStateChange, _state);
            }
        }

        private void PurgeRegisters()
        {
            foreach (int key in _ents.Keys)
            {
                IActor a = _ents[key];
                a.RemoveActor();
            }
            _ents.Clear();
            _playerStarts.Clear();
            _pathNodes.Clear();
            _plyrId = NullActorId;
            _nextEntId = 1;
        }

        public void OnGlobalEvent(GlobalEventType type, object obj)
        {
            switch (type)
            {
                case GlobalEventType.MapChange:
                    _mapName = obj as string;
                    // Clear any records of entities as they are about to be freed
                    PurgeRegisters();
                    // Enter a 'no game rules' state.
                    SetGameState(State.Limbo);
                    // Remove all children from game root node
                    for (int i = root.GetChildCount() - 1; i >= 0; --i)
                    {
                        root.GetChild(i).QueueFree();
                    }
                    break;
                case GlobalEventType.PlayerDied:
                    _main.cam.DetachFromCustomParent();
                    SetGameState(State.GameOver);
                    break;
            }
        }

        public void Tick()
        {
            switch (GetGameState())
            {
                case State.Pregame:
                    if (_main.gameInputActive
                        && Input.IsActionJustPressed("attack_1")
                        && _playerStarts.Any())
                    {
                        PlayerStartNode node = _playerStarts[0];
                        EntPlayer plyr = _main.factory.SpawnPlayer();
                        _plyrId = plyr.actorId;
                        plyr.GlobalTransform = node.GlobalTransform;
                        SetGameState(State.Gamplay);
                    }
                    break;
                case State.GameOver:
                    uint diff = OS.GetTicksMsec() - _stateTimestamp;
                    if (diff < 1000) { break; }
                    if (_main.gameInputActive
                        && Input.IsActionJustPressed("attack_1"))
                    {
                        string cmd = $"map {_mapName}";
                        Console.WriteLine($"Restart map: {cmd}");
                        _main.console.Execute(cmd);
                    }
                    break;
            }
            if (_debugActorId != NullActorId)
            {
                IActor actor = GetActor(_debugActorId);
                if (actor == null) { _debugActorId = NullActorId; }
                else
                {
                    _main.SetDebugText(actor.GetActorDebugText());
                }
            }
            else
            {
                _main.SetDebugText(string.Empty);
            }
        }

        public bool Cmd_DebugPlayer(string command, string[] tokens)
        {
            if (_plyrId == NullActorId)
            {
                Console.WriteLine($"No registered player to debug");
                return true;
            }
            if (_debugActorId == _plyrId) { return true; }
            _debugActorId = _plyrId;
            Console.WriteLine($"Player avatar {_plyrId} is debug actor");
            return true;
        }

        public bool Cmd_God(string command, string[] tokens)
        {
            _godMode = !_godMode;
            Console.WriteLine($"God mode {_godMode}");
            return true;
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
            if (attacker == Team.Mobs && _plyrId != NullActorId)
            {
                // try and retrieve player
                actor = GetActor(_plyrId);
            }
            return actor;
        }

        #endregion


        #region Game Object Register

        public void RegisterPathNode(InfoPath path)
        {
            if (_pathNodes.ContainsKey(path.Name))
            {
                Console.WriteLine($"WARNING - already have a path node called {path.Name}");
                return;
            }
            _pathNodes.Add(path.Name, path);
        }

        public void DeregisterPathNode(InfoPath path)
        {
            if (_pathNodes.ContainsKey(path.Name))
            {
                _pathNodes.Remove(path.Name);
            }
        }

        public InfoPath GetPathNode(string name)
        {
            if (string.IsNullOrWhiteSpace(name)
                || !_pathNodes.ContainsKey(name))
            {
                return null;
            }
            return _pathNodes[name];
        }

        public void RegisterPlayerStart(PlayerStartNode node)
        {
            Vector3 p = node.GlobalTransform.origin;
            Console.WriteLine($"Register player start at {p}");
            _playerStarts.Add(node);
            if (_playerStarts.Count == 1)
            {
                SetGameState(State.Pregame);
                if (_main.cam.GetParentType() != GameCamera.ParentType.Player)
                {
                    _main.cam.AttachToTarget(node, new Vector3(0, 1, 0), GameCamera.ParentType.Misc);
                }
            }
        }

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
            if (actor.actorId == NullActorId)
            { actor.SetActorId(ReserveActorId(1)); }
            if (_ents.ContainsKey(actor.actorId))
            { throw new ArgumentException($"Actor Id {actor.actorId} already registered"); }

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


        #region Static functions


        public static bool CheckTeamVsTeam(Team attacker, Team victim)
        {
            if (victim == Team.None) { return true; }
            if (victim == Team.NonCombatant) { return false; }
            return attacker != victim;
        }

        public static IActor DescendTreeToActor(Node node)
        {
            Node parent = node.GetParent();
            while (parent != null)
            {
                if (parent is IActor)
                {
                    return parent as IActor;
                }
                parent = parent.GetParent();
            }
            return null;
        }

        public static IActor ExtractActor(object obj)
        {
            IActorProvider provider = obj as IActorProvider;
            if (provider == null) { return null; }
            return provider.GetActor();
        }

        public static ITouchable ExtractTouchable(object obj)
        {
            return obj as ITouchable;
        }

        public static TouchResponseData TouchGameObject(TouchData touchData, object subject)
        {
            ITouchable touchable = ExtractTouchable(subject);
            if (touchable != null)
            {
                return touchable.Touch(touchData);
            }
            /*IActor actor = ExtractActor(subject);
            if (actor != null)
            {
                return actor.ActorTouch(touchData);
            }*/
            return TouchResponseData.empty;
        }

        #endregion

    }
}
