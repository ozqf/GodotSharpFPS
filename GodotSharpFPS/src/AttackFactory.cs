using Godot;
using GodotSharpFps.src.extended;

namespace GodotSharpFps.src
{
    public class AttackFactory
    {
        public static InvWeapon CreatePlayerShotgun(Spatial launchNode, PhysicsBody ignoreBody)
        {
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Shotgun";
			weapDef.primaryRefireTime = 0.02f;
			weapDef.secondaryRefireTime = 0.75f;
			weapDef.primaryPrjCount = 1;
			weapDef.secondaryPrjCount = 12;

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 25;
			primaryPrjDef.launchSpeed = 35;
			primaryPrjDef.timeToLive = 1;

			ProjectileDef secondaryPrjDef = new ProjectileDef();
			secondaryPrjDef.damage = 25;
			secondaryPrjDef.launchSpeed = 35;
			secondaryPrjDef.timeToLive = 1;
			secondaryPrjDef.prefabPath = GameFactory.Path_StakeProjectile;

			InvWeapShotgun weapon = new InvWeapShotgun(launchNode, weapDef, primaryPrjDef, secondaryPrjDef, ignoreBody);
			return weapon;
		}

		public static InvWeapon CreateStakegun(Spatial launchNode, PhysicsBody ignoreBody)
		{
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Launcher";
			weapDef.primaryRefireTime = 0.02f;
			weapDef.secondaryRefireTime = 0.75f;
			weapDef.primaryPrjCount = 1;
			weapDef.secondaryPrjCount = 12;

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 25;
			primaryPrjDef.launchSpeed = 35;
			primaryPrjDef.timeToLive = 1;

			ProjectileDef secondaryPrjDef = new ProjectileDef();
			secondaryPrjDef.damage = 25;
			secondaryPrjDef.launchSpeed = 35;
			secondaryPrjDef.timeToLive = 1;

			InvWeapStakegun weapon = new InvWeapStakegun(launchNode, weapDef, primaryPrjDef, secondaryPrjDef, ignoreBody);
			return weapon;
		}

		public static InvWeapon CreateLauncher(Spatial launchNode, PhysicsBody ignoreBody)
		{
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Launcher";
			weapDef.primaryRefireTime = 0.02f;
			weapDef.secondaryRefireTime = 0.75f;
			weapDef.primaryPrjCount = 1;
			weapDef.secondaryPrjCount = 12;

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 25;
			primaryPrjDef.launchSpeed = 35;
			primaryPrjDef.timeToLive = 1;

			ProjectileDef secondaryPrjDef = new ProjectileDef();
			secondaryPrjDef.damage = 25;
			secondaryPrjDef.launchSpeed = 35;
			secondaryPrjDef.timeToLive = 1;

			InvWeapon weapon = new InvWeapon(launchNode, weapDef, primaryPrjDef, secondaryPrjDef, ignoreBody);
			return weapon;
		}
	}
}
