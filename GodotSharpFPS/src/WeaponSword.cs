using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src
{
    public class WeaponSword : InvWeapon
    {
        public WeaponSword(Spatial launchNode, WeaponDef weaponDef, ProjectileDef primaryDef, ProjectileDef secondaryDef, PhysicsBody ignoreBody)
            : base(launchNode, weaponDef, primaryDef, secondaryDef, ignoreBody)
        {
        }


    }
}
