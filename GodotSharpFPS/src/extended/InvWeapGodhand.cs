﻿using Godot;
using GodotSharpFps.src.nodes;
using System;

namespace GodotSharpFps.src.extended
{
    public class InvWeapGodhand : IEquippable
    {
        public enum Mode { Deathray, DebugTag, Spawn, CycleSpawn, Last }
        private Mode _mode = Mode.Deathray;
        private bool _equipped = false;
        private Spatial _launchNode;
        private Spatial _aimLaserNode;
        private string _displayName = string.Empty;
        private int _aimActorId = Game.NullActorId;
        private ProjectileDef _riflePrjDef;

        private string[] _mobTypes;
        private int _mobTypeIndex = 0;
        //private string _mobSpawnType = string.Empty;

        public InvWeapGodhand(
            Spatial launchNode,
            Spatial aimLaserNode)
        {
            _launchNode = launchNode;
            _aimLaserNode = aimLaserNode;
            _riflePrjDef = new ProjectileDef();
            _riflePrjDef.damage = 10000;
            _riflePrjDef.launchSpeed = 1000;
            _mobTypes = Main.i.factory.GetMobTypeList();
            //_mobSpawnType = _mobTypes[0];
            _mobTypeIndex = 0;
            //_mobSpawnType = GameFactory.MobType_Titan;
            UpdateDisplayName();
        }

        private void UpdateDisplayName()
        {
            _displayName = $"Godhand - {_mode}";
            if (_mode == Mode.DebugTag)
            {
                if (_aimActorId != Game.NullActorId)
                {
                    _displayName += $" - actor {_aimActorId}";
                }
            }
            else if (_mode == Mode.Spawn)
            {
                _displayName += $" - {_mobTypes[_mobTypeIndex]}";
            }
            else if (_mode == Mode.CycleSpawn)
            {
                _displayName += $" - {_mobTypes[_mobTypeIndex]}";
            }
        }

        virtual public float GetRefireLerp()
        {
            return 0;
        }


        public bool CanEquip()
        {
            return true;
        }

        public bool CanSwitchAway()
        {
            return true;
        }

        public string GetDisplayName()
        {
            return _displayName;
        }

        public int GetLoadedAmmo()
        {
            return 999;
        }

        public void SetEquipped(bool flag)
        {
            _equipped = flag;
        }

        private void FireDeathRayShot(EquippableTickInfo info)
        {
            PointProjectile prj = Main.i.factory.SpawnProjectile(string.Empty);
            if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }

            Transform t = _launchNode.GlobalTransform;
            prj.Launch(t.origin, -t.basis.z, _riflePrjDef, info.src.ignoreBody, info.src.team);
        }

        private void SpawnMob(Vector3 pos)
        {
            EntMob mob = Main.i.factory.SpawnMob(_mobTypes[_mobTypeIndex]);
            ZqfGodotUtils.Teleport(mob, pos);
        }

        public void Tick(float delta, EquippableTickInfo info)
        {
            if (info.secondaryWasOn)
            {
                _mode++;
                if (_mode >= Mode.Last) { _mode = Mode.Deathray; }
                UpdateDisplayName();
            }
            switch (_mode)
            {
                case Mode.Deathray:
                    if (info.primaryOn) { FireDeathRayShot(info); }
                    break;
                case Mode.Spawn:
                    if (info.primaryWasOn)
                    {
                        if (_aimLaserNode == null)
                        {
                            Console.WriteLine($"Godhand has no aim laser for spawning");
                            break;
                        }
                        Vector3 pos = _aimLaserNode.GlobalTransform.origin;
                        SpawnMob(pos);
                    }
                    break;
                case Mode.CycleSpawn:
                    if (info.primaryWasOn)
                    {
                        _mobTypeIndex++;
                        if (_mobTypeIndex >= _mobTypes.Length)
                        {
                            _mobTypeIndex = 0;
                        }
                        UpdateDisplayName();
                    }
                    break;
                case Mode.DebugTag:
                    //_aimActorId
                    Transform t = _launchNode.GlobalTransform;
                    Vector3 origin = t.origin;
                    Vector3 dest = origin + (-t.basis.z * 1000);
                    Godot.Collections.Dictionary hitDict = ZqfGodotUtils
                        .CastRay(_launchNode, origin, dest, uint.MaxValue, info.src.ignoreBody);
                    if (hitDict.Keys.Count == 0)
                    {
                        _aimActorId = Game.NullActorId;
                        UpdateDisplayName();
                        break;
                    }
                    IActor actor = Game.ExtractActor(hitDict["collider"]);
                    if (actor == null)
                    {
                        _aimActorId = Game.NullActorId;
                        UpdateDisplayName();
                        break;
                    }
                    int newId = actor.actorId;
                    // refresh actor Id if required
                    if (_aimActorId != newId)
                    {
                        _aimActorId = newId;
                        UpdateDisplayName();
                    }
                    if (info.primaryWasOn)
                    {
                        Main.i.game.SetDebugActorId(_aimActorId);
                    }
                    break;
            }
        }

    }
}
