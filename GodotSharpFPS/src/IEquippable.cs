using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src
{
    public interface IEquippable
    {
        string GetDisplayName();
        bool CanEquip();
        int GetLoadedAmmo();
        bool CanSwitchAway();
        void SetEquipped(bool flag);
        void Tick(float delta, bool primaryOn, bool secondaryOn, AttackSource src);
    }
}
