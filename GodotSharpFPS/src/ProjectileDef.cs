
namespace GodotSharpFps.src
{
    public class ProjectileDef
    {
        public float launchSpeed = 20;
        public float damage = 10;
        public float timeToLive = 4;
    }

    public class WeaponDef
    {
        public float primaryRefireTime;
        public int primaryPrjCount;
        public float secondaryRefireTime;

        public void Validate()
        {
            if (primaryRefireTime <= 0) { primaryRefireTime = 1; }
            if (secondaryRefireTime <= 0) { secondaryRefireTime = 1; }
            if (primaryPrjCount <= 0) { primaryPrjCount = 1; }
        }
    }
}
