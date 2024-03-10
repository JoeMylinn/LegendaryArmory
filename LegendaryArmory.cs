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
			
		}

		protected override async Task LoadAsync()
		{
			_armoryService = new ArmoryService(Gw2ApiManager);

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
				Id = $"{nameof(LegendaryArmory)}_Main_Window"
			};


			_armoryView = new ArmoryView(_armoryService);

			_armoryCornerIcon.Click += delegate 
			{
				_armoryService.UpdateAmounts(Gw2ApiManager, _armoryView);
				_armoryWindow.ToggleWindow(_armoryView); 
			};

			Gw2ApiManager.SubtokenUpdated += delegate
			{
				_armoryService.UpdateAmounts(Gw2ApiManager, _armoryView);
			};
		}

		protected override void OnModuleLoaded(EventArgs e)
		{

			// Base handler must be called
			base.OnModuleLoaded(e);
		}

		protected override void Update(GameTime gameTime)
		{

		}

		/// <inheritdoc />
		protected override void Unload()
		{
			// Unload here
			_armoryCornerIcon?.Dispose();
			_armoryWindow?.Dispose();
			_armoryService = null;
			_armoryView?.DoUnload();
			// All static members must be manually unset
		}

	}

}
