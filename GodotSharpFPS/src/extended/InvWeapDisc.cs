using Godot;
using GodotSharpFps.src.nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.extended
{
    public class InvWeapDisc : InvWeapon
    {
        private SwordThrowProjectile _projectile;

        public InvWeapDisc(
            Spatial launchNode,
            WeaponDef weaponDef,
            ProjectileDef primaryDef,
            ProjectileDef secondaryDef,
            PhysicsBody ignoreBody)
            : base(launchNode, weaponDef, primaryDef, secondaryDef, ignoreBody)
        {
        }

        public void SetDiscProjectile(SwordThrowProjectile proj)
        {
            _projectile = proj;
        }

        public override void FirePrimary(AttackSource src)
        {
            base.FirePrimary(src);
        }
    }
}
