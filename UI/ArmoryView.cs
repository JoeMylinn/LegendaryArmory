using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Models;
using LegendaryArmory.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SharpDX.MediaFoundation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.UI
{
	internal class ArmoryView : View {

		private ArmoryService armoryService;
		private List<string> twoHanded = new List<string> { "Hammer", "LongBow", "ShortBow", "Greatsword", "Rifle", "Staff", "Speargun", "Harpoon", "Trident" };
		private List<string> oneHanded = new List<string>{ "Sword", "Axe", "Dagger", "Mace", "Pistol", "Scepter" };
		private List<string> offHand = new List<string>{ "Focus", "Warhorn", "Torch", "Shield" };
		private List<string> trinkets = new List<string> { "Accessory", "Ring", "Amulet" };

		public ArmoryView(ArmoryService armoryService)
		{
			this.armoryService = armoryService;
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
			};

			var allWeaponsMenuItem = new MenuItem
			{
				Parent = weaponMenu,
				Text = "All Types"
			};

			var twoHandedMenuItem = new MenuItem
			{
				Parent = weaponMenu,
				Text = "Two-Handed"
			};

			var oneHandedMenuItem = new MenuItem
			{
				Parent = weaponMenu,
				Text = "One-Handed"
			};

			var offHandMenuItem = new MenuItem
			{
				Parent = weaponMenu,
				Text = "Off Hand"
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
			};

			var allArmorsMenuItem = new MenuItem
			{
				Parent = armorMenu,
				Text = "All Weights"
			};

			var lightArmorMenuItem = new MenuItem
			{
				Parent = armorMenu,
				Text = "Light"
			};

			var mediumArmorMenuItem = new MenuItem
			{
				Parent = armorMenu,
				Text = "Medium"
			};

			var heavyArmorMenuItem = new MenuItem
			{
				Parent = armorMenu,
				Text = "Heavy"
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
			};

			var allOtherMenuItem = new MenuItem
			{
				Parent = otherMenu,
				Text = "All Types"
			};

			var backpacksMenuItem = new MenuItem
			{
				Parent = otherMenu,
				Text = "Backpacks"
			};

			var trinketsMenuItem = new MenuItem
			{
				Parent = otherMenu,
				Text = "Accessories"
			};

			var upgradesMenuItem = new MenuItem
			{
				Parent = otherMenu,
				Text = "Upgrades"
			};

			var armoryView = new ViewContainer
			{
				Parent = buildPanel,
				Left = menu.Right,
				Width = buildPanel.ContentRegion.Width - menu.Width,
				Height = buildPanel.ContentRegion.Height,
				ShowBorder = true
			};

			allWeaponsMenuItem.Click += (sender, e) => armoryView.Show(new WeaponView(armoryService.GetWeapons()));
			twoHandedMenuItem.Click += (sender, e) => armoryView.Show(new WeaponView(armoryService.GetWeapons().Where(_ => _.Item2.WieldType == Gw2Sharp.ProfessionWeaponFlag.TwoHand).ToList()));
			oneHandedMenuItem.Click += (sender, e) => armoryView.Show(new WeaponView(armoryService.GetWeapons().Where(_ => _.Item2.WieldType == Gw2Sharp.ProfessionWeaponFlag.Mainhand).ToList()));
			offHandMenuItem.Click += (sender, e) => armoryView.Show(new WeaponView(armoryService.GetWeapons().Where(_ => _.Item2.WieldType == Gw2Sharp.ProfessionWeaponFlag.Offhand).ToList()));
			allArmorsMenuItem.Click += (sender, e) => armoryView.Show(new ArmorView(armoryService.GetArmors()));
			lightArmorMenuItem.Click += (sender, e) => armoryView.Show(new ArmorView(armoryService.GetArmors().Where(_ => _.Item2.WeightType == ItemWeightType.Light).ToList()));
			mediumArmorMenuItem.Click += (sender, e) => armoryView.Show(new ArmorView(armoryService.GetArmors().Where(_ => _.Item2.WeightType == ItemWeightType.Medium).ToList()));
			heavyArmorMenuItem.Click += (sender, e) => armoryView.Show(new ArmorView(armoryService.GetArmors().Where(_ => _.Item2.WeightType == ItemWeightType.Heavy).ToList()));
			allOtherMenuItem.Click += (sender, e) => armoryView.Show(new MiscView(armoryService.GetOthers()));
			backpacksMenuItem.Click += (sender, e) => armoryView.Show(new MiscView(armoryService.GetOthers(true).Where(_ => _.Item2.SubType == SubType.Back).ToList()));
			trinketsMenuItem.Click += (sender, e) => armoryView.Show(new MiscView(armoryService.GetOthers(true).Where(_ => _.Item2.Type == ItemType.Trinket).ToList()));
			upgradesMenuItem.Click += (sender, e) => armoryView.Show(new MiscView(armoryService.GetOthers(true).Where(_ => _.Item2.Type == ItemType.UpgradeComponent).ToList()));

			base.Build(buildPanel);
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
