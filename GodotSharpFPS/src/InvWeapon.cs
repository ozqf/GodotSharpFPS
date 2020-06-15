using Godot;
using System;

namespace GodotSharpFps.src
{
    public class InvWeapon
    {
        protected Spatial _launchNode;
        protected PhysicsBody _ignoreBody;

        protected WeaponDef _weaponDef;
        protected ProjectileDef _primaryPrjDef;
        protected ProjectileDef _secondaryPrjDef;

        protected int _ownerId;
        protected int _primaryOn;
        protected int _secondaryOn;
        protected float _tick;
        protected float _primaryRefireTime;
        protected float _secondaryRefireTime;

        public InvWeapon(
            Spatial launchNode,
            WeaponDef weaponDef,
            ProjectileDef primaryDef,
            ProjectileDef secondaryDef,
            PhysicsBody ignoreBody)
        {
            _launchNode = launchNode;
            _ignoreBody = ignoreBody;

            _weaponDef = weaponDef;
            _primaryPrjDef = primaryDef;
            _secondaryPrjDef = secondaryDef;
        }

        public void FirePrimary()
        {
            if (_primaryPrjDef == null) { return; }
            PointProjectile prj = Main.instance.factory.SpawnPointProjectile();
            if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
            prj.Launch(_launchNode.GlobalTransform, _primaryPrjDef, _ignoreBody);
            _tick = _weaponDef.primaryRefireTime;
        }

        public void FireSecondary()
        {
            if (_secondaryPrjDef == null) { return; }
            PointProjectile prj = Main.instance.factory.SpawnPointProjectile();
            if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
            prj.Launch(_launchNode.GlobalTransform, _primaryPrjDef, _ignoreBody);
            _tick = _weaponDef.secondaryRefireTime;
        }

        protected void CheckTriggers(bool primaryOn, bool secondaryOn)
        {
            if (primaryOn)
            { FirePrimary(); }
            else if (secondaryOn)
            { FireSecondary(); }
        }

        protected void CommonTick(float delta, bool primaryOn, bool secondaryOn)
        {
            if (_tick > 0) { _tick -= delta; }
            else { CheckTriggers(primaryOn, secondaryOn); }
        }

        public void Tick(float delta, bool primaryOn, bool secondaryOn)
        {
            CommonTick(delta, primaryOn, secondaryOn);
        }
    }
}
