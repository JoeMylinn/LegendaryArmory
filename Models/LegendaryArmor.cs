using Gw2Sharp.WebApi.V2.Models;

namespace LegendaryArmory.Models
{
    internal class LegendaryArmor : LegendaryItem
    {
        public ItemArmorSlotType Slot {  get; set; }

        public ItemWeightType WeightClass { get; set; }

        public int Defense { get; set; }
    }
}
