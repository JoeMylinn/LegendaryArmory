using Blish_HUD;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using File = System.IO.File;

namespace LegendaryArmory.Helper
{
    internal class IOHelper
    {
        private static readonly Logger Logger = Logger.GetLogger<IOHelper>();
        private static string data_path = DirectoryUtil.BasePath + @"\legendary_armory\data\";
        private static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public static bool SaveItemData(List<LegendaryItem> legendaries)
        {
            //Make sure dir exists
            Directory.CreateDirectory(data_path);

            try
            {
                Logger.Warn("Saving item data to local storage.");
                File.WriteAllText(data_path + "legendaries.json", JsonConvert.SerializeObject(legendaries, settings));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save item data to local storage.");
                return false;
            }
        }

        public static List<LegendaryItem> LoadItemData()
        {
            var result = new List<LegendaryItem>();
            try
            {
                Logger.Warn("Loading item data from local storage.");
                result = JsonConvert.DeserializeObject<List<LegendaryItem>>(File.ReadAllText(data_path + "legendaries.json"), settings);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load item data from local storage.");
                return result;
            }
        }

        public static bool SaveAccountData(List<AccountLegendaryArmory> unlocked)
        {
            //Make sure dir exists
            Directory.CreateDirectory(data_path);

            try
            {
                Logger.Warn("Saving account data to local storage.");
                File.WriteAllText(data_path + @"\unlocked.json", JsonConvert.SerializeObject(unlocked, settings));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save account data to local storage.");
                return false;
            }
        }

        public static List<AccountLegendaryArmory> LoadAccountData()
        {
            var result = new List<AccountLegendaryArmory>();
            try
            {
                Logger.Warn("Loading account data from local storage.");
                result = JsonConvert.DeserializeObject<List<AccountLegendaryArmory>>(File.ReadAllText(data_path + "unlocked.json"), settings);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load account data from local storage.");
                return result;
            }
        }
    }
}
