﻿using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Gw2Sharp;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Helper;
using LegendaryArmory.Services;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace LegendaryArmory.UI
{
	internal class ArmoryView : View {

		private ArmoryService armoryService;
		private LoreWindow _loreWindow;
		private List<string> twoHanded = new List<string> { "Hammer", "LongBow", "ShortBow", "Greatsword", "Rifle", "Staff", "Speargun", "Harpoon", "Trident" };
		private List<string> oneHanded = new List<string>{ "Sword", "Axe", "Dagger", "Mace", "Pistol", "Scepter" };
		private List<string> offHand = new List<string>{ "Focus", "Warhorn", "Torch", "Shield" };
		private List<string> trinkets = new List<string> { "Accessory", "Ring", "Amulet" };

		public List<(int, LegendaryImage)> LegendaryImages = new List<(int, LegendaryImage)>();

		public ArmoryView(ArmoryService armoryService)
		{
			this.armoryService = armoryService;
			_loreWindow = new LoreWindow(this);

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
			foreach (var category in LegendaryImages.Where(t => t.Item2.Type == ItemType.Weapon).SelectMany(t => t.Item2.WieldType).Distinct())
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
			foreach (var category in LegendaryImages.Where(t => t.Item2.Type == ItemType.Armor).Select(t => t.Item2.WeightClass).Distinct().ToList())
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
			foreach (var category in LegendaryImages.Where(t => t.Item2.Type != ItemType.Armor && t.Item2.Type != ItemType.Weapon).Select(t => t.Item2.Type).Distinct().ToList())
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
				MaxAmount = armoryService.legendaryIds.Find(a => a.Id == item.Id).MaxCount 
			};

			switch (item.Type.ToEnum())
			{
				case ItemType.Weapon:
					AddWeaponValues((ItemWeapon)item, image);
					break;
				case ItemType.Armor:
					AddArmorValues((ItemArmor)item, image);
					break;
			}

			image.Click += delegate
			{
				_loreWindow.Show(item.Id);
			};

			return image;
		}

		private void BuildCategories(Container buildPanel)
		{
			//TODO: Simplify to single repeated function
			//Weapon Categories
			var weapons = LegendaryImages.Where(t => t.Item2.Type == ItemType.Weapon).Select(t => t.Item2).ToList();
			foreach (var gen in weapons.Select(i => i.Generation).Distinct().ToList())
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
				foreach(var weapon in weapons.Where(i => i.Generation == gen)) { 
						weapon.Parent = categoryPanel;				
				}
			}

			//Armor Categories
			var armors = LegendaryImages.Where(t => t.Item2.Type == ItemType.Armor).Select(t => t.Item2).ToList();
			//Custom sort order, change to custom service later
			foreach (var slot in armors.Select(i => i.Slot).Distinct().ToList().OrderBy(t => "HSCGLB".IndexOf(t.ToString()[0])))
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
				foreach (var armor in armors.Where(i => i.Slot == slot))
				{
					armor.Parent = categoryPanel;
				}
			}

			//Misc Categories
			var misc = LegendaryImages.Where(t => t.Item2.Type != ItemType.Weapon && t.Item2.Type != ItemType.Armor).Select(t => t.Item2).ToList();
			foreach (var category in misc.Select(i => i.Type).Distinct().ToList())
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
				foreach (var item in misc.Where(i => i.Type == category))
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
				> 97783 and <= 103815 => 4,
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
			foreach (var flags in armoryService.ProfessionWeapons.Where(p => p.Key.Equals(weapon, System.StringComparison.OrdinalIgnoreCase)).Select(p => p.Value.Flags))
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
			var updated = new List<int>();

			foreach (var item in owned) {
				foreach (var img in LegendaryImages.Where(t => t.Item1 == item.Id)) {
					if (img.Item2.Amount != item.Count)
					{
						img.Item2.Amount = item.Count;
						updated.Add(img.Item1);
					}
				}
			}

			foreach (var img in LegendaryImages.Where(t => t.Item2.Tooltip == null))
			{
				updated.Add(img.Item1);
			}

			updated.Distinct();

			foreach(var img in LegendaryImages.Where(i => updated.Contains(i.Item1))) {
				var item = armoryService.LegendaryItems.First(i => i.Id == img.Item1);
				switch (item.Type.ToEnum())
				{
					case ItemType.Weapon:
						img.Item2.Tooltip = TooltipHelper.CreateTooltip((ItemWeapon)item, img.Item2);
						break;
					case ItemType.Armor:
						img.Item2.Tooltip = TooltipHelper.CreateTooltip((ItemArmor)item, img.Item2);
						break;
					case ItemType.Back:
						img.Item2.Tooltip = TooltipHelper.CreateTooltip((ItemBack)item, img.Item2);
						break;
					case ItemType.Trinket:
						img.Item2.Tooltip = TooltipHelper.CreateTooltip((ItemTrinket)item, img.Item2);
						break;
					default:
						img.Item2.Tooltip = TooltipHelper.CreateTooltip(item, img.Item2);
						break;
				}
			}
		} 

		protected override void Unload()
		{
			_loreWindow.Hide();
			base.Unload();
		}
	}
}
