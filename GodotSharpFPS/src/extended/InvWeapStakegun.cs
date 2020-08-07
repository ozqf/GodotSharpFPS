using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.extended
{
    public class InvWeapStakegun : InvProjectileWeapon
    {


        public InvWeapStakegun(Spatial launchNode, WeaponDef weaponDef, ProjectileDef primaryDef, ProjectileDef secondaryDef, PhysicsBody ignoreBody) : base(launchNode, weaponDef, primaryDef, secondaryDef, ignoreBody)
        {
        }
    }
}
