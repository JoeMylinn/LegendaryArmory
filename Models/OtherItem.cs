using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.Models
{
	internal class OtherItem :LegendaryItem
	{
		public SubType SubType { get; set; }
		public AquisitionType AquisitionType { get; set; } = AquisitionType.Unknown;
	}
}
