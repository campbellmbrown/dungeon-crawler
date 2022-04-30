using DungeonCrawler.Management;
using DungeonCrawler.Visual;

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
        readonly IActionManager _actionManager;
        Sprite _viewMask;
        // TODO: move to a MouseManager
        Sprite _mouseLight;

        public PlayingState(ILogManager logManager, SpriteBatchManager spriteBatchManager)
        {
            _logManager = logManager;
            _spriteBatchManager = spriteBatchManager;
            _actionManager = new ActionManager(_logManager);
            _clickManager = new ClickManager(_spriteBatchManager.MainLayerView);
            _gridManager = new GridManager(_logManager, this, _clickManager);
            _entityManager = new EntityManager(_logManager, _gridManager, _actionManager);
            _mouseLight = new Sprite(Game1.Textures["medium_light"]);
            _viewMask = new Sprite(Game1.Textures["center_view"]);
        }

        public void FrameTick(IGameTimeWrapper gameTime)
        {
            _clickManager.FrameTick();
            _entityManager.FrameTick(gameTime);
            _actionManager.FrameTick(gameTime);

            _spriteBatchManager.MainLayerView.Focus(_entityManager.Player.Position);
        }

        public void Draw(ISpriteBatchWrapper spriteBatch)
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

        public void SetPlayerDestination(IFloor floor)
        {
            if (_actionManager.ActionState != ActionState.Stopped)
            {
                _logManager.Log("Action tick triggered but actions are in progress.", LogLevel.Warning);
            }
            else
            {
                var floorsQueued = _entityManager.Player.SetDestination(floor);
                if (floorsQueued > 0)
                {
                    _actionManager.Start();
                }
            }
        }
    }
}
