using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.Models
{


	internal class ArmorItem : LegendaryItem
	{
		public ItemWeightType WeightType { get; set; }
		public ItemArmorSlotType SlotType { get; set; }
		public AquisitionType AquisitionType { get; set; } = AquisitionType.Unknown;
	}
}
