using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using System;
using System.ComponentModel.Design;
using System.Linq;
using MonoGame.Extended.TextureAtlases;


namespace LegendaryArmory.UI
{
	internal class LoreWindow : Container
	{
		private readonly ArmoryView _armory;
		private LegendaryImage _current;
		private String _name = "";
		private Image _item = new();
		private Label _lore = new();

		private readonly AsyncTexture2D _windowBackground = AsyncTexture2D.FromAssetId(921097);
		private readonly AsyncTexture2D _border = AsyncTexture2D.FromAssetId(921096);
		private readonly AsyncTexture2D _banner = AsyncTexture2D.FromAssetId(2468319);
		private readonly AsyncTexture2D _book = AsyncTexture2D.FromAssetId(2468318);
		private readonly AsyncTexture2D _aurene = AsyncTexture2D.FromAssetId(2420385);
		private readonly AsyncTexture2D _textBox = AsyncTexture2D.FromAssetId(1701859);

		public LoreWindow(ArmoryView armory)
		{
			_armory = armory;

			_children = new ControlCollection<Control>();

			Parent = GameService.Graphics.SpriteScreen;
			_zIndex = 100;
			Visible = false;
			Location = new Point(1172, 575);
			Width = 1135;
			Height = 800;

			//init Item Image
			_item.Parent = this;
			_item.Size = new Point(72, 72);
			_item.Location = new Point(347, 508);

			

			GameService.Graphics.SpriteScreen.Resized += OnSpriteScreenResized;

			var close = new StandardButton()
			{
				Text = "Close",
				Parent = this
			};

			_lore.Parent = this;
			_lore.Location = new Point((Width - 220) / 2, 500);
			_lore.Width = 350;
			_lore.Height = 110;
			_lore.WrapText = true;
			_lore.VerticalAlignment = VerticalAlignment.Top;

			var hint = new Label()
			{
				Parent = this,
				Text = Strings.UI.ArmoryHint,
				Location = new Point(347, 620),
				Width = 441,
				Height = 50,
				VerticalAlignment = VerticalAlignment.Top,
				WrapText =	true
			};

			close.Location = new Point((Width - close.Width)/ 2, 706);

			close.Click += delegate
			{
				Hide();
			};

			
		}

		public void Show(int id)
		{
			var item = _armory.LegendaryImages.Find(i => i.Item1 == id).Item2;
			if (Visible || item.Amount < 1) return;
			_current = item;
			_item.Texture = _current.Texture;
			_item.Tooltip = _current.Tooltip;
			_lore.Text = Strings.Lore.ResourceManager.GetString(":" + id.ToString());
			_name = _current.Tooltip.GetChildrenOfType<Label>().First().Text;

			Show();
		}

		private void OnSpriteScreenResized(object sender, ResizedEventArgs resizedEventArgs)
		{
			Location = new Point(((GameService.Graphics.SpriteScreen.Size.X - Width) / 2) - 1,
				((GameService.Graphics.SpriteScreen.Size.Y - Height) / 2) - 4);
		}

		protected virtual void PaintWindowBackground(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawOnCtrl(this,
				_windowBackground,
				new Rectangle(205, 62, 726, 726));
		}

		private void PaintBorder(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawOnCtrl(this,
				_border,
				new Rectangle(227, 72, 684, 684));
		}

		private void PaintBanner(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawOnCtrl(this,
				_banner,
				new Rectangle(0, 0, 1135, 143));
		}

		private void PaintTitle (SpriteBatch spriteBatch)
		{
			spriteBatch.DrawStringOnCtrl(this,
				"Congratulations!",
				Content.DefaultFont32,
				new Rectangle((Width - 500) / 2, 17, 500, 100),
				Color.White,
				horizontalAlignment: HorizontalAlignment.Center
				);
		}

		private void PaintSplash(SpriteBatch spriteBatch)
		{
			if (_current != null)
			{
				spriteBatch.DrawOnCtrl(this,
					_current?.Generation == 3 ? _book : _book,//draw same for both until proper Aurene splash screen found
					new Rectangle(137, 132, 864, 324));
			}
		}

		private void PaintTextBox(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawOnCtrl(this,
				_textBox,
				new Rectangle(237, 484, 664, 203));
		}

		private void PaintItemName(SpriteBatch spriteBatch)
		{
			//Opacity = 0.5f;
			spriteBatch.DrawStringOnCtrl(this,
				_name,
				Content.DefaultFont32,
				new Rectangle((Width - 500)/2, 406, 500, 100),
				new Color(218, 149, 212),
				horizontalAlignment:HorizontalAlignment.Center
				);
		}

		private void PaintItemDescription(SpriteBatch spriteBatch)
		{
			var test = Strings.Lore._30689;
		}

		public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
		{
			PaintWindowBackground(spriteBatch);
			PaintBorder(spriteBatch);
			PaintBanner(spriteBatch);
			PaintTitle(spriteBatch);
			PaintSplash(spriteBatch);
			PaintTextBox(spriteBatch);
			PaintItemName(spriteBatch);
		}
	}
}
