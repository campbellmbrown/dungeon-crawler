using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dungeoncrawler.Visual
{
    public class SpriteSheet : Sprite
    {
        private readonly Dictionary<int, Rectangle> _textureRectangles;
        private Rectangle _currentTextureRectangle;

        public SpriteSheet(Texture2D texture, Dictionary<int, Rectangle> textureRectangles, int initialID) : base(texture)
        {
            _textureRectangles = textureRectangles;
            ChangeTextureRectangle(initialID);
        }

        public void ChangeTextureRectangle(int ID)
        {
            _currentTextureRectangle = _textureRectangles[ID];
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float layer = DrawOrder.DEFAULT)
        {
            if (layer == DrawOrder.DEFAULT)
            {
                Game1.Log("Drawing at default layer", LogLevel.Warning);
            }
            spriteBatch.Draw(texture, position, _currentTextureRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
        }
    }
}
