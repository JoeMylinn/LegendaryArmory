using Blish_HUD;
using Blish_HUD.Controls;
using Gw2Sharp;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace LegendaryArmory.UI
{
	internal class LegendaryImage : Image
	{
		private int _amount;

		private int _maxAmount;

		private int _generation;

		private ItemType _type;

		private ItemArmorSlotType _slot;

		private List<ProfessionWeaponFlag> _wieldType;

		private ItemWeightType _weightClass;
		public int Amount
		{
			get => _amount;
			set{
				SetProperty(ref _amount, value);
				Opacity = (float)(_amount > 0 ? 1 : 0.3);
			}
		}

		public int MaxAmount
		{
			get => _maxAmount;
			set
			{
				SetProperty(ref _maxAmount, value);
			}
		}

		public ItemType Type {
			get => _type;
			set => SetProperty(ref _type, value);
		}

		public List<ProfessionWeaponFlag> WieldType {
			get => _wieldType;
			set => SetProperty(ref _wieldType, value);
		}

		public ItemWeightType WeightClass
		{
			get => _weightClass;
			set => SetProperty(ref _weightClass, value);
		}

		public ItemArmorSlotType Slot
		{
			get => _slot;
			set => SetProperty(ref _slot, value);
		}

		public int Generation
		{
			get => _generation;
			set =>SetProperty(ref _generation, value);
		}

		private BitmapFont _font = GameService.Content.DefaultFont16;

		protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
		{
			base.Paint(spriteBatch, bounds);
			DrawBorder(spriteBatch, bounds);

			var text = _amount.ToString() + "/" + _maxAmount.ToString();
			var dest = new Rectangle(0, -2, bounds.Width, bounds.Height);
			spriteBatch.DrawStringOnCtrl(this, text, _font, dest,
										 new(255, 247, 169), false, true, 2,
										 HorizontalAlignment.Center, VerticalAlignment.Bottom);
		}

		private void DrawBorder(SpriteBatch spriteBatch, Rectangle bounds)
		{
			//Values for Border
			int lineWidth = 2;
			Color color = new Color(136, 79, 217);
			// Top-Left to Top-Right
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width - lineWidth, lineWidth), color);

			// Top-Left to Bottom-Left
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X, bounds.Y + lineWidth, lineWidth, bounds.Height - lineWidth), color);

			// Bottom-Left to Bottom-Right
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X + lineWidth, bounds.Y + bounds.Height - lineWidth, bounds.Width - lineWidth, lineWidth), color);

			// Top-Right to Bottom-Right
			spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bounds.X + bounds.Width - lineWidth, bounds.Y, lineWidth, bounds.Height - lineWidth), color);
		}
	}
}
