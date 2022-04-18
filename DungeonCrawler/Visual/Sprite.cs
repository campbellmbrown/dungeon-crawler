using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.Visual
{
    public class Sprite
    {
        protected Texture2D _texture;

        public virtual Vector2 Size
        {
            get
            {
                return new Vector2(_texture.Width, _texture.Height);
            }
        }

        public Sprite(Texture2D texture)
        {
            this._texture = texture;
        }

        public void FrameTick(GameTime gameTime)
        {
        }

        public virtual void DrawCenter(SpriteBatch spriteBatch, Vector2 position, float layer = DrawOrder.DEFAULT)
        {
            Draw(spriteBatch, position - Size / 2f, layer);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float layer = DrawOrder.DEFAULT)
        {
            spriteBatch.Draw(_texture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
        }
    }
}
