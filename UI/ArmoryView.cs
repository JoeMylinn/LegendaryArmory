using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Gw2Sharp;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Services;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LegendaryArmory.UI
{
	internal class ArmoryView : View {

		private ArmoryService armoryService;
		private List<string> twoHanded = new List<string> { "Hammer", "LongBow", "ShortBow", "Greatsword", "Rifle", "Staff", "Speargun", "Harpoon", "Trident" };
		private List<string> oneHanded = new List<string>{ "Sword", "Axe", "Dagger", "Mace", "Pistol", "Scepter" };
		private List<string> offHand = new List<string>{ "Focus", "Warhorn", "Torch", "Shield" };
		private List<string> trinkets = new List<string> { "Accessory", "Ring", "Amulet" };

		public List<(int, LegendaryImage)> LegendaryImages = new List<(int, LegendaryImage)>();

		public ArmoryView(ArmoryService armoryService)
		{
			this.armoryService = armoryService;
			foreach (var item in armoryService.LegendaryItems)
			{
				LegendaryImages.Add((item.Id, ImageFromItem(item)));
			}

		}

		protected override void Build(Container buildPanel)
		{
			var menu = new FlowPanel
			{
				Parent = buildPanel,
				Width = 250,
				Height = buildPanel.ContentRegion.Height,
				FlowDirection = ControlFlowDirection.SingleTopToBottom
			};

			var itemView = new FilterFlowPanel
			{
				Parent = buildPanel,
				Left = menu.Right,
				Width = buildPanel.ContentRegion.Width - menu.Width,
				Height = buildPanel.ContentRegion.Height,
				FlowDirection = ControlFlowDirection.LeftToRight,
				ShowBorder = true,
				CanScroll = true,
				OuterControlPadding = new Vector2(Panel.LEFT_PADDING, Panel.TOP_PADDING)
			};

			BuildCategories(itemView);

			var weaponPanel = new FlowPanel
			{
				Parent = menu,
				Width = menu.Width,
				HeightSizingMode = SizingMode.AutoSize,
				CanCollapse = true,
				Collapsed = false,
				Title = "Weapons",
				FlowDirection = ControlFlowDirection.SingleTopToBottom
			};

			var weaponMenu = new Menu
			{
				Parent = weaponPanel,
				Width = weaponPanel.Width,
				Height = weaponPanel.Height,
				CanSelect = true,
			};

			var armorPanel = new FlowPanel
			{
				Parent = menu,
				Width = menu.Width,
				HeightSizingMode = SizingMode.AutoSize,
				CanCollapse = true,
				Collapsed = true,
				Title = "Armors",
				Location = new Point(0, 36)
			};

			var armorMenu = new Menu
			{
				Parent = armorPanel,
				Width = armorPanel.Width,
				Height = armorPanel.Height,
				CanSelect = true,
			};

			var otherPanel = new FlowPanel
			{
				Parent = menu,
				Width = menu.Width,
				HeightSizingMode = SizingMode.AutoSize,
				CanCollapse = true,
				Collapsed = true,
				Title = "Other"
			};

			var otherMenu = new Menu
			{
				Parent = otherPanel,
				Width = otherPanel.Width,
				Height = otherPanel.Height,
				CanSelect = true,
			};

			var allWeaponsMenuItem = weaponMenu.AddMenuItem("All Types");
			foreach (var category in LegendaryImages.Where(_ => _.Item2.Type == ItemType.Weapon).SelectMany(_ => _.Item2.WieldType).Distinct())
			{
				if (category == ProfessionWeaponFlag.Aquatic) continue;
				var subMenu = weaponMenu.AddMenuItem(category.ToString());
				subMenu.Click += delegate
				{
					DeselctAll(armorMenu);
					DeselctAll(otherMenu);
					itemView.Filter(item => item.Type == ItemType.Weapon && item.WieldType.Contains(category));
				};
			}

			var allArmorsMenuItem = armorMenu.AddMenuItem("All Weights");
			foreach (var category in LegendaryImages.Where(_ => _.Item2.Type == ItemType.Armor).Select(_ => _.Item2.WeightClass).Distinct().ToList())
			{
				var subMenu = armorMenu.AddMenuItem(category.ToString());
				subMenu.Click += delegate
				{
					DeselctAll(weaponMenu);
					DeselctAll(otherMenu);
					itemView.Filter(item => item.Type == ItemType.Armor && item.WeightClass == category);
				};
			}

			var allOtherMenuItem = otherMenu.AddMenuItem("All Types");
			foreach (var category in LegendaryImages.Where(_ => _.Item2.Type != ItemType.Armor && _.Item2.Type != ItemType.Weapon).Select(_ => _.Item2.Type).Distinct().ToList())
			{
				var subMenu = otherMenu.AddMenuItem(category.ToString());
				subMenu.Click += delegate
				{
					DeselctAll(weaponMenu);
					DeselctAll(armorMenu);
					itemView.Filter(item => item.Type == category);
				};
			}



			allWeaponsMenuItem.Click += delegate
			{
				DeselctAll(armorMenu);
				DeselctAll(otherMenu);
				itemView.Filter(item => item.Type == ItemType.Weapon);
			};

			allArmorsMenuItem.Click += delegate
			{
				DeselctAll(weaponMenu);
				DeselctAll(otherMenu);
				itemView.Filter(item => item.Type == ItemType.Armor);
			};

			allOtherMenuItem.Click += delegate
			{
				DeselctAll(weaponMenu);
				DeselctAll(armorMenu);
				itemView.Filter(item => item.Type != ItemType.Armor && item.Type != ItemType.Weapon);
			};

			allWeaponsMenuItem.Select();
			itemView.Filter(item => item.Type == ItemType.Weapon);
			base.Build(buildPanel);
		}

		private LegendaryImage ImageFromItem(Item item)
		{ 
			LegendaryImage image = new LegendaryImage()
			{
				Type = item.Type,
				Texture = armoryService.GetIconFromUrl(item.Icon),
				Width = 64,
				Height = 64,
				Opacity = (float)0.3,
				BasicTooltipText = item.Name,
				MaxAmount = armoryService.legendaryIds.Find(_ => _.Id == item.Id).MaxCount 
			};

			switch(item.Type.ToEnum())
			{
				case ItemType.Weapon:
					AddWeaponValues((ItemWeapon)item, image);
					break;
				case ItemType.Armor:
					AddArmorValues((ItemArmor)item, image);
					break;
				default:
					break;
			}

			return image;
		}

		private void BuildCategories(Container buildPanel)
		{
			//TODO: Simplify to single repeated function
			//Weapon Categories
			var weapons = LegendaryImages.Where(_ => _.Item2.Type == ItemType.Weapon).Select(_ => _.Item2).ToList();
			foreach (var gen in weapons.Select(_ => _.Generation).Distinct().ToList())
			{
				var categoryPanel = new FlowPanel()
				{
					Title = "Generation "  + gen.ToString(),
					Parent = buildPanel,
					Width = buildPanel.ContentRegion.Width - 24,
					HeightSizingMode = SizingMode.AutoSize,
					CanCollapse = true,
					Collapsed = false,
					OuterControlPadding = new Vector2(5, 5),
					ControlPadding = new Vector2(5, 5)
				};
				foreach(var weapon in weapons.Where(_ => _.Generation == gen)) { 
						weapon.Parent = categoryPanel;				
				}
			}

			//Armor Categories
			var armors = LegendaryImages.Where(_ => _.Item2.Type == ItemType.Armor).Select(_ => _.Item2).ToList();
			//Custom sort order, change to custom service later
			foreach (var slot in armors.Select(_ => _.Slot).Distinct().ToList().OrderBy(_ => "HSCGLB".IndexOf(_.ToString()[0])))
			{
				var categoryPanel = new FlowPanel()
				{
					Title = slot.ToString(),
					Parent = buildPanel,
					Width = buildPanel.ContentRegion.Width - 24,
					HeightSizingMode = SizingMode.AutoSize,
					CanCollapse = true,
					Collapsed = false,
					OuterControlPadding = new Vector2(5, 5),
					ControlPadding = new Vector2(5, 5)
				};
				foreach (var armor in armors.Where(_ => _.Slot == slot))
				{
					armor.Parent = categoryPanel;
				}
			}

			//Misc Categories
			var misc = LegendaryImages.Where(_ => _.Item2.Type != ItemType.Weapon && _.Item2.Type != ItemType.Armor).Select(_ => _.Item2).ToList();
			foreach (var category in misc.Select(_ => _.Type).Distinct().ToList())
			{
				var categoryPanel = new FlowPanel()
				{
					Title = category.ToString(),
					Parent = buildPanel,
					Width = buildPanel.ContentRegion.Width - 24,
					HeightSizingMode = SizingMode.AutoSize,
					CanCollapse = true,
					Collapsed = false,
					OuterControlPadding = new Vector2(5, 5),
					ControlPadding = new Vector2(5, 5)
				};
				foreach (var item in misc.Where(_ => _.Type == category))
				{
					item.Parent = categoryPanel;
				}
			}
		}

		private void AddArmorValues(ItemArmor item, LegendaryImage image)
		{
			image.WeightClass = item.Details.WeightClass;
			image.Slot = item.Details.Type;
		}

		private void AddWeaponValues(ItemWeapon item, LegendaryImage image)
		{
			image.Generation = item.Id switch
			{
				<= 30704 => 1,
				> 30704 and <= 90551 => 2,
				> 90551 and <= 97783 => 3,
				_ => 0,
			};
			image.WieldType = GetWeaponFlags(item.Details.Type.ToString());
		}

		private List<ProfessionWeaponFlag> GetWeaponFlags(string weapon)
		{
			//Workaround for inconsistent naming in api
			if(weapon == "Harpoon")
			{
				weapon = "Spear";
			}
			List <ProfessionWeaponFlag> result = new List<ProfessionWeaponFlag>();
			foreach (var flags in armoryService.ProfessionWeapons.Where(_ => _.Key.Equals(weapon, System.StringComparison.OrdinalIgnoreCase)).Select(_ => _.Value.Flags))
			{
				foreach (var flag in flags)
				{
					result.Add(flag);
				}
			}
			return result;
		}

		private void DeselctAll(Menu menu)
		{
			foreach (var menuItem in menu.GetChildrenOfType<MenuItem>())
			{
				menuItem.Deselect();
			}
		}

		public void UpdateAmounts(List<AccountLegendaryArmory> owned)
		{
			foreach (var item in owned) {
				foreach(var img in LegendaryImages.Where(_ => _.Item1 == item.Id)) {
					img.Item2.Amount = item.Count;
				}
			}
		} 

		private void TwoHanded_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
		{
			throw new System.NotImplementedException();
		}


		protected override void Unload()
		{
			base.Unload();
		}
	}
}
