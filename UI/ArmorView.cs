using Blish_HUD.Controls;
using Blish_HUD;
using LegendaryArmory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.UI
{
	internal class ArmorView : ItemView
	{
		private List<(string, ArmorItem)> armors;
		public ArmorView(List<(string, ArmorItem)> armors)
		{
			this.armors = armors;
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


			foreach (string category in armors.Select(_ => _.Item1).Distinct())
			{
				FlowPanel categoryPanel = base.BuildCategory(itemPanel, category);

				foreach (ArmorItem item in armors.Where(_ => _.Item1 == category).Select(_ => _.Item2))
				{
					Tooltip tmp = new Tooltip();
					Image img = new Image()
					{
						Parent = tmp,
						Height = 32,
						Width = 32,
						Texture = GameService.Content.DatAssetCache.GetTextureFromAssetId(item.IconId)
					};
					Label test = new Label()
					{
						Left = img.Right + 5,
						AutoSizeWidth = true,
						Parent = tmp,
						Text = item.Name,
					};
					base.AddItem(categoryPanel, item.IconId, tmp, item.Amount, item.MaxAmount);
				}
			}

			base.Build(buildPanel);
		}
	}
}
