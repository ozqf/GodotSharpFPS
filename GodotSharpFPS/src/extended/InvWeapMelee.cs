using Godot;
using GodotSharpFps.src.nodes;
using System;

namespace GodotSharpFps.src.extended
{
    public class InvWeapMelee : InvProjectileWeapon
    {
        private MeleeHitVolume _volume = null;

        public InvWeapMelee(
            Spatial launchNode,
            WeaponDef weaponDef,
            ProjectileDef primaryDef,
            ProjectileDef secondaryDef,
            PhysicsBody ignoreBody)
            : base(launchNode, weaponDef, primaryDef, secondaryDef, ignoreBody)
        {
        }

        public void SetMeleeVolume(MeleeHitVolume volume)
        {
            _volume = volume;
        }

        public override void FirePrimary(AttackSource src)
        {
            if (_volume == null)
            {
                throw new NullReferenceException($"Melee weapon has no melee volume");
            }
            _tick = _weaponDef.primaryRefireTime;
            _volume.Fire(src);
        }
    }
}
