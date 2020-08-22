using Godot;

namespace GodotSharpFps.src
{
    public class ProjectileDef : Node
    {
        public enum MoveMode { Basic, Accel };
        public enum DestroyMode { Gfx, Embed };

        public enum PhysicalType { Rod, Ball };


        public string prefabPath = string.Empty;
        public MoveMode moveMode = MoveMode.Basic;
        public float launchSpeed = 20;
        public float maxSpeed = 999f;
        public float minSpeed = 0;
        public float accelPerSecond = 0;

        public int damage = 10;
        public DamageType damageType = DamageType.Physical;
        public float timeToLive = 4;
        public DestroyMode destroyMode = DestroyMode.Gfx;
        public ProjectileImpactDef impactDef = null;

        public void Validate()
        {
            if (launchSpeed <= 0) { launchSpeed = 1; }
            if (damage < 0) { damage = 0; }
            if (timeToLive < 0) { timeToLive = 1; }
        }
    }

    public class ProjectileImpactDef
    {
        public enum ImpactType { None, Explode };
        public ImpactType impactType = ImpactType.None;
        public float radius = 1;
        public float damage = 1;
    }

    public class WeaponDef
    {
        public float primaryRefireTime;
        public int primaryPrjCount;
        public Vector2 primarySpread = new Vector2();

        public float secondaryRefireTime;
        public int secondaryPrjCount;
        public Vector2 secondarySpread = new Vector2();

        public string name;
        public int magazineSize = 1;
        public float magazineReloadTime = 2;

        public void Validate()
        {
            if (primaryRefireTime <= 0) { primaryRefireTime = 1; }
            if (secondaryRefireTime <= 0) { secondaryRefireTime = 1; }
            if (primaryPrjCount <= 0) { primaryPrjCount = 1; }
            if (secondaryPrjCount <= 0) { secondaryPrjCount = 1; }
        }
    }
}
