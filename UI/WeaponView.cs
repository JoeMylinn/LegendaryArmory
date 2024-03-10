using Blish_HUD;
using Blish_HUD.Controls;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace LegendaryArmory.UI
{
	internal class WeaponView : ItemView
	{
		private List<(string, WeaponItem)> weapons;

		public WeaponView(List<(string, WeaponItem)> weapons)
		{
			this.weapons = weapons;
		}
		protected override void Build(Container buildPanel)
		{
			var itemPanel = new FlowPanel
			{
				Parent = buildPanel,
				Width = buildPanel.ContentRegion.Width,
				Height = buildPanel.ContentRegion.Height,
				CanScroll = true
			};


			foreach(string category in weapons.Select(_ => _.Item1).Distinct()) {
				FlowPanel categoryPanel = base.BuildCategory(itemPanel, CategoryNameOverride(category));

				foreach (WeaponItem item in weapons.Where(_ => _.Item1 == category).Select(_ => _.Item2))
				{
					base.AddItem(categoryPanel, item.IconId, BuildTooltip(item), item.Amount, item.MaxAmount);
				}
			}

			base.Build(buildPanel);
		}

		private Tooltip BuildTooltip(WeaponItem item)
		{
			Tooltip tooltip = new Tooltip();
			Image icon = new Image()
			{
				Parent = tooltip,
				Height = 32,
				Width = 32,
				Texture = GameService.Content.DatAssetCache.GetTextureFromAssetId(item.IconId)
			};
			Label name = new Label()
			{
				Left = icon.Right + 5,
				AutoSizeWidth = true,
				Parent = tooltip,
				TextColor = new Color(152, 52, 242),
			Text = item.Name,
			};
			Label type = new Label()
			{
				Parent = tooltip,
				Top = icon.Bottom + 5,
				AutoSizeWidth = true,
				Text = CategoryNameOverride(item.WeaponType.ToString()),
			};
			Label description = new Label()
			{
				Parent = tooltip,
				Top = type.Bottom + 5,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				WrapText = true,
				TextColor = new Color(159, 247, 230),
				Text = item.Description
			};
			

			return tooltip;
		}

		private string CategoryNameOverride(string category)
		{
			return category switch
			{
				"1" => "Generation 1",
				"2" => "Generation 2",
				"3" => "Generation 3",
				"LongBow" => "Longbow",
				"ShortBow" => "Shortbow",
				//Yes really...
				"Speargun" => "Spear",
				"Harpoon" => "Harpoon Gun",
				_ => category,
			};
		}
	}
}
