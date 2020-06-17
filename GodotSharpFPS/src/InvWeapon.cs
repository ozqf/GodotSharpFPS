using Godot;
using System;
using System.Collections.Generic;

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

        protected List<Vector3> _spread = new List<Vector3>();

        public InvWeapon(
            Spatial launchNode,
            WeaponDef weaponDef,
            ProjectileDef primaryDef,
            ProjectileDef secondaryDef,
            PhysicsBody ignoreBody)
        {
            weaponDef.Validate();

            _launchNode = launchNode;
            _ignoreBody = ignoreBody;

            _weaponDef = weaponDef;
            _primaryPrjDef = primaryDef;
            _secondaryPrjDef = secondaryDef;
            for (int i = 0; i < weaponDef.primaryPrjCount; ++i)
            {
                _spread.Add(new Vector3());
            }
        }

        virtual public void FirePrimary()
        {
            if (_primaryPrjDef == null) { return; }
            Transform t = _launchNode.GlobalTransform;

            ZqfGodotUtils.FillSpreadAngles(t, _spread);
            for (int i = 0; i < _spread.Count; ++i)
            {
                PointProjectile prj = Main.instance.factory.SpawnPointProjectile();
                if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
                //prj.Launch(_launchNode.GlobalTransform, _primaryPrjDef, _ignoreBody);

                prj.Launch(t.origin, _spread[i], _primaryPrjDef, _ignoreBody);
                _tick = _weaponDef.primaryRefireTime;
            }
        }

        virtual public void FireSecondary()
        {
            if (_secondaryPrjDef == null) { return; }
            PointProjectile prj = Main.instance.factory.SpawnPointProjectile();
            if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
            Transform t = _launchNode.GlobalTransform;
            prj.Launch(t.origin, -t.basis.z, _primaryPrjDef, _ignoreBody);
            _tick = _weaponDef.primaryRefireTime;
        }

        virtual protected void CheckTriggers(bool primaryOn, bool secondaryOn)
        {
            if (primaryOn)
            { FirePrimary(); }
            else if (secondaryOn)
            { FireSecondary(); }
        }

        virtual protected void CommonTick(float delta, bool primaryOn, bool secondaryOn)
        {
            if (_tick > 0) { _tick -= delta; }
            else { CheckTriggers(primaryOn, secondaryOn); }
        }

        virtual public void Tick(float delta, bool primaryOn, bool secondaryOn)
        {
            CommonTick(delta, primaryOn, secondaryOn);
        }
    }
}
