using Gw2Sharp;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.Models
{
    [CastableType(ItemType.Armor, typeof(LegendaryArmor))]
    [CastableType(ItemType.Weapon, typeof(LegendaryWeapon))]
    internal class LegendaryItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public int Amount { get; set; } 

        public int MaxAmount { get; set; } 

        public ItemType Type { get; set; }

        public int Level { get; set; }

        public string Description { get; set; }

        public List<String> Flags { get; set; } = new List<String>();
    }
}
