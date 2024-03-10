using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD;
using LegendaryArmory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gw2Sharp.WebApi.V2.Models;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework;

namespace LegendaryArmory.UI
{
	internal class ItemView : View
	{ 
		protected FlowPanel BuildCategory(FlowPanel parent, string category)
		{

				return new FlowPanel
				{
					Parent = parent,
					Width = parent.Width - 20,
					HeightSizingMode = SizingMode.AutoSize,
					AutoSizePadding = new Point(5, 5),
					CanCollapse = true,
					Collapsed = false,
					OuterControlPadding = new Vector2(5, 5),
					ControlPadding = new Vector2(5, 5),
					Title = category
				};
			
		}

		 protected void AddItem(FlowPanel parent, int IconId, Tooltip tooltip, int amount, int maxAmount)
		{

			var legendary = new LegendaryImage
			{
				Parent = parent,
				Width = 64,
				Height = 64,
				Texture = GameService.Content.DatAssetCache.GetTextureFromAssetId(IconId),
				Opacity = (float)(amount > 0 ? 1 : 0.3),
				Tooltip = tooltip
			};
		}
	}
}
