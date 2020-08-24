using Godot;
using GodotSharpFps.src.extended;
using GodotSharpFps.src.nodes;

namespace GodotSharpFps.src
{
    public class AttackFactory
    {
		public static IEquippable CreatePlayerMelee(
			MeleeHitVolume volume,
			SwordThrowProjectile projectile,
			LaserDot aimLaser)
		{
			InvWeapMelee weapon = new InvWeapMelee(
				volume, projectile, aimLaser, 0.3f, 25);
			return weapon;
		}

        public static IEquippable CreatePlayerShotgun(Spatial launchNode, PhysicsBody ignoreBody)
        {
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Triple-Stick";
			weapDef.magazineSize = 3;
			weapDef.magazineReloadTime = 2f;

			weapDef.primaryRefireTime = 0.4f;
			weapDef.primaryPrjCount = 10;
			weapDef.primarySpread = new Vector2(1000, 600);

			weapDef.secondaryRefireTime = weapDef.primaryRefireTime * weapDef.magazineSize;
			weapDef.secondaryPrjCount = weapDef.primaryPrjCount * weapDef.magazineSize;
			weapDef.secondarySpread = new Vector2(2000, 1200);

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 12;
			primaryPrjDef.launchSpeed = 100;
			primaryPrjDef.timeToLive = 2f;// 0.1f;

			PatternDef pattern = new PatternDef();
			pattern.patternType = PatternType.Cone3DRandom;
			pattern.count = 10;
			pattern.scale.x = 1000;
			pattern.scale.y = 600;

			// No secondary def, use primary twice

			InvWeapShotgun weapon = new InvWeapShotgun(
				launchNode, weapDef, primaryPrjDef, pattern);
			return weapon;
		}

		public static IEquippable CreateStakegun(Spatial launchNode, PhysicsBody ignoreBody)
		{
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Stakegun";
			weapDef.primaryRefireTime = 0.3f;
			weapDef.secondaryRefireTime = 2;
			weapDef.primarySpread = new Vector2();
			weapDef.secondarySpread = new Vector2(600, 400);
			weapDef.primaryPrjCount = 1;
			weapDef.secondaryPrjCount = 4;
			weapDef.magazineSize = 4;

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 50;
			primaryPrjDef.launchSpeed = 150;
			primaryPrjDef.timeToLive = 2;
			primaryPrjDef.prefabPath = GameFactory.Path_StakeProjectile;
			primaryPrjDef.destroyMode = ProjectileDef.DestroyMode.Embed;

			InvWeapStakegun weapon = new InvWeapStakegun(launchNode, weapDef, primaryPrjDef, primaryPrjDef, ignoreBody);
			return weapon;
		}

		public static IEquippable CreateLauncher(Spatial launchNode, PhysicsBody ignoreBody)
		{
			WeaponDef weapDef = new WeaponDef();
			weapDef.name = "Launcher";
			weapDef.primaryRefireTime = 0.5f;
			weapDef.secondaryRefireTime = 0.75f;
			weapDef.primaryPrjCount = 1;
			weapDef.secondaryPrjCount = 12;

			ProjectileDef primaryPrjDef = new ProjectileDef();
			primaryPrjDef.damage = 25;
			primaryPrjDef.launchSpeed = 50;
			primaryPrjDef.timeToLive = 1;
			primaryPrjDef.damageType = DamageType.Launch;
			primaryPrjDef.impactDef = new ProjectileImpactDef()
			{
				impactType = ProjectileImpactDef.ImpactType.Explode,
				radius = 5,
				damage = 100
			};

			ProjectileDef secondaryPrjDef = new ProjectileDef();
			secondaryPrjDef.damage = 25;
			secondaryPrjDef.launchSpeed = 35;
			secondaryPrjDef.timeToLive = 1;

			InvProjectileWeapon weapon = new InvProjectileWeapon(launchNode, weapDef, primaryPrjDef, secondaryPrjDef, ignoreBody);
			return weapon;
		}
	}
}
