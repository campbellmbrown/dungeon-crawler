using dungeoncrawler.Management;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class PlayingState : IGameState
    {
        // Dependencies
        private readonly SpriteBatchManager _spriteBatchManager;

        // Private
        private readonly GridManager _gridManager;
        private readonly ClickManager _clickManager;
        private readonly Player _player;
        private Sprite _viewMask;
        // TODO: move to a MouseManager
        private Sprite _mouseLight;

        public PlayingState(SpriteBatchManager spriteBatchManager)
        {
            _spriteBatchManager = spriteBatchManager;
            _clickManager = new ClickManager(spriteBatchManager.mainLayerView);
            _gridManager = new GridManager(this, _clickManager);
            _player = new Player(_gridManager, this, _gridManager.GetStartingFloor());
            _mouseLight = new Sprite(Game1.textures["medium_light"]);
            _viewMask = new Sprite(Game1.textures["center_view"]);

            _gridManager.UpdateVisibilityStates();
        }

        public void FrameTick(GameTime gameTime)
        {
            _clickManager.FrameTick();
            _player.PriorityFrameTick(gameTime);
            _gridManager.FrameTick(gameTime);

            _spriteBatchManager.mainLayerView.Focus(_player.position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // We don't need to switch to DrawType.MainContent
            _gridManager.Draw(spriteBatch);

            // Drawing overlay content
            // Uncomment to draw overlay content
            // _spriteBatchManager.Switch(DrawType.OverlayContent);

            _spriteBatchManager.Switch(DrawType.PointLightContent);
            // TODO: move to a MouseManager
            _mouseLight.DrawCenter(spriteBatch, _spriteBatchManager.mainLayerView.mousePosition);

            _spriteBatchManager.Switch(DrawType.ViewLightContent);
            _viewMask.DrawCenter(spriteBatch, _spriteBatchManager.mainLayerView.middle);
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
