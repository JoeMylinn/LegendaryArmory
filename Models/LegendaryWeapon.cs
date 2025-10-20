using Gw2Sharp;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
