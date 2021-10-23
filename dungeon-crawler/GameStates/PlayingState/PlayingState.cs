using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class PlayingState : IGameState
    {
        private GridManager _gridManager;

        public PlayingState()
        {
            _gridManager = new GridManager();
        }

        public void FrameTick(GameTime gameTime)
        {
            _gridManager.FrameTick(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _gridManager.Draw(spriteBatch);
        }
    }
}
