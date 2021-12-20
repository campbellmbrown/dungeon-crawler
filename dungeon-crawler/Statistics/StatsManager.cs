using dungeoncrawler.GameStates.PlayingState;
using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace dungeoncrawler.Statistics
{
    public delegate float GetStatsFunc();

    public class StatsManager
    {
        private bool _enabled;
        private float _lifeTime;
        private const float VERTICAL_SPACING = 10f;
        private const float HORIZONTAL_SPACING = 20f;

        private InputManager _inputManager;

        // Plots
        private Plot _drawingPlots;

        public StatsManager(ViewManager viewManager, GridManager gridManager)
        {
            _enabled = false;
            _lifeTime = 0;
            _inputManager = new InputManager();
            _inputManager.AddSingleShotInput(Keys.H, ToggleStats);

            _drawingPlots = new Plot(viewManager, "SpriteBatch Draws", "Count", new Vector2(HORIZONTAL_SPACING, VERTICAL_SPACING), 3f);
            _drawingPlots.AddSeries("GridSquares", Color.Yellow, gridManager.GetNumberDrawableGridSquares);
        }

        public void FrameTick(GameTime gameTime)
        {
            _inputManager.FrameTick();
            if (_enabled)
            {
                _lifeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _drawingPlots.Update(_lifeTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_enabled)
            {
                _drawingPlots.Draw(spriteBatch);
            }
        }

        public void ToggleStats()
        {
            _enabled = !_enabled;
            if (_enabled)
            {
                _lifeTime = 0;
                _drawingPlots.Clear();
            }
        }
    }
}
