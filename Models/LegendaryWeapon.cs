using Gw2Sharp;
using System.Collections.Generic;

namespace LegendaryArmory.Models
{
    internal class LegendaryWeapon : LegendaryItem
    {
        public int Generation {  get; set; }

        public List<ProfessionWeaponFlag> WieldType { get; set; }

        public int MinPower { get; set; }

        public int MaxPower { get; set; }

        public string DamageType { get; set; }

        public string WeaponType { get; set; }
    }
}
