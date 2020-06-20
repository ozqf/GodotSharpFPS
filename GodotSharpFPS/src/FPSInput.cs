using Godot;
using System.Collections.Generic;

namespace GodotSharpFps.src
{
    internal struct GodotInputToBit
    {
        public string inputName;
        public int bit;
    }
    public class FPSInput
    {
        private const string MoveForward = "move_forward";
        private const string MoveBackward = "move_backward";
        private const string MoveLeft = "move_left";
        private const string MoveRight = "move_right";

        private const string MoveUp = "move_up";
        private const string MoveDown = "move_down";

        private const string LookLeft = "ui_left";
        private const string LookRight = "ui_right";
        private const string LookUp = "ui_up";
        private const string LookDown = "ui_down";

        private const string Attack1 = "attack_1";
        private const string Attack2 = "attack_2";
        private const string Offhand1 = "offhand_1";

        private const string NextSlot = "next_slot";
        private const string PrevSlot = "prev_slot";

        private const string Slot1 = "slot_1";
        private const string Slot2 = "slot_2";
        private const string Slot3 = "slot_3";
        private const string Slot4 = "slot_4";


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

        public const int BitAttack1 = (1 << 10);
        public const int BitAttack2 = (1 << 11);
        public const int BitOffhand1 = (1 << 12);

        public const int BitNextSlot = (1 << 13);
        public const int BitPrevSlot = (1 << 14);

        public const int BitSlot1 = (1 << 15);
        public const int BitSlot2 = (1 << 16);
        public const int BitSlot3 = (1 << 17);
        public const int BitSlot4 = (1 << 18);

        public int buttons;
        public Vector3 rotation;

        private List<GodotInputToBit> _inputs = new List<GodotInputToBit>();

        public FPSInput()
        {
            AddInput(MoveForward, BitMoveForward);
            AddInput(MoveBackward, BitMoveBackward);
            AddInput(MoveLeft, BitMoveLeft);
            AddInput(MoveRight, BitMoveRight);

            AddInput(Attack1, BitAttack1);
            AddInput(Attack2, BitAttack2);
            AddInput(Offhand1, BitOffhand1);

            AddInput(NextSlot, BitNextSlot);
            AddInput(PrevSlot, BitPrevSlot);

            AddInput(Slot1, BitSlot1);
            AddInput(Slot2, BitSlot2);
            AddInput(Slot3, BitSlot3);
            AddInput(Slot4, BitSlot4);
        }

        private void AddInput(string name, int bit)
        {
            GodotInputToBit input = new GodotInputToBit();
            input.inputName = name;
            input.bit = bit;
            _inputs.Add(input);
        }

        public bool isBitOn(int bit)
        {
            return (buttons & bit) != 0;
        }

        private void ApplyInputButtonBit(FPSInput input, string keyName, int bit)
        { if (Input.IsActionPressed(keyName)) { input.buttons |= bit; } }

        public void ReadGodotInputs()
        {
            int len = _inputs.Count;
            for (int i = 0; i < len; ++i)
            {
                ApplyInputButtonBit(this, _inputs[i].inputName, _inputs[i].bit);
            }
        }
    }
}
