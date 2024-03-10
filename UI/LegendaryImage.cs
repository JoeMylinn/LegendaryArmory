using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryArmory.UI
{
	internal class LegendaryImage : Image
	{
		private int _amount;
		public int Amount
		{
			get => _amount;
			set => SetProperty(ref _amount, value);
		}

		private BitmapFont _font = GameService.Content.DefaultFont16;

		public LegendaryImage() : base()
		{
			
		}

		protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
		{
			base.Paint(spriteBatch, bounds);
			DrawBorder(spriteBatch, bounds);
		}

		private void DrawBorder(SpriteBatch spriteBatch, Rectangle bounds)
		{
			//Values for Border
			int lineWidth = 2;
			Color color = new Color(136, 79, 217);
			// Top-Left to Top-Right
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, lineWidth), color);

			// Top-Left to Bottom-Left
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X, bounds.Y, lineWidth, bounds.Height), color);

			// Bottom-Left to Bottom-Right
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X, bounds.Y + bounds.Height - lineWidth, bounds.Width, lineWidth), color);

			// Top-Right to Bottom-Right
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X + bounds.Width - lineWidth, bounds.Y, lineWidth, bounds.Height), color);
		}
	}
}
