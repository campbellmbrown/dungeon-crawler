using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public interface IGameTimeWrapper
    {
        float TimeDiffSec { get; }
        GameTime GameTime { set; }
    }

    public class GameTimeWrapper : IGameTimeWrapper
    {
        public GameTime GameTime { private get; set; }
        public float TimeDiffSec => (float)GameTime.ElapsedGameTime.TotalSeconds;
    }
}
