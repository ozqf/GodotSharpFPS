
namespace GodotSharpFps.src
{
    /// <summary>
    /// Static mob properties
    /// </summary>
    public class MobDef
    {
        public string mobTypeName = string.Empty;
        public string prefabPath = string.Empty;

        public int defaultHealth = 100;
        public float stuntime = 0.5f;
        public int stunThreshold = 1;
        public float evadeRange = 10;
        public float walkSpeed = 5;
        public int friction = 5;
        public int accelForce = 50;

        public float pushMultiplier = 1;
        public int thinkIndexBase = 0;
    }
}
