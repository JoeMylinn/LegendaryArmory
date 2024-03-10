using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LegendaryArmory.Services
{
	internal class ArmoryService : IDisposable
	{
		private static Logger Logger = Logger.GetLogger<ArmoryService>();

		public List<WeaponItem> weapons = new List<WeaponItem>();
		public List<ArmorItem> armors = new List<ArmorItem>();
		public List<OtherItem> other = new List<OtherItem>();
		public ArmoryService(Gw2ApiManager apiManager) 
		{
				InitLegendaries(apiManager.Gw2ApiClient.V2);	
		}

		private void InitLegendaries (IGw2WebApiV2Client apiClient) 
		{
			List<Item> legendaries = new List<Item>();
			List<Skin> variants = new List<Skin>();
			List<Gw2Sharp.WebApi.V2.Models.LegendaryArmory> legendaryIds = new List<Gw2Sharp.WebApi.V2.Models.LegendaryArmory>();
			try
			{
				Logger.Debug("Getting Legendaries from API.");
				legendaryIds = apiClient.LegendaryArmory.AllAsync().Result.ToList();
				legendaries = (List<Item>)apiClient.Items.ManyAsync(legendaryIds.Select(item => item.Id)).Result;
				variants = apiClient.Skins.ManyAsync(variantSkinIds).Result.ToList();
			} catch (Exception ex)
			{
				Logger.Warn(ex, "Failed to get Legendaries.");
			}

			foreach(Item legendary in legendaries)
			{
				switch (legendary.Type.ToEnum())
				{
					case ItemType.Weapon:
						ItemWeapon weapon = (ItemWeapon)legendary;
						AddLegendaryWeapon(weapon, legendaryIds.Find(item => item.Id == legendary.Id).MaxCount);
						break;
					case ItemType.Armor:
						ItemArmor armor = (ItemArmor)legendary;
						AddLegendaryArmor(armor, legendaryIds.Find(item => item.Id == legendary.Id).MaxCount);
						break;
					case ItemType.Trinket:
						ItemTrinket trinket = (ItemTrinket)legendary;
						AddLegendaryTrinket(trinket, legendaryIds.Find(item => item.Id == legendary.Id).MaxCount);
						break;
					case ItemType.Back:
						ItemBack back = (ItemBack)legendary;
						AddLegendaryBack(back, legendaryIds.Find(item => item.Id == legendary.Id).MaxCount);
						break;
					case ItemType.UpgradeComponent:
						ItemUpgradeComponent upgrade = (ItemUpgradeComponent)legendary;
						AddLegendaryUpgrade(upgrade, legendaryIds.Find(item => item.Id == legendary.Id).MaxCount);
						break;
					default:
						break;
				}
			}
			weapons.Sort((a, b) => { return a.Id.CompareTo(b.Id); });
			armors.Sort((a, b) => { return a.Id.CompareTo(b.Id); });
			other.Sort((a, b) => { return a.Id.CompareTo(b.Id); });

			//TODO: Figure out proper sorting
			/*foreach (Skin variant in variants)
			{
				AddLegendaryVariant((SkinWeapon)variant);
			}
*/
		}

		private void AddLegendaryWeapon(ItemWeapon item, int maxAmount)
		{
			weapons.Add(new WeaponItem()
			{
				Id = item.Id,
				Name = item.Name,
				IconId = GetIconIDFromUrl(item.Icon),
				Description = item.Description,
				ChatLink = item.ChatLink,
				Type = item.Type,
				Amount = 0,
				MaxAmount = maxAmount,
				WeaponType = item.Details.Type,
				Generation = item.Id switch 
				{
					<=30704 => 1,
					>30704 and <= 90551 => 2,
					>90551 and <= 97783 => 3,
					_ => 0,
				}
			});
		}

		private void AddLegendaryArmor(ItemArmor item, int maxAmount)
		{
			armors.Add(new ArmorItem()
			{
				Id = item.Id,
				Name = item.Name,
				IconId = GetIconIDFromUrl(item.Icon),
				Description = item.Description,
				ChatLink = item.ChatLink,
				Type = item.Type,
				Amount = 0,
				MaxAmount = maxAmount,
				WeightType = item.Details.WeightClass,
				SlotType = item.Details.Type
			});
		}

		private void AddLegendaryTrinket(ItemTrinket item, int maxAmount)
		{
			other.Add(new OtherItem()
			{
				Id = item.Id,
				Name = item.Name,
				IconId = GetIconIDFromUrl(item.Icon),
				Description = item.Description,
				ChatLink = item.ChatLink,
				Type = item.Type,
				Amount = 0,
				MaxAmount = maxAmount,
				SubType = item.Details.Type.Value switch
				{
					ItemTrinketType.Accessory => SubType.Accessory,
					ItemTrinketType.Amulet => SubType.Amulet,
					ItemTrinketType.Ring => SubType.Ring,
					_ => SubType.Unknown
				}
			});
		}

		private void AddLegendaryBack(ItemBack item, int maxAmount)
		{
			other.Add(new OtherItem()
			{
				Id = item.Id,
				Name = item.Name,
				IconId = GetIconIDFromUrl(item.Icon),
				Description = item.Description,
				ChatLink = item.ChatLink,
				Type = item.Type,
				Amount = 0,
				MaxAmount = maxAmount,
				SubType = SubType.Back
			});
		}

		private void AddLegendaryUpgrade(ItemUpgradeComponent item, int maxAmount)
		{
			other.Add(new OtherItem()
			{
				Id = item.Id,
				Name = item.Name,
				IconId = GetIconIDFromUrl(item.Icon),
				Description = item.Description,
				ChatLink = item.ChatLink,
				Type = item.Type,
				Amount = 0,
				MaxAmount = maxAmount,
				SubType = item.Details.Type.Value switch { ItemUpgradeComponentType.Sigil => SubType.Sigil, ItemUpgradeComponentType.Rune => SubType.Rune, _ => SubType.Unknown }
			});
		}

		private List<int> variantSkinIds = new List<int>
		{
			10244,10250,10251,10252,10259,10260,10266,10284,10287,10288,10294,10301,10309,10311,10320,10324,
			10327,10329,10332,10343,10345,10346,10347,10350,10353,10355,10358,10363,10366,10381,10388,10390,
			10394,10410,10414,10415,10417,10422,10429,10436,10437,10448,10449,10453,10472,10480,10481,10490,
			10491,10496,10497,10499,10511,10513,10515,10517,10521,10525,10532,10537,10539,10541,10544,10550,
			10552,10553,10564,10570,10579,10583,10593,10596,10601,10623,10628,10630,10632,10645,10660,10674,
			10676,10678,10685,10693,10696,10697,10711,10712,10716,10720,10729,10730,10738,10739,10747,10748
		};

		private List<int> variantIds = new List<int>
		{
			98091,98098,98099,98104,98109,98116,98118,98120,98138,98142,98143,98144,98146,98158,98160,98162,
			98219,98224,98227,98230,98231,98232,98260,98270,98275,98282,98284,98286,98289,98291,98296,98298,
			98436,98446,98450,98457,98481,98519,98636,98707,98727,98745,98798,98800,98816,98818,98830,98869,
			98999,99000,99003,99006,99011,99017,99019,99023,99025,99029,99030,99031,99032,99033,99041,99045,
			99191,99194,99197,99198,99202,99211,99214,99216,99217,99218,99228,99232,99239,99241,99253,99263,
			99393,99407,99412,99415,99423,99442,99443,99452,99455,99470,99479,99491,99495,99509,99520,99547
		};


		private void AddLegendaryVariant(SkinWeapon skin)
		{
			weapons.Add(new WeaponItem()
			{
				Id = skin.Id,
				Name = skin.Name,
				IconId = GetIconIDFromUrl(skin.Icon),
				Description = skin.Description,
				ChatLink = "",
				Type = ItemType.Weapon,
				Amount = 0,
				MaxAmount = 1,
				WeaponType = skin.Details.Type,
				Generation = 3,
				Variant = true
			});
		}

		public void UpdateAmounts(Gw2ApiManager apiManager)
		{
			try
			{
				if (apiManager.HasPermissions(new[] { TokenPermission.Account, TokenPermission.Inventories, TokenPermission.Unlocks }))
				{
					Logger.Debug("Getting owned Legendaries from API");
					List<AccountLegendaryArmory> owned = (List<AccountLegendaryArmory>)apiManager.Gw2ApiClient.V2.Account.LegendaryArmory.GetAsync().Result;
					foreach (var current in owned)
					{
						foreach (var item in weapons.Where(_ => _.Id == current.Id))
						{
							item.Amount = current.Count;
						}
						foreach (var item in armors.Where(_ => _.Id == current.Id))
						{
							item.Amount = current.Count;
						}
						foreach (var item in other.Where(_ => _.Id == current.Id))
						{
							item.Amount = current.Count;
						}
					}
					Logger.Debug("Got {legendaresCount} owned Legendaries from API.", owned.Count);
				}
				else
				{
					Logger.Warn("Skipping getting owned Legendaries, API Key missing permissions.");
				}
			}
			catch (Exception e) {
				Logger.Warn(e, "Failed to update owned Legendaries.");	
			}
		}

		private List<string> weaponTypeOrder = new List<string>
		{
			"Sword","Hammer","LongBow","ShortBow","Axe","Dagger","Greatsword","Mace","Pistol","Rifle","Scepter","Staff","Focus","Torch","Warhorn","Shield","Harpoon", "Speargun","Trident"
		};

		public List<(string, WeaponItem)> GetWeapons()
		{
			List <(string, WeaponItem)> result = new List<(string, WeaponItem)>();
			foreach (var item in weapons)
			{
				result.Add((item.Generation.ToString(), item));
			}
			return result.OrderBy(_ => weaponTypeOrder.IndexOf(_.Item1)).ToList();
		}

		public List<(string, ArmorItem)> GetArmors()
		{
			List<(string, ArmorItem)> result = new List<(string, ArmorItem)>();
			foreach (var item in armors)
			{
				result.Add((item.SlotType.ToString(), item));
			}
			return result;
		}

		public List<(string, OtherItem)> GetOthers(bool detailed = false)
		{
			List<(string, OtherItem)> result = new List<(string, OtherItem)>();
			foreach (var item in other)
			{
				if (detailed)
				{
					result.Add((item.SubType.ToString(), item));
				}
				else
				{
					result.Add((item.Type.ToString(), item));
				}
			}
			return result;
		}

		private int GetIconIDFromUrl(string url)
		{
			char[] delimiters = { '.', '/' };
			String[] splitUrl = url.Split(delimiters);
			int assetId = int.Parse(splitUrl[splitUrl.Length - 2]);
			return assetId;
		}
		public void Dispose()
		{
			
		}
	}
}
