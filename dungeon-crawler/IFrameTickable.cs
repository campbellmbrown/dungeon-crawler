using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public interface IFrameTickable
    {
        void FrameTick(GameTime gameTime);
    }
}