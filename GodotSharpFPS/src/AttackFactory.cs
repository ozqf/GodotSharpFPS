using Godot;
using GodotSharpFps.src.extended;

namespace GodotSharpFps.src
{
    public class AttackFactory
    {
        public static InvWeapon CreatePlayerShotgun(Spatial launchNode, PhysicsBody ignoreBody)
        {
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Triple-Stick";
			weapDef.magazineSize = 3;
			weapDef.magazineReloadTime = 2f;

			weapDef.primaryRefireTime = 0.4f;
			weapDef.primaryPrjCount = 10;

			weapDef.secondaryRefireTime = weapDef.primaryRefireTime * weapDef.magazineSize;
			weapDef.secondaryPrjCount = weapDef.primaryPrjCount * weapDef.magazineSize;

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 25;
			primaryPrjDef.launchSpeed = 50;
			primaryPrjDef.timeToLive = 1;

			// No secondary def, use primary twice

			InvWeapShotgun weapon = new InvWeapShotgun(launchNode, weapDef, primaryPrjDef, primaryPrjDef, ignoreBody);
			return weapon;
		}

		public static InvWeapon CreateStakegun(Spatial launchNode, PhysicsBody ignoreBody)
		{
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Stakegun";
			weapDef.primaryRefireTime = 0.2f;
			weapDef.secondaryRefireTime = 2;
			weapDef.primaryPrjCount = 1;
			weapDef.secondaryPrjCount = 4;
			weapDef.magazineSize = 4;

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 25;
			primaryPrjDef.launchSpeed = 50;
			primaryPrjDef.timeToLive = 2;
			primaryPrjDef.prefabPath = GameFactory.Path_StakeProjectile;

			ProjectileDef secondaryPrjDef = new ProjectileDef();
			secondaryPrjDef.damage = 25;
			secondaryPrjDef.launchSpeed = 50;
			secondaryPrjDef.timeToLive = 2;
			secondaryPrjDef.prefabPath = GameFactory.Path_StakeProjectile;

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
