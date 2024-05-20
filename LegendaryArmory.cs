using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Gw2Sharp.Mumble;
using Gw2Sharp.WebApi.Render;
using LegendaryArmory.Services;
using LegendaryArmory.UI;
using Microsoft.Xna.Framework;
using MonoGame.Extended.VectorDraw;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace LegendaryArmory
{
	[Export(typeof(Blish_HUD.Modules.Module))]
	public class LegendaryArmory : Blish_HUD.Modules.Module
	{

		private static readonly Logger Logger = Logger.GetLogger<Module>();

		private CornerIcon _armoryCornerIcon;
		private StandardWindow _armoryWindow;
		private ArmoryView _armoryView;
		private ArmoryService _armoryService;
		private CancellationTokenSource cTS;
		private CancellationToken cT;

		internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
		internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
		internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
		internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;

		[ImportingConstructor]
		public LegendaryArmory([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

		protected override void DefineSettings(SettingCollection settings)
		{
		}

		protected override void Initialize()
		{
			cTS = new CancellationTokenSource();
			cT = cTS.Token;


			_armoryService = new ArmoryService();

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
				Title = this.Name,
				Location = new Point(300, 300),
				SavesPosition = true,
				Emblem = ContentsManager.GetTexture("1824203.png"),
				Id = $"{nameof(LegendaryArmory)}_Main_Window"
			};
		}

		protected override async Task LoadAsync()
		{
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
				await Task.Run(() => { _armoryService.UpdateAmounts(Gw2ApiManager, _armoryView); });
			};


			// Base handler must be called
			base.OnModuleLoaded(e);
		}

		private async void UpdateAmountsRepeat()
		{
			await Task.Run(() => { _armoryService.UpdateAmounts(Gw2ApiManager, _armoryView); });
			try
			{
				await Task.Delay(1000 * 60 * 5).ContinueWith(t =>
				{
					cT.ThrowIfCancellationRequested();
					UpdateAmountsRepeat();
				}, cT);
			}
			catch (OperationCanceledException ex) { }
		}

		protected override void Update(GameTime gameTime)
		{

		}

		/// <inheritdoc />
		protected override void Unload()
		{
			cTS.Cancel();
			cTS.Dispose();
			// Unload here
			_armoryCornerIcon?.Dispose();
			_armoryWindow?.Dispose();
			_armoryView?.DoUnload();
			// All static members must be manually unset
			_armoryService = null;
		}

	}

}
