using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        InvWeapon GetCurrentWeapon()
        {
            if (_currentWeaponIndex < 0 || _currentWeaponIndex >= _weapons.Count())
            {
                return null;
            }
            return _weapons[_currentWeaponIndex];
        }

        #endregion

        public void Tick(float delta, bool primaryOn, bool secondaryOn)
        {
            InvWeapon weap = GetCurrentWeapon();
            if (weap != null)
            {
                weap.Tick(delta, primaryOn, secondaryOn);
            }
        }
    }
}
