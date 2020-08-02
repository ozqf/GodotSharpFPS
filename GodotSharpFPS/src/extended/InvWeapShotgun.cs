using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.extended
{
    public class InvWeapShotgun : InvWeapon
    {
        public InvWeapShotgun(
            Spatial launchNode,
            WeaponDef weaponDef,
            ProjectileDef primaryDef,
            ProjectileDef secondaryDef,
            PhysicsBody ignoreBody)
            : base(launchNode, weaponDef, primaryDef, secondaryDef, ignoreBody)
        {
            _roundsLoaded = weaponDef.magazineSize;
        }

        public override int GetLoadedAmmo()
        {
            return _roundsLoaded;
        }

        public override void FirePrimary(AttackSource src)
        {
            base.FirePrimary(src);
            _roundsLoaded--;
            if (_roundsLoaded <= 0)
            {
                _tick = _weaponDef.magazineReloadTime;
                _isReloading = true;
            }
        }

        public override void FireSecondary(AttackSource src)
        {
            int numShots = _roundsLoaded;
            _roundsLoaded = 0;
            _isReloading = true;
            _tick = _weaponDef.magazineReloadTime;
            int numPellets = _weaponDef.primaryPrjCount * numShots;
            Console.WriteLine($"Fire {numShots} shots ({numPellets} pellets)");

            Transform t = _launchNode.GlobalTransform;
            // fill all angles, although we may be using less anyway.
            ZqfGodotUtils.FillSpreadAngles(t, _secondarySpread, 2000, 1200);
            for (int i = 0; i < numPellets; ++i)
            {
                PointProjectile prj = Main.i.factory.SpawnProjectile(_secondaryPrjDef.prefabPath);
                if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
                prj.Launch(t.origin, _secondarySpread[i], _secondaryPrjDef, src.ignoreBody, src.team);
                _tick = _weaponDef.secondaryRefireTime;
            }
        }
    }
}
