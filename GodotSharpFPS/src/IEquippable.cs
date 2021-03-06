﻿
namespace GodotSharpFps.src
{
    public struct EquippableTickInfo
    {
        public bool primaryWasOn;
        public bool primaryOn;

        public bool secondaryWasOn;
        public bool secondaryOn;
        public AttackSource src;

        public EquippableTickInfo(
            bool inputPrimaryOn,
            bool inputPrimaryWasOn,
            bool inputSecondaryOn,
            bool inputSecondaryWasOn,
            AttackSource atkSource)
        {
            primaryOn = inputPrimaryOn;
            primaryWasOn = inputPrimaryWasOn;
            secondaryOn = inputSecondaryOn;
            secondaryWasOn = inputSecondaryWasOn;
            src = atkSource;
        }
    }

    public interface IEquippable
    {
        string GetDisplayName();
        bool CanEquip();
        int GetLoadedAmmo();
        float GetRefireLerp();
        bool CanSwitchAway();
        void SetEquipped(bool flag);
        void Tick(float delta, EquippableTickInfo info);
    }
}
