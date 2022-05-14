using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.Visual
{
    public class SpriteSheet : Sprite
    {
        readonly ILogManager _logManager;
        readonly Dictionary<int, Rectangle> _textureRectangles;
        Rectangle _currentTextureRectangle;

        public SpriteSheet(ILogManager logManager, Texture2D texture, Dictionary<int, Rectangle> textureRectangles, int initialID) : base(texture)
        {
            _logManager = logManager;
            _textureRectangles = textureRectangles;
            ChangeTextureRectangle(initialID);
        }

        public void ChangeTextureRectangle(int id)
        {
            _currentTextureRectangle = _textureRectangles[id];
        }

        public override void Draw(ISpriteBatchWrapper spriteBatch, Vector2 position, float layer = DrawOrder.DEFAULT)
        {
            if (layer == DrawOrder.DEFAULT)
            {
                _logManager.Log("Drawing at default layer", LogLevel.Warning);
            }
            spriteBatch.SpriteBatch.Draw(_texture, position, _currentTextureRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
        }
    }
}
