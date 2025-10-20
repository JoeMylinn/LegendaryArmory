using Blish_HUD;
using Blish_HUD.Controls;
using LegendaryArmory.Models;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace LegendaryArmory.UI
{
    internal class LegendaryImage : Image
    {
        private LegendaryItem _item;
        public LegendaryItem Item
        {
            get => _item;
            set => _item = value;
        }

        public void UpdateOpacity()
        {
            Opacity = (float)(_item.Amount > 0 ? 1 : 0.3);
        }

        private BitmapFont _font = GameService.Content.DefaultFont16;

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            base.Paint(spriteBatch, bounds);
            DrawBorder(spriteBatch, bounds);

            var text = _item.Amount + "/" + _item.MaxAmount;
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
