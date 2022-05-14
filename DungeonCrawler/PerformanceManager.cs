using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace DungeonCrawler
{
    public interface IPerformanceManager
    {
        void Start();
        void Stop();
        void Draw(SpriteBatch spriteBatch);
    }

    public class PerformanceManager : IPerformanceManager
    {
        readonly ILayerView _layerView;

        Queue<float> _times { get; set; } = new Queue<float>(); // Every 10 loops is a time
        Stopwatch _stopWatch;
        int _currentLoop = 0;
        float _averagePercent => _times.Count > 0 ? 100 * CalculatePercentage(_times.Average()) : 0f;
        Vector2 _plotBottomRight => _layerView.TopRight + new Vector2(-SPACING, SPACING) + new Vector2(-PLOT_WIDTH, PLOT_HEIGHT);

        const int LOOPS_PER_TIME = 10;
        const int MAX_TIMES = 100;
        const int SPACING = 10;
        const int PLOT_HEIGHT = 200;
        const int PLOT_WIDTH = 400;
        const float MILLI_SECONDS_PER_TICK = 1000 / 60f;

        public PerformanceManager(ILayerView layerView)
        {
            _layerView = layerView;

            _stopWatch = new Stopwatch();
        }

        public void Start()
        {
            _stopWatch.Start();
        }

        public void Stop()
        {
            _stopWatch.Stop();
            _currentLoop++;
            if (_currentLoop > LOOPS_PER_TIME)
            {
                _times.Enqueue(_stopWatch.ElapsedMilliseconds);
                while (_times.Count > MAX_TIMES)
                {
                    _times.Dequeue();
                }
                _currentLoop = 0;
                _stopWatch.Reset();
            }
        }

        float CalculatePercentage(float time)
        {
            return time / (float)(LOOPS_PER_TIME * MILLI_SECONDS_PER_TICK);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Y axis
            spriteBatch.DrawLine(_plotBottomRight, _plotBottomRight + new Vector2(0, -PLOT_HEIGHT), Color.Yellow);
            // X axis
            spriteBatch.DrawLine(_plotBottomRight, _plotBottomRight + new Vector2(PLOT_WIDTH, 0), Color.Yellow);

            int idx = 0;
            foreach (var time in _times)
            {
                float xPos = _plotBottomRight.X + (idx * PLOT_WIDTH / (float)MAX_TIMES);
                float yPos = _plotBottomRight.Y - (PLOT_HEIGHT * CalculatePercentage(time));
                spriteBatch.DrawPoint(xPos, yPos, Color.White);
                idx++;
            }

            spriteBatch.DrawString(Game1.Fonts["normal_font"], _averagePercent.ToString("F"), _plotBottomRight, Color.Yellow);
        }
    }
}
