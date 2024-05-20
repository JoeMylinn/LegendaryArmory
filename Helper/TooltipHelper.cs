using Blish_HUD.Controls;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace LegendaryArmory.Helper
{
	internal class TooltipHelper
	{

		public static Tooltip CreateTooltip(ItemWeapon weapon, LegendaryImage image)
		{
			Tooltip tooltip = new() { };

			var img = Title(weapon, image, tooltip);

			var strength = new Label()
			{
				Parent = tooltip,
				Top = img.Bottom + 5,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = "Weapon Strength: " + weapon.Details.MinPower + " - " + weapon.Details.MaxPower,
			};

			if (weapon.Details.DamageType != "Physical")
			{
				var damageType = new Label()
				{
					Parent = tooltip,
					Top = strength.Top,
					Left = strength.Right,
					AutoSizeHeight = true,
					AutoSizeWidth = true,
					Text = " (" + weapon.Details.DamageType.ToString() + ")",
					Opacity = (float)0.7
				};
			}

			var unlock = Unlock(weapon, image, tooltip, strength);

			var rarity = Rarity(tooltip, unlock);

			var type = new Label()
			{
				Parent = tooltip,
				Top = rarity.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = weapon.Details.Type.ToString(),
			};

			var wieldType = new Label()
			{
				Parent = tooltip,
				Top = type.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = "(" + image.WieldType.First().ToString() + ")",
				Opacity = (float)0.7
			};

			End(weapon, image, tooltip, wieldType);

			return tooltip;
		}

		public static Tooltip CreateTooltip(ItemArmor armor, LegendaryImage image)
		{
			Tooltip tooltip = new() { };

			var img = Title(armor, image, tooltip);

			var defense = new Label()
			{
				Parent = tooltip,
				Top = img.Bottom + 5,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = "Defense: " + armor.Details.Defense,
			};

			var unlock = Unlock(armor, image, tooltip, defense);

			var rarity = Rarity(tooltip, unlock);

			var weightClass = new Label()
			{
				Parent = tooltip,
				Top = rarity.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = armor.Details.WeightClass.ToString(),
			};

			var type = new Label()
			{
				Parent = tooltip,
				Top = weightClass.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = armor.Details.Type.ToString(),
			};

			End(armor, image, tooltip, type);

			return tooltip;
		}

		public static Tooltip CreateTooltip(ItemBack back, LegendaryImage image)
		{
			Tooltip tooltip = new() { };

			var img = Title(back, image, tooltip);

			var placeholder = new Label()
			{
				Parent = tooltip,
				Top = img.Bottom + 5,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
			};


			var unlock = Unlock(back, image, tooltip, placeholder);

			var rarity = Rarity(tooltip, unlock);

			var type = new Label()
			{
				Parent = tooltip,
				Top = rarity.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = back.Type.ToString(),
			};

			End(back, image, tooltip, type);

			return tooltip;
		}

		public static Tooltip CreateTooltip(ItemTrinket trinket, LegendaryImage image)
		{
			Tooltip tooltip = new() { };

			var img = Title(trinket, image, tooltip);

			var placeholder = new Label()
			{
				Parent = tooltip,
				Top = img.Bottom + 5,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
			};

			var rarity = Rarity(tooltip, placeholder);

			var type = new Label()
			{
				Parent = tooltip,
				Top = rarity.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = trinket.Details.Type.ToString(),
			};

			End(trinket, image, tooltip, type);

			return tooltip;
		}

		public static Tooltip CreateTooltip(Item item, LegendaryImage image)
		{
			Tooltip tooltip = new() { };

			var img = Title(item, image, tooltip);

			var placeholder = new Label()
			{
				Parent = tooltip,
				Top = img.Bottom + 5,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
			};

			var rarity = Rarity(tooltip, placeholder);

			var type = new Label()
			{
				Parent = tooltip,
				Top = rarity.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = item.Id switch
				{
					91505 => "Sigil",
					91536 => "Rune",
					101582 => "Relic",
					_ => "Unknown",
				}
			};

			End(item, image, tooltip, type);

			return tooltip;
		}

		private static Image Title(Item item, LegendaryImage image, Tooltip tooltip)
		{
			var img = new Image()
			{
				Parent = tooltip,
				Texture = image.Texture,
				Width = 32,
				Height = 32
			};

			var name = new Label()
			{
				Parent = tooltip,
				Top = tooltip.Top + 5,
				Left = img.Right + 5,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = item.Name,
				TextColor = new Microsoft.Xna.Framework.Color(158, 53, 253),
			};

			return img;
		}

		private static Label Unlock(Item item, LegendaryImage image, Tooltip tooltip, Label last)
		{
			return new Label()
			{
				Parent = tooltip,
				Top = last.Bottom + 10,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = (image.Amount > 0 ? "Skin Unlocked\n" : "Skin Locked\n") + item.Name,
				Opacity = (float)(image.Amount > 0 ? 1 : 0.7),
			};
		}

		private static Label Rarity(Tooltip tooltip, Label last)
		{
			return new Label()
			{
				Parent = tooltip,
				Top = last.Bottom + 10,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = "Legendary",
				TextColor = new Microsoft.Xna.Framework.Color(158, 53, 253),

			};
		}

		private static Label Level(Item item, Tooltip tooltip, Label last)
		{
			return new Label()
			{
				Parent = tooltip,
				Top = last.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = "Required Level: " + item.Level.ToString()
			};
		}

		private static Label Description(Item item, Tooltip tooltip, Label last)
		{
			//Description of Relic in API is useless, ignore if ID matches relic
			if (item.Description != null && item.Id != 101582)
			{
				return new Label()
				{
					Parent = tooltip,
					Top = last.Bottom,
					AutoSizeHeight = true,
					WrapText = true,
					Width = 280,
					Text = Regex.Replace(item.Description, @"\<.*?\>", ""),
					TextColor = new Microsoft.Xna.Framework.Color(159, 247, 230)
				};
			}

			return last;
		}

		private static Label Status(Item item, LegendaryImage image, Tooltip tooltip, Label last)
		{
			return new Label()
			{
				Parent = tooltip,
				Top = last.Bottom,
				AutoSizeHeight = true,
				WrapText = true,
				Width = 280,
				Text = $"You have unlocked {image.Amount} and {(image.Amount == image.MaxAmount ? "cannot unlock any" : $"can unlock {image.MaxAmount - image.Amount}")} more of this item in your Legendary Armory.",
				TextColor = new Microsoft.Xna.Framework.Color(211, 193, 123)
			};
		}

		private static Label Binding(Item item, Tooltip tooltip, Label last)
		{
			return new Label()
			{
				Parent = tooltip,
				Top = last.Bottom,
				AutoSizeHeight = true,
				AutoSizeWidth = true,
				Text = item.Flags.Where(_ => _.ToString().StartsWith("Account")).First().ToString(),
			};
		}

		private static Label End(Item item, LegendaryImage image, Tooltip tooltip, Label last)
		{
			var level = Level(item, tooltip, last);

			var description = Description(item, tooltip, level);

			var status = Status(item, image, tooltip, description);

			return Binding(item, tooltip, status);
		}

	}
}
