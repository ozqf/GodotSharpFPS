using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GodotSharpFps.src
{
    public class ActorInventory
    {
        Spatial _launchNode;
        //InvWeapon _godHand; // Debugging weapon
        List<InvWeapon> _weapons = new List<InvWeapon>();
        int _currentWeaponIndex = -1;
        int _queuedWeaponSwitchIndex = -1;
        int _ownerId;

        public void Init(Spatial launchNode, int ownerId)
        {
            _launchNode = launchNode;
            _ownerId = ownerId; 
        }

        public int GetLoadedAmmo()
        {
            return -1;
        }

        public void AddWeapon(InvWeapon weap)
        {
            _weapons.Add(weap);
            if (_currentWeaponIndex == -1)
            {
                _currentWeaponIndex = _weapons.Count() - 1;
            }
        }

        #region Weapon List

        private InvWeapon GetCurrentWeapon()
        {
            if (_currentWeaponIndex < 0 || _currentWeaponIndex >= _weapons.Count())
            {
                return null;
            }
            return _weapons[_currentWeaponIndex];
        }

        private int StepWeaponIndex(int current, int step)
        {
            current += step;
            int len = _weapons.Count;
            if (current >= len) { current = 0; }
            else if (current < 0) { current = len - 1; }
            return current;
        }

        private void CycleSelectedWeapon(int step)
        {
            if (step != 1 && step != -1) { return; }
            int escape = 0;
            int current = _currentWeaponIndex;
            for(; ; )
            {
                current = StepWeaponIndex(current, step);
                if (_weapons[current].CanEquip())
                {
                    _queuedWeaponSwitchIndex = current;
                    Console.WriteLine($"Cycle switch to {_queuedWeaponSwitchIndex}");
                    return;
                }
                if (escape > _weapons.Count)
                {
                    Console.WriteLine($"No weapons to cycle to");
                    return;
                }
                escape++;
            }
        }

        public void SelectWeaponByIndex(int index)
        {
            int numSlots = _weapons.Count;
            if (index < 0 || index >= numSlots) { return; }
            _queuedWeaponSwitchIndex = index;
            Console.WriteLine($"Queue switch to {_queuedWeaponSwitchIndex}");
        }

        public void SelectNextWeapon()
        {
            Console.WriteLine($"Select next");
            CycleSelectedWeapon(1);
        }

        public void SelectPrevWeapon()
        {
            Console.WriteLine($"Select prev");
            CycleSelectedWeapon(-1);
        }

        public void FillHudStatus(HUDPlayerState state)
        {
            InvWeapon weap = GetCurrentWeapon();
            if (weap == null)
            {
                state.ammoLoaded = 999;
                state.weaponName = "None";
                return;
            }
            state.weaponName = weap.GetDisplayName();
            state.ammoLoaded = weap.GetLoadedAmmo();
        }

        private void CheckQueuedSwitch()
        {
            if (_queuedWeaponSwitchIndex >= 0)
            {
                InvWeapon cur = GetCurrentWeapon();
                if (cur != null)
                {
                    // check that current weapon is okay
                    if (!cur.CanSwitchAway())
                    {
                        return;
                    }
                    cur.SetEquipped(false);
                }
                
                _currentWeaponIndex = _queuedWeaponSwitchIndex;
                _queuedWeaponSwitchIndex = -1;
                cur = GetCurrentWeapon();
                cur.SetEquipped(true);
            }
        }

        #endregion

        public void Tick(float delta, bool primaryOn, bool secondaryOn)
        {
            CheckQueuedSwitch();
            InvWeapon weap = GetCurrentWeapon();
            // Tick
            if (weap != null)
            {
                weap.Tick(delta, primaryOn, secondaryOn);
            }
        }
    }
}
