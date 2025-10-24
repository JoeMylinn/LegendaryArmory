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
        private static readonly string data_path = DirectoryUtil.BasePath + @"\legendary_armory\";
        private static readonly JsonSerializerSettings settings = new()
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
                Logger.Debug("Saving item data to local storage.");
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
            Logger.Warn("Loading item data from local storage.");
            if (File.Exists(data_path + "legendaries.json"))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<List<LegendaryItem>>(File.ReadAllText(data_path + "legendaries.json"), settings);
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to load item data from local storage.");
                    return result;
                }
            }
            else
            {
                Logger.Warn("No item data found.");
                return result;
            }
        }

        public static bool SaveAccountData(List<AccountLegendaryArmory> unlocked, string account)
        {
            //Make sure dir exists
            Directory.CreateDirectory(data_path + account);
            try
            {
                Logger.Debug("Saving account data to local storage.");
                File.WriteAllText(data_path + account + @"\unlocked.json", JsonConvert.SerializeObject(unlocked, settings));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save account data to local storage.");
                return false;
            }
        }

        public static List<AccountLegendaryArmory> LoadAccountData(string character)
        {
            Logger.Warn("Loading account data from local storage.");
            var result = new List<AccountLegendaryArmory>();
            var account = GetAccountFromCharacter(character);

            try
            {
                if (account != null)
                {
                    if (Directory.Exists(data_path + account))
                    {
                        result = JsonConvert.DeserializeObject<List<AccountLegendaryArmory>>(File.ReadAllText(data_path + account + @"\unlocked.json"), settings);
                    }
                    else
                    {
                        Logger.Warn("No account data for {account} found.", account);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load account data from local storage.");
            }
            return result;
        }

        public static bool SaveCharacterData(string account, List<string> characters)
        {
            //Make sure dir exists
            Directory.CreateDirectory(data_path);
            var characterMap = new Dictionary<string, string>();

            try
            {
                Logger.Debug("Saving character data to local storage.");
                //Load existing data
                if (File.Exists(data_path + "characters.json"))
                {
                    characterMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(data_path + "characters.json"));
                }
                //Update data
                foreach (var character in characters)
                {
                    characterMap[character] = account;
                }

                File.WriteAllText(data_path + "characters.json", JsonConvert.SerializeObject(characterMap, Formatting.Indented));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save character data to local storage.");
                return false;
            }
        }

        private static string GetAccountFromCharacter(string character)
        {
            try
            {
                Logger.Warn("Getting account from current character.");
                if (File.Exists(data_path + "characters.json"))
                {
                    var characters = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(data_path + "characters.json"));
                    if (characters.ContainsKey(character))
                    {
                        return characters[character];
                    }
                    else
                    {
                        Logger.Warn("No Account for Character {character} found.", character);
                        return null;
                    }
                }
                else
                {
                    Logger.Warn("No character data found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                {
                    Logger.Error(ex, "Failed to load character data from local storage.");
                    return null;
                }
            }
        }

        public static bool FirstLaunch() => !Directory.Exists(data_path);
    }
}
