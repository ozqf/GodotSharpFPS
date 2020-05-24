using Godot;

namespace GodotSharpFps.src
{
    public class FPSInput
    {
        public const int BitMoveForward = (1 << 0);
        public const int BitMoveBackward = (1 << 1);
        public const int BitMoveLeft = (1 << 2);
        public const int BitMoveRight = (1 << 3);

        public const int BitLookUp = (1 << 4);
        public const int BitLookDown = (1 << 5);
        public const int BitLookLeft = (1 << 6);
        public const int BitLookRight = (1 << 7);

        public const int BitMoveUp = (1 << 8);
        public const int BitMoveDown = (1 << 9);

        public int buttons;
        public Vector3 rotation;

        public bool isBitOn(int bit)
        {
            return (buttons & bit) != 0;
        }
    }
}
