using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.Models
{
	internal class LegendaryItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int IconId { get; set; }
		public string Description { get; set; } = string.Empty;
		public string ChatLink { get; set; }
		public ItemType Type { get; set; }
		public int Amount { get; set; }
		public int MaxAmount { get; set; }
	}
}
