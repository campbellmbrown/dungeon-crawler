using dungeoncrawler.Management;
using dungeoncrawler.Statistics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class PlayingState : IGameState
    {
        private readonly ViewManager _viewManager;
        private readonly GridManager _gridManager;
        private readonly ClickManager _clickManager;
        private readonly StatsManager _statsManager;
        private readonly Player _player;

        public PlayingState(ViewManager viewManager)
        {
            _viewManager = viewManager;
            _clickManager = new ClickManager(_viewManager);
            _gridManager = new GridManager(this, _clickManager);
            _player = new Player(_gridManager, this, _gridManager.GetStartingFloor());
            _statsManager = new StatsManager(_viewManager, _gridManager);

            _gridManager.UpdateVisibilityStates();
        }

        public void FrameTick(GameTime gameTime)
        {
            _clickManager.FrameTick();
            _player.PriorityFrameTick(gameTime);
            _gridManager.FrameTick(gameTime);

            _viewManager.UpdateCameraPosition(_player.position);
            _statsManager.FrameTick(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _gridManager.Draw(spriteBatch);
            _statsManager.Draw(spriteBatch);
        }

        public void ActionTick()
        {
            _player.PriorityActionTick();
            _gridManager.ActionTick();
        }

        public void SetPlayerDestination(Floor floor)
        {
            if (_gridManager.Busy())
            {
                Game1.Log("Action tick triggered but something is busy.", LogLevel.Warning);
            }
            else
            {
                _player.SetDestination(floor);
            }
        }
    }
}
