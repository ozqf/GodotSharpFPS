using Godot;
using System;
using System.Collections.Generic;

namespace GodotSharpFps.src
{
    public class InvProjectileWeapon : IEquippable
    {
        protected Spatial _launchNode;
        protected PhysicsBody _ignoreBody;

        protected WeaponDef _weaponDef;
        protected ProjectileDef _primaryPrjDef;
        protected ProjectileDef _secondaryPrjDef;

        protected int _ownerId;
        protected int _primaryOn;
        protected int _secondaryOn;
        protected float _lastTickMax;
        protected float _tick;
        protected float _primaryRefireTime;
        protected float _secondaryRefireTime;
        protected bool _isEquipped = false;

        protected bool _isReloading = false;
        protected int _roundsLoaded = 1;


        protected List<Vector3> _primarySpread = new List<Vector3>();
        protected List<Vector3> _secondarySpread = new List<Vector3>();

        public InvProjectileWeapon(
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
                _primarySpread.Add(new Vector3());
            }
            for (int i = 0; i < weaponDef.secondaryPrjCount; ++i)
            {
                _secondarySpread.Add(new Vector3());
            }
        }

        virtual public void SetEquipped(bool flag)
        {
            _isEquipped = flag;
        }

        virtual public string GetDisplayName()
        {
            return _weaponDef.name;
        }

        virtual public int GetLoadedAmmo()
        {
            return 999;
        }

        virtual public float GetRefireLerp()
        {
            if (_lastTickMax == 0) { return 0; }
            float f = _tick / _lastTickMax;
            if (f < 0) { f = 0; }
            if (f > 1) { f = 1; }
            return f;
        }

        virtual public bool CanEquip()
        {
            return true;
        }

        virtual public bool CanSwitchAway()
        {
            if (_primaryRefireTime > 0 || _secondaryRefireTime > 0)
            {
                return false;
            }
            return true;
        }

        virtual public void FirePrimary(AttackSource src)
        {
            if (_primaryPrjDef == null) { return; }
            Transform t = _launchNode.GlobalTransform;

            ZqfGodotUtils.FillSpreadAngles(
                t, _primarySpread, _weaponDef.primarySpread.x, _weaponDef.primarySpread.y);
            for (int i = 0; i < _primarySpread.Count; ++i)
            {
                PointProjectile prj = Main.i.factory.SpawnProjectile(_primaryPrjDef.prefabPath);
                if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
                //prj.Launch(_launchNode.GlobalTransform, _primaryPrjDef, _ignoreBody);

                prj.Launch(t.origin, _primarySpread[i], _primaryPrjDef, src.ignoreBody, src.team);
                _tick = _weaponDef.primaryRefireTime;
                _lastTickMax = _tick;
            }
        }

        virtual public void FireSecondary(AttackSource src)
        {
            if (_secondaryPrjDef == null) { return; }
            Transform t = _launchNode.GlobalTransform;

            ZqfGodotUtils.FillSpreadAngles(
                t, _secondarySpread, _weaponDef.secondarySpread.x, _weaponDef.secondarySpread.y);
            for (int i = 0; i < _secondarySpread.Count; ++i)
            {
                PointProjectile prj = Main.i.factory.SpawnProjectile(_secondaryPrjDef.prefabPath);
                if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
                //prj.Launch(_launchNode.GlobalTransform, _primaryPrjDef, _ignoreBody);

                prj.Launch(t.origin, _secondarySpread[i], _secondaryPrjDef, src.ignoreBody, src.team);
                _tick = _weaponDef.secondaryRefireTime;
                _lastTickMax = _tick;
            }
        }

        virtual protected void CheckTriggers(bool primaryOn, bool secondaryOn, AttackSource src)
        {
            if (primaryOn && _primaryPrjDef != null)
            { FirePrimary(src); }
            else if (secondaryOn && _secondaryPrjDef != null)
            { FireSecondary(src); }
        }

        virtual protected void CommonTick(float delta, bool primaryOn, bool secondaryOn, AttackSource src)
        {
            if (_tick > 0) { _tick -= delta; }
            else
            {
                // check for reloading finish
                if (_isReloading == true)
                {
                    _isReloading = false;
                    _roundsLoaded = _weaponDef.magazineSize;
                }
                CheckTriggers(primaryOn, secondaryOn, src);
            }
        }

        virtual public void Tick(float delta, EquippableTickInfo info)
        {
            CommonTick(delta, info.primaryOn, info.secondaryOn, info.src);
        }
    }
}
