using Gw2Sharp;
using Gw2Sharp.Models;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.Models
{ 
	internal class WeaponItem: LegendaryItem
	{
		public ItemWeaponType WeaponType { get; set; } = ItemWeaponType.Unknown;
		public int Generation { get; set; }
		public ProfessionType UsableBy { get; set; } = ProfessionType.Guardian;
		public ProfessionWeaponFlag WieldType { get; set; } = ProfessionWeaponFlag.Unknown;
		public bool Variant {  get; set; } = false;
	}
}
