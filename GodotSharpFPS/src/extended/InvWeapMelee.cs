using Godot;
using GodotSharpFps.src.nodes;
using System;

namespace GodotSharpFps.src.extended
{
    public class InvWeapMelee : IEquippable
    {
        private MeleeHitVolume _volume = null;
        private float _tick = 0;
        private float _refireTime = 0.5f;
        private int _damage = 25;
        private bool _isEquipped = false;

        public InvWeapMelee(MeleeHitVolume volume, float refireTime, int damage)
        {
            _volume = volume;
            _refireTime = refireTime;
            _damage = damage;
        }

        public string GetDisplayName()
        {
            return "Melee";
        }

        public bool CanEquip()
        {
            return true;
        }

        public int GetLoadedAmmo()
        {
            return 999;
        }

        public bool CanSwitchAway()
        {
            return (_tick <= 0);
        }

        public void SetEquipped(bool flag)
        {
            _isEquipped = flag;
        }

        public void Tick(float delta, EquippableTickInfo info)
        {
            if (_tick <= 0)
            {
                if (info.primaryOn && _volume != null)
                {
                    _tick = _refireTime;
                    _volume.SetDamage(_damage);
                    _volume.Fire(info.src);
                }
            }
            else
            {
                _tick -= delta;
            }
        }
    }
}
