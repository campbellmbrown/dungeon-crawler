using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class PlayingState : IGameState
    {
        private GridManager _gridManager;
        private Player _player;

        public PlayingState(ViewManager viewManager)
        {
            _gridManager = new GridManager();
            _player = new Player(viewManager, _gridManager, this);
            _gridManager.MoveToRandom(_player);
        }

        public void FrameTick(GameTime gameTime)
        {
            _player.FrameTick(gameTime);
            _gridManager.FrameTick(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _gridManager.Draw(spriteBatch);
        }

        public void ActionTick()
        {
            _player.ActionTick();
        }
    }
}
