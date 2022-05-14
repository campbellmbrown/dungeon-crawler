using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler
{
    public interface ISpriteBatchWrapper
    {
        SpriteBatch SpriteBatch { get; }
    }

    public class SpriteBatchWrapper : ISpriteBatchWrapper
    {
        SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch { get => _spriteBatch; }

        public SpriteBatchWrapper(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }
    }
}
