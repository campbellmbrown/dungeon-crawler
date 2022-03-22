using DungeonCrawler.Management;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class PlayingState : IGameState
    {
        // Dependencies
        readonly SpriteBatchManager _spriteBatchManager;

        // Private
        readonly GridManager _gridManager;
        readonly ClickManager _clickManager;
        readonly Player _player;
        Sprite _viewMask;
        // TODO: move to a MouseManager
        Sprite _mouseLight;

        public PlayingState(SpriteBatchManager spriteBatchManager)
        {
            _spriteBatchManager = spriteBatchManager;
            _clickManager = new ClickManager(spriteBatchManager.MainLayerView);
            _gridManager = new GridManager(this, _clickManager);
            _player = new Player(_gridManager, this, _gridManager.GetStartingFloor());
            _mouseLight = new Sprite(Game1.Textures["medium_light"]);
            _viewMask = new Sprite(Game1.Textures["center_view"]);

            _gridManager.UpdateVisibilityStates();
        }

        public void FrameTick(GameTime gameTime)
        {
            _clickManager.FrameTick();
            _player.PriorityFrameTick(gameTime);
            _gridManager.FrameTick(gameTime);

            _spriteBatchManager.MainLayerView.Focus(_player.Position);
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
            _mouseLight.DrawCenter(spriteBatch, _spriteBatchManager.MainLayerView.MousePosition);

            _spriteBatchManager.Switch(DrawType.ViewLightContent);
            _viewMask.DrawCenter(spriteBatch, _spriteBatchManager.MainLayerView.Middle);
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
