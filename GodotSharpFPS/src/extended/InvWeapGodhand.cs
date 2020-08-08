using Godot;
using GodotSharpFps.src.nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.extended
{
    public class InvWeapGodhand : IEquippable
    {
        public enum Mode { Deathray, Cannon, Spawn, Last }
        private Mode _mode = Mode.Deathray;
        private bool _equipped = false;
        protected Spatial _launchNode;
        private Spatial _aimLaserNode;
        protected PhysicsBody _ignoreBody;
        private string _displayName = string.Empty;

        private ProjectileDef _riflePrjDef;

        public InvWeapGodhand(
            Spatial launchNode,
            Spatial aimLaserNode,
            PhysicsBody ignoreBody)
        {
            _launchNode = launchNode;
            _aimLaserNode = aimLaserNode;
            _ignoreBody = ignoreBody;
            UpdateDisplayName();
            _riflePrjDef = new ProjectileDef();
            _riflePrjDef.damage = 10000;
            _riflePrjDef.launchSpeed = 1000;
        }

        private void UpdateDisplayName()
        {
            _displayName = $"Godhand - {_mode}";
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
            EntMob mob = Main.i.factory.SpawnMob();
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
            }
        }

    }
}
