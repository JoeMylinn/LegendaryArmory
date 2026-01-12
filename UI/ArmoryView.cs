using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Gw2Sharp;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Helper;
using LegendaryArmory.Models;
using LegendaryArmory.Services;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace LegendaryArmory.UI
{
    internal class ArmoryView : View
    {

        private ArmoryService armoryService;
        private LoreWindow _loreWindow;
        private List<string> twoHanded = new() { "Hammer", "LongBow", "ShortBow", "Greatsword", "Rifle", "Staff", "Speargun", "Harpoon", "Trident" };
        private List<string> oneHanded = new() { "Sword", "Axe", "Dagger", "Mace", "Pistol", "Scepter" };
        private List<string> offHand = new() { "Focus", "Warhorn", "Torch", "Shield" };
        private List<string> trinkets = new() { "Accessory", "Ring", "Amulet" };
        private List<string> slotOrder = new() { "HelmAquatic", "Helm", "Shoulders", "Coat", "Gloves", "Leggings", "Boots" };

        public List<(int, LegendaryImage)> LegendaryImages = new();

        public ArmoryView(ArmoryService armoryService)
        {
            this.armoryService = armoryService;
            _loreWindow = new LoreWindow(this);

            foreach (var item in armoryService.legendaries)
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

            var allWeaponsMenuItem = weaponMenu.AddMenuItem(Strings.UI.allTypes);
            foreach (var category in LegendaryImages.Where(t => t.Item2.Item.Type == ItemType.Weapon).SelectMany(t => ((LegendaryWeapon)t.Item2.Item).WieldType).Distinct().OrderBy(t => "TMO".IndexOf(t.ToString()[0])))
            {
                if (category == ProfessionWeaponFlag.Aquatic) continue;
                var subMenu = weaponMenu.AddMenuItem(Strings.UI.ResourceManager.GetString(category.ToString()));
                subMenu.Click += delegate
                {
                    DeselctAll(armorMenu);
                    DeselctAll(otherMenu);
                    itemView.Filter(i => i.Item.Type == ItemType.Weapon && ((LegendaryWeapon)i.Item).WieldType.Contains(category));
                };
            }

            var allArmorsMenuItem = armorMenu.AddMenuItem(Strings.UI.allWeights);
            foreach (var category in LegendaryImages.Where(t => t.Item2.Item.Type == ItemType.Armor).Select(t => ((LegendaryArmor)t.Item2.Item).WeightClass).Distinct().ToList())
            {
                var subMenu = armorMenu.AddMenuItem(Strings.UI.ResourceManager.GetString(category.ToString()));
                subMenu.Click += delegate
                {
                    DeselctAll(weaponMenu);
                    DeselctAll(otherMenu);
                    itemView.Filter(item => item.Item.Type == ItemType.Armor && ((LegendaryArmor)item.Item).WeightClass == category);
                };
            }

            var allOtherMenuItem = otherMenu.AddMenuItem(Strings.UI.allTypes);
            foreach (var category in LegendaryImages.Where(t => t.Item2.Item.Type != ItemType.Armor && t.Item2.Item.Type != ItemType.Weapon).Select(t => t.Item2.Item.Type).Distinct().ToList())
            {
                var subMenu = otherMenu.AddMenuItem(Strings.UI.ResourceManager.GetString(category.ToString()));
                subMenu.Click += delegate
                {
                    DeselctAll(weaponMenu);
                    DeselctAll(armorMenu);
                    itemView.Filter(item => item.Item.Type == category);
                };
            }



            allWeaponsMenuItem.Click += delegate
            {
                DeselctAll(armorMenu);
                DeselctAll(otherMenu);
                itemView.Filter(item => item.Item.Type == ItemType.Weapon);
            };

            allArmorsMenuItem.Click += delegate
            {
                DeselctAll(weaponMenu);
                DeselctAll(otherMenu);
                itemView.Filter(item => item.Item.Type == ItemType.Armor);
            };

            allOtherMenuItem.Click += delegate
            {
                DeselctAll(weaponMenu);
                DeselctAll(armorMenu);
                itemView.Filter(item => item.Item.Type != ItemType.Armor && item.Item.Type != ItemType.Weapon);
            };

            allWeaponsMenuItem.Select();
            itemView.Filter(item => item.Item.Type == ItemType.Weapon);
            base.Build(buildPanel);
        }

        private LegendaryImage ImageFromItem(LegendaryItem item)
        {
            LegendaryImage image = new()
            {
                Item = item,
                Texture = armoryService.GetIconFromUrl(item.Icon),
                Width = 64,
                Height = 64,
                Opacity = (float)0.3,
            };

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
            var weapons = LegendaryImages.Where(t => t.Item2.Item.Type == ItemType.Weapon).Select(t => t.Item2).ToList();
            foreach (var gen in weapons.Select(i => ((LegendaryWeapon)i.Item).Generation).Distinct().ToList())
            {
                var categoryPanel = new FlowPanel()
                {
                    Title = Strings.UI.generation + " " + gen.ToString(),
                    Parent = buildPanel,
                    Width = buildPanel.ContentRegion.Width - 24,
                    HeightSizingMode = SizingMode.AutoSize,
                    CanCollapse = true,
                    Collapsed = false,
                    OuterControlPadding = new Vector2(5, 5),
                    ControlPadding = new Vector2(5, 5)
                };
                foreach (var weapon in weapons.Where(i => ((LegendaryWeapon)i.Item).Generation == gen))
                {
                    weapon.Parent = categoryPanel;
                }
            }

            //Armor Categories
            var armors = LegendaryImages.Where(t => t.Item2.Item.Type == ItemType.Armor).Select(t => t.Item2).ToList();
            //Custom sort order, improve, maybe save layout in json?
            foreach (var slot in armors.Select(i => ((LegendaryArmor)i.Item).Slot).Distinct().ToList().OrderBy(t => slotOrder.IndexOf(t.ToString())))
            {
                var categoryPanel = new FlowPanel()
                {
                    Title = Strings.UI.ResourceManager.GetString(slot.ToString()),
                    Parent = buildPanel,
                    Width = buildPanel.ContentRegion.Width - 24,
                    HeightSizingMode = SizingMode.AutoSize,
                    CanCollapse = true,
                    Collapsed = false,
                    OuterControlPadding = new Vector2(5, 5),
                    ControlPadding = new Vector2(5, 5)
                };
                foreach (var armor in armors.Where(i => ((LegendaryArmor)i.Item).Slot == slot))
                {
                    armor.Parent = categoryPanel;
                }
            }

            //Misc Categories
            var misc = LegendaryImages.Where(t => t.Item2.Item.Type != ItemType.Weapon && t.Item2.Item.Type != ItemType.Armor).Select(t => t.Item2).ToList();
            foreach (var category in misc.Select(i => i.Item.Type).Distinct().ToList())
            {
                var categoryPanel = new FlowPanel()
                {
                    Title = Strings.UI.ResourceManager.GetString(category.ToString()),
                    Parent = buildPanel,
                    Width = buildPanel.ContentRegion.Width - 24,
                    HeightSizingMode = SizingMode.AutoSize,
                    CanCollapse = true,
                    Collapsed = false,
                    OuterControlPadding = new Vector2(5, 5),
                    ControlPadding = new Vector2(5, 5)
                };
                foreach (var item in misc.Where(i => i.Item.Type == category))
                {
                    item.Parent = categoryPanel;
                }
            }
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

            foreach (var img in LegendaryImages)
            {
                var current = owned.Where(i => i.Id == img.Item2.Item.Id);
                var amount = 0;
                if (current.Count() != 0)
                {
                    amount = current.First().Count;
                }

                if (img.Item2.Item.Amount != amount)
                {
                    img.Item2.Item.Amount = amount;
                    updated.Add(img.Item1);
                }
            }

            foreach (var img in LegendaryImages.Where(t => t.Item2.Tooltip == null))
            {
                updated.Add(img.Item1);
            }

            updated.Distinct();

            foreach (var img in LegendaryImages.Where(i => updated.Contains(i.Item1)))
            {
                img.Item2.UpdateOpacity();
                var item = armoryService.legendaries.First(i => i.Id == img.Item1);

                img.Item2.Tooltip = item.Type switch
                {
                    ItemType.Weapon => TooltipHelper.CreateTooltip((LegendaryWeapon)item, img.Item2),
                    ItemType.Armor => TooltipHelper.CreateTooltip((LegendaryArmor)item, img.Item2),
                    ItemType.Trinket => TooltipHelper.CreateTooltip((LegendaryTrinket)item, img.Item2),
                    ItemType.UpgradeComponent => TooltipHelper.CreateTooltip((LegendaryUpgrade)item, img.Item2),
                    _ => TooltipHelper.CreateTooltip(item, img.Item2),
                };
            }
        }

        protected override void Unload()
        {
            _loreWindow.Hide();
            base.Unload();
        }
    }
}
