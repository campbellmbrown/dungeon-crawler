using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dungeoncrawler.Visual
{
    public class Sprite
    {
        protected Texture2D texture;

        public virtual Vector2 size
        {
            get
            {
                return new Vector2(texture.Width, texture.Height);
            }
        }

        public Sprite(Texture2D texture)
        {
            this.texture = texture;
        }

        public void FrameTick(GameTime gameTime)
        {
        }

        public virtual void DrawCenter(SpriteBatch spriteBatch, Vector2 position, float layer = DrawOrder.DEFAULT)
        {
            Draw(spriteBatch, position - size / 2f, layer);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float layer = DrawOrder.DEFAULT)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
        }
    }
}
