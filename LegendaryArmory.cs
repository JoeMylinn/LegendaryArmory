using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Gw2Sharp.WebApi.V2.Models;
using LegendaryArmory.Helper;
using LegendaryArmory.Services;
using LegendaryArmory.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace LegendaryArmory
{
    [Export(typeof(Module))]
    public class LegendaryArmory : Module
    {

        private static readonly Logger Logger = Logger.GetLogger<LegendaryArmory>();

        private CornerIcon _armoryCornerIcon;
        private StandardWindow _armoryWindow;
        private ArmoryView _armoryView;
        private ArmoryService _armoryService;
        private CancellationTokenSource _cTs;
        private CancellationToken _cT;
        private SettingCollection _internalSettings;
        private SettingEntry<bool> _firstLaunch;

        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

        [ImportingConstructor]
        public LegendaryArmory([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            _internalSettings = settings.AddSubCollection("InternalSettings", false, true);
            _firstLaunch = _internalSettings.DefineSetting("firstLaunch", true);
            
        }

        protected override void Initialize()
        {
            UpdatePermissions();

            _cTs = new CancellationTokenSource();
            _cT = _cTs.Token;

            GameService.Content.DatAssetCache.TryGetTextureFromAssetId(1078536, out AsyncTexture2D chestTexture);

            _armoryCornerIcon = new CornerIcon()
            {
                Icon = chestTexture,
                BasicTooltipText = $"Legendary Armory",
                Priority = 1221451,
                Parent = GameService.Graphics.SpriteScreen
            };

            _armoryWindow = new StandardWindow(
                GameService.Content.DatAssetCache.GetTextureFromAssetId(155985),
                new Rectangle(40, 26, 913, 691),
                new Rectangle(70, 36, 839, 635))

            {
                Parent = GameService.Graphics.SpriteScreen,
                Title = Name,
                Location = new Point(300, 300),
                SavesPosition = true,
                Emblem = ContentsManager.GetTexture("1824203.png"),
                Id = $"{nameof(LegendaryArmory)}_Main_Window"
            };
        }

        protected override async Task LoadAsync()
        {
            _armoryService = new ArmoryService();

            _armoryService.InitLegendaries(Gw2ApiManager.Gw2ApiClient.V2);
            _armoryView = new ArmoryView(_armoryService);
            UpdateAmountsRepeat();
        }

        protected override async void OnModuleLoaded(EventArgs e)
        {
            _armoryCornerIcon.Click += delegate
            {
                _armoryWindow.ToggleWindow(_armoryView);
            };

            Gw2ApiManager.SubtokenUpdated += async delegate
            {
                await Task.Run(() =>
                {
                    _armoryService.UpdateAmounts(Gw2ApiManager, _armoryView);
                    _armoryService.UpdateCharacters(Gw2ApiManager);
                });

            };

            GameService.Gw2Mumble.PlayerCharacter.NameChanged += async delegate
            {
                await Task.Run(() =>
                {
                    _armoryService.UpdateAmounts(Gw2ApiManager, _armoryView);
                });
            };

            await Task.Run(() => { _armoryService.UpdateCharacters(Gw2ApiManager); });

            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private async void UpdateAmountsRepeat()
        {
            await Task.Run(() => { _armoryService.UpdateAmounts(Gw2ApiManager, _armoryView); });
            try
            {
                await Task.Delay(1000 * 60 * 5).ContinueWith(_ =>
                {
                    _cT.ThrowIfCancellationRequested();
                    UpdateAmountsRepeat();
                }, _cT);
            }
            catch (OperationCanceledException) { }
        }

        //Workaround to add new API permissions to the module
        private void UpdatePermissions()
        {
            //Only do this the first time the new update is launched, to avoid overriding changes made by user
            if (_firstLaunch.Value)
            {
                Logger.Info("First launch since update");
                _firstLaunch.Value = false;
                //Deprecated, kept to avoid overriding settings, remove in future release
                if (IOHelper.FirstLaunch())
                {
                    var currentPermissions = GameService.Module.ModuleStates.Value[ModuleParameters.Manifest.Namespace]
                        .UserEnabledPermissions?.ToList();
                    var newPermissions = ModuleParameters.Manifest.ApiPermissions.Keys.ToArray();
                    var updatedPermissions = ModuleParameters.Manifest.ApiPermissions.Keys.ToList()
                        .Except(currentPermissions ?? new List<TokenPermission>());

                    //Skip if no permissions need to be updated
                    if (updatedPermissions.Count() > 0)
                    {
                        Logger.Info("First launch since update, adding { updatedPermissions } API permissions.",
                            string.Join(", ", updatedPermissions.ToArray()));

                        GameService.Module.ModuleStates.Value[ModuleParameters.Manifest.Namespace]
                            .UserEnabledPermissions = newPermissions;
                        GameService.Settings.Save();

                        //Try to renew the subtoken, not necessary but avoids having to restart Blish HUD to get a new subtoken with updated permissions
                        try
                        {
                            var flags = System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance;
                            var RenewSubtoken = Gw2ApiManager.GetType().GetMethod("RenewSubtoken", flags);
                            var _permissions = Gw2ApiManager.GetType().GetField("_permissions", flags);

                            _permissions.SetValue(Gw2ApiManager, newPermissions.ToHashSet());
                            RenewSubtoken.Invoke(Gw2ApiManager, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, "Failed to renew subtoken.");
                        }
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {

        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _cTs.Cancel();
            _cTs.Dispose();
            // Unload here
            _armoryCornerIcon?.Dispose();
            _armoryWindow?.Dispose();
            _armoryView?.DoUnload();
            // All static members must be manually unset
            _armoryService = null;
        }

    }

}
