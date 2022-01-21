using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.Visual
{
    public class SpriteSheet : Sprite
    {
        private Dictionary<int, Rectangle> _textureRectangles;
        private int _currentID;
        private Rectangle _currentTextureRectangle;

        public SpriteSheet(Texture2D texture, Dictionary<int, Rectangle> textureRectangles, int initialID) : base(texture)
        {
            _textureRectangles = textureRectangles;
            ChangeTextureRectangle(initialID);
        }

        public void ChangeTextureRectangle(int ID)
        {
            _currentID = ID;
            _currentTextureRectangle = _textureRectangles[ID];
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float opacity = 1f)
        {
            spriteBatch.Draw(texture, position, _currentTextureRectangle, Color.White * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
