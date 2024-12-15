using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegendaryArmory.Services
{
	internal class ArmoryService : IDisposable
	{
		private static readonly Logger Logger = Logger.GetLogger<ArmoryService>();

		public List<Gw2Sharp.WebApi.V2.Models.LegendaryArmory> legendaryIds = new();
		public List<Item> LegendaryItems = new();
		public List<AccountLegendaryArmory> OwnedLegendaries = new();
		public List<KeyValuePair<String, ProfessionWeapon>> ProfessionWeapons = new();
		public List<Skin> WeaponVariants = new();
		public ArmoryService() 
		{
				
		}

		public void InitLegendaries (IGw2WebApiV2Client apiClient) 
		{
			try
			{
				Logger.Debug("Getting Legendaries from API.");
				legendaryIds = apiClient.LegendaryArmory.AllAsync().Result.ToList();
				LegendaryItems = (List<Item>)apiClient.Items.ManyAsync(legendaryIds.Select(item => item.Id)).Result;
				WeaponVariants = apiClient.Skins.ManyAsync(variantSkinIds).Result.ToList();
				ProfessionWeapons = apiClient.Professions.AllAsync().Result.ToList().SelectMany(p => p.Weapons).ToList();
			}
			catch (Exception ex)
			{
				Logger.Warn(ex, "Failed to get Legendaries.");
			}
			
			LegendaryItems.Sort((a, b) => { return a.Id.CompareTo(b.Id); });


			//TODO: Figure out proper sorting
			/*foreach (Skin variant in variants)
			{
				AddLegendaryVariant((SkinWeapon)variant);
			}*/

			//Workaround for Relics not properly Modeled by GW2Sharp
			var relic = LegendaryItems.Find(i => i.Id == 101582);
			if(relic != null)
			{
				relic.Type = ItemType.UpgradeComponent;
			}
		}

		private readonly List<int> variantSkinIds = new()
		{
			10244,10250,10251,10252,10259,10260,10266,10284,10287,10288,10294,10301,10309,10311,10320,10324,
			10327,10329,10332,10343,10345,10346,10347,10350,10353,10355,10358,10363,10366,10381,10388,10390,
			10394,10410,10414,10415,10417,10422,10429,10436,10437,10448,10449,10453,10472,10480,10481,10490,
			10491,10496,10497,10499,10511,10513,10515,10517,10521,10525,10532,10537,10539,10541,10544,10550,
			10552,10553,10564,10570,10579,10583,10593,10596,10601,10623,10628,10630,10632,10645,10660,10674,
			10676,10678,10685,10693,10696,10697,10711,10712,10716,10720,10729,10730,10738,10739,10747,10748
		};

		private readonly List<int> variantIds = new()
		{
			98091,98098,98099,98104,98109,98116,98118,98120,98138,98142,98143,98144,98146,98158,98160,98162,
			98219,98224,98227,98230,98231,98232,98260,98270,98275,98282,98284,98286,98289,98291,98296,98298,
			98436,98446,98450,98457,98481,98519,98636,98707,98727,98745,98798,98800,98816,98818,98830,98869,
			98999,99000,99003,99006,99011,99017,99019,99023,99025,99029,99030,99031,99032,99033,99041,99045,
			99191,99194,99197,99198,99202,99211,99214,99216,99217,99218,99228,99232,99239,99241,99253,99263,
			99393,99407,99412,99415,99423,99442,99443,99452,99455,99470,99479,99491,99495,99509,99520,99547
		};


		/*private void AddLegendaryVariant(SkinWeapon skin)
		{
			LegendaryItems.Add(new ItemWeapon()
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
		}*/

		public async void UpdateAmounts(Gw2ApiManager apiManager, ArmoryView view)
		{
			try
			{
				if (apiManager.HasPermissions(new[] { TokenPermission.Account, TokenPermission.Inventories, TokenPermission.Unlocks }))
				{
					Logger.Debug("Getting owned Legendaries from API");
					await Task.Run(() =>
					{
						OwnedLegendaries = apiManager.Gw2ApiClient.V2.Account.LegendaryArmory.GetAsync().Result.ToList();
					});
					Logger.Debug("Got {legendaresCount} owned Legendaries from API.", OwnedLegendaries.Count);
				}
				else
				{
					Logger.Warn("Skipping getting owned Legendaries, API Key missing permissions.");
				}
			}
			catch (Exception e)
			{
				Logger.Warn(e, "Failed to update owned Legendaries.");
			}
			view.UpdateAmounts(OwnedLegendaries);
		}

		private readonly List<string> weaponTypeOrder = new()
		{
			"Sword","Hammer","LongBow","ShortBow","Axe","Dagger","Greatsword","Mace","Pistol","Rifle","Scepter","Staff","Focus","Torch","Warhorn","Shield","Harpoon", "Speargun","Trident"
		};

		internal AsyncTexture2D GetIconFromUrl(string url)
		{
			char[] delimiters = { '.', '/' };
			String[] splitUrl = url.Split(delimiters);
			int assetId = int.Parse(splitUrl[splitUrl.Length - 2]);
			return GameService.Content.DatAssetCache.GetTextureFromAssetId(assetId);
		}

		public void Dispose()
		{
			
		}
	}
}
