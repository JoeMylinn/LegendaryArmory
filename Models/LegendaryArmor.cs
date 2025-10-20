using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.Models
{
    internal class LegendaryArmor : LegendaryItem
    {
        public ItemArmorSlotType Slot {  get; set; }

        public ItemWeightType WeightClass { get; set; }

        public int Defense { get; set; }
    }
}
