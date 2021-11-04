using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class PlayingState : IGameState
    {
        private readonly ViewManager _viewManager;

        private GridManager _gridManager;
        private Player _player;

        public PlayingState(ViewManager viewManager)
        {
            _viewManager = viewManager;
            _gridManager = new GridManager();
            _player = new Player(_gridManager, this, _gridManager.GetRandomGridSquare());
            _gridManager.UpdateVisibilityStates();
        }

        public void FrameTick(GameTime gameTime)
        {
            _player.FrameTick(gameTime);
            _gridManager.FrameTick(gameTime);

            _viewManager.UpdateCameraPosition(_player.position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _gridManager.Draw(spriteBatch);
        }

        public void ActionTick()
        {
            _player.ActionTick();
            _gridManager.ActionTick();
        }
    }
}
