using DungeonCrawler.Management;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IPlayingState : IGameState
    {
    }

    public class PlayingState : IPlayingState
    {
        // Dependencies
        readonly ILogManager _logManager;
        readonly SpriteBatchManager _spriteBatchManager;

        // Private
        readonly GridManager _gridManager;
        readonly IEntityManager _entityManager;
        readonly ClickManager _clickManager;
        Sprite _viewMask;
        // TODO: move to a MouseManager
        Sprite _mouseLight;

        public PlayingState(ILogManager logManager, SpriteBatchManager spriteBatchManager)
        {
            _logManager = logManager;
            _spriteBatchManager = spriteBatchManager;
            _clickManager = new ClickManager(spriteBatchManager.MainLayerView);
            _gridManager = new GridManager(logManager, this, _clickManager);
            _entityManager = new EntityManager(logManager, _gridManager);
            _mouseLight = new Sprite(Game1.Textures["medium_light"]);
            _viewMask = new Sprite(Game1.Textures["center_view"]);
        }

        public void FrameTick(GameTime gameTime)
        {
            _clickManager.FrameTick();
            _entityManager.FrameTick(gameTime);

            _spriteBatchManager.MainLayerView.Focus(_entityManager.Player.Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // We don't need to switch to DrawType.MainContent
            _gridManager.Draw(spriteBatch);
            _entityManager.Draw(spriteBatch);

            // Drawing overlay content
            // Uncomment to draw overlay content
            // _spriteBatchManager.Switch(DrawType.OverlayContent);

            _spriteBatchManager.Switch(DrawType.PointLightContent);
            // TODO: move to a MouseManager
            _mouseLight.DrawCenter(spriteBatch, _spriteBatchManager.MainLayerView.MousePosition);

            _spriteBatchManager.Switch(DrawType.ViewLightContent);
            _viewMask.DrawCenter(spriteBatch, _spriteBatchManager.MainLayerView.Middle);
        }

        public void ActionTick()
        {
            _entityManager.ActionTick();
        }

        public void SetPlayerDestination(IFloor floor)
        {
            if (_entityManager.IsBusy())
            {
                _logManager.Log("Action tick triggered but something is busy.", LogLevel.Warning);
            }
            else
            {
                _entityManager.Player.SetDestination(floor);
            }
        }
    }
}
