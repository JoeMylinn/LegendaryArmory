using Blish_HUD.Controls;
using LegendaryArmory.Models;
using LegendaryArmory.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace LegendaryArmory.Helper
{
    internal class TooltipHelper
    {
        public static Tooltip CreateTooltip(LegendaryWeapon weapon, LegendaryImage image)
        {
            Tooltip tooltip = new();

            var img = Title(weapon, image, tooltip);

            var strength = new Label()
            {
                Parent = tooltip,
                Top = img.Bottom + 5,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Text = "Weapon Strength: " + weapon.MinPower + " - " + weapon.MaxPower,
            };

            if (weapon.DamageType != "Physical")
            {
                _ = new Label()
                {
                    Parent = tooltip,
                    Top = strength.Top,
                    Left = strength.Right,
                    AutoSizeHeight = true,
                    AutoSizeWidth = true,
                    Text = " (" + weapon.DamageType + ")",
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
                Text = weapon.WeaponType,
            };

            var wieldType = new Label()
            {
                Parent = tooltip,
                Top = type.Bottom,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Text = "(" + weapon.WieldType.First().ToString() + ")",
                Opacity = (float)0.7
            };

            End(weapon, tooltip, wieldType);

            return tooltip;
        }

        public static Tooltip CreateTooltip(LegendaryArmor armor, LegendaryImage image)
        {
            Tooltip tooltip = new();

            var img = Title(armor, image, tooltip);

            var defense = new Label()
            {
                Parent = tooltip,
                Top = img.Bottom + 5,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Text = "Defense: " + armor.Defense,
            };

            var unlock = Unlock(armor, image, tooltip, defense);

            var rarity = Rarity(tooltip, unlock);

            var weightClass = new Label()
            {
                Parent = tooltip,
                Top = rarity.Bottom,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Text = armor.WeightClass.ToString(),
            };

            var type = new Label()
            {
                Parent = tooltip,
                Top = weightClass.Bottom,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Text = armor.Slot.ToString(),
            };

            End(armor, tooltip, type);

            return tooltip;
        }

        public static Tooltip CreateTooltip(LegendaryTrinket trinket, LegendaryImage image)
        {
            Tooltip tooltip = new();

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
                Text = trinket.TrinketType,
            };

            End(trinket, tooltip, type);

            return tooltip;
        }

        public static Tooltip CreateTooltip(LegendaryUpgrade upgrade, LegendaryImage image)
        {
            Tooltip tooltip = new();

            var img = Title(upgrade, image, tooltip);

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
                Text = upgrade.UpgradeType
            };

            End(upgrade, tooltip, type);

            return tooltip;
        }

        public static Tooltip CreateTooltip(LegendaryItem item, LegendaryImage image)
        {
            Tooltip tooltip = new();

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
            };

            End(item, tooltip, type);

            return tooltip;
        }

        private static Image Title(LegendaryItem item, LegendaryImage image, Tooltip tooltip)
        {
            var img = new Image()
            {
                Parent = tooltip,
                Texture = image.Texture,
                Width = 32,
                Height = 32
            };

            _ = new Label()
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

        private static Label Unlock(LegendaryItem item, LegendaryImage image, Tooltip tooltip, Label last)
        {
            return new Label()
            {
                Parent = tooltip,
                Top = last.Bottom + 10,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Text = (image.Item.Amount > 0 ? "Skin Unlocked\n" : "Skin Locked\n") + item.Name,
                Opacity = (float)(image.Item.Amount > 0 ? 1 : 0.7),
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

        private static Label Level(LegendaryItem item, Tooltip tooltip, Label last)
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

        private static Label Description(LegendaryItem item, Tooltip tooltip, Label last)
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

        private static Label Status(LegendaryItem item, Tooltip tooltip, Label last)
        {
            return new Label()
            {
                Parent = tooltip,
                Top = last.Bottom,
                AutoSizeHeight = true,
                WrapText = true,
                Width = 280,
                Text = $"You have unlocked {item.Amount} and {(item.Amount == item.MaxAmount ? "cannot unlock any" : $"can unlock {item.MaxAmount - item.Amount}")} more of this item in your Legendary Armory.",
                TextColor = new Microsoft.Xna.Framework.Color(211, 193, 123)
            };
        }

        private static void Binding(LegendaryItem item, Tooltip tooltip, Label last)
        {
            _ = new Label()
            {
                Parent = tooltip,
                Top = last.Bottom,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Text = item.Flags.First(flag => flag.ToString().StartsWith("Account")),
            };
        }

        private static void End(LegendaryItem item, Tooltip tooltip, Label last)
        {
            var level = Level(item, tooltip, last);

            var description = Description(item, tooltip, level);

            var status = Status(item, tooltip, description);

            Binding(item, tooltip, status);
        }

    }
}
