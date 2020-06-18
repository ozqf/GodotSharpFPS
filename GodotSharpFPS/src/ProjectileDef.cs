
namespace GodotSharpFps.src
{
    public class ProjectileDef
    {
        public string prefabPath = string.Empty;
        public float launchSpeed = 20;
        public int damage = 10;
        public float timeToLive = 4;
        
        public void Validate()
        {
            if (launchSpeed <= 0) { launchSpeed = 1; }
            if (damage < 0) { damage = 0; }
            if (timeToLive < 0) { timeToLive = 1; }
        }
    }

    public class WeaponDef
    {
        public float primaryRefireTime;
        public int primaryPrjCount;
        public float secondaryRefireTime;
        public int secondaryPrjCount;
        public string name;

        public void Validate()
        {
            if (primaryRefireTime <= 0) { primaryRefireTime = 1; }
            if (secondaryRefireTime <= 0) { secondaryRefireTime = 1; }
            if (primaryPrjCount <= 0) { primaryPrjCount = 1; }
            if (secondaryPrjCount <= 0) { secondaryPrjCount = 1; }
        }
    }
}
