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

        public PlayingState(SpriteBatchManager spriteBatchManager)
        {
            _spriteBatchManager = spriteBatchManager;
            _clickManager = new ClickManager(spriteBatchManager.mainLayerView);
            _gridManager = new GridManager(this, _clickManager);
            _player = new Player(_gridManager, this, _gridManager.GetStartingFloor());

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
            _gridManager.Draw(spriteBatch);

            // TODO: remove
            _spriteBatchManager.Switch(DrawType.OverlayContent);
            spriteBatch.Draw(Game1.textures["gray_brick_walls"], new Vector2(4, 4), new Rectangle(0, 0, 40, 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            _spriteBatchManager.Switch(DrawType.ViewLightContent);
            spriteBatch.Draw(
                Game1.textures["center_view"],
                _spriteBatchManager.mainLayerView.middle - new Vector2(Game1.textures["center_view"].Width / 2f, Game1.textures["center_view"].Height / 2f),
                null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // TODO: remove
            _spriteBatchManager.Switch(DrawType.PointLightContent);
            spriteBatch.Draw(Game1.textures["medium_light"], new Vector2(0, 0), null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Game1.textures["medium_light"], new Vector2(30, 30), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Game1.textures["medium_light"], new Vector2(100, 100), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Game1.textures["medium_light"], new Vector2(150, 20), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Game1.textures["medium_light"], new Vector2(30, 300), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            spriteBatch.Draw(
                Game1.textures["medium_light"],
                _spriteBatchManager.mainLayerView.mousePosition - new Vector2(Game1.textures["medium_light"].Width / 2f, Game1.textures["medium_light"].Height / 2f),
                null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
