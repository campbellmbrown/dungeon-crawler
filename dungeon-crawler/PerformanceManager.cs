using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace dungeoncrawler
{
    public interface IPerformanceManager
    {
        void Start();
        void Stop();
        void Draw(SpriteBatch spriteBatch);
    }

    public class PerformanceManager : IPerformanceManager
    {
        public Queue<float> times { get; set; } = new Queue<float>(); // Every 10 loops is a time

        readonly ILayerView _layerView;

        Stopwatch stopWatch;
        int currentLoop = 0;
        float averagePercent => times.Count > 0 ? 100 * CalculatePercentage(times.Average()) : 0f;
        Vector2 plotBottomRight => _layerView.topRight + new Vector2(-spacing, spacing) + new Vector2(-plotWidth, plotHeight);

        const int loopsPerTime = 10;
        const int maxTimes = 100;
        const int spacing = 10;
        const int plotHeight = 200;
        const int plotWidth = 400;
        const float milliSecondsPerTick = 1000 / 60f;

        public PerformanceManager(ILayerView layerView)
        {
            _layerView = layerView;

            stopWatch = new Stopwatch();
        }

        public void Start()
        {
            stopWatch.Start();
        }

        public void Stop()
        {
            stopWatch.Stop();
            currentLoop++;
            if (currentLoop > loopsPerTime)
            {
                times.Enqueue(stopWatch.ElapsedMilliseconds);
                while (times.Count > maxTimes)
                {
                    times.Dequeue();
                }
                currentLoop = 0;
                stopWatch.Reset();
            }
        }

        float CalculatePercentage(float time)
        {
            return time / (float)(loopsPerTime * milliSecondsPerTick);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Y axis
            spriteBatch.DrawLine(plotBottomRight, plotBottomRight + new Vector2(0, -plotHeight), Color.Yellow);
            // X axis
            spriteBatch.DrawLine(plotBottomRight, plotBottomRight + new Vector2(plotWidth, 0), Color.Yellow);

            int idx = 0;
            foreach (var time in times)
            {
                float xPos = plotBottomRight.X + (idx * plotWidth / (float)maxTimes);
                float yPos = plotBottomRight.Y - (plotHeight * CalculatePercentage(time));
                spriteBatch.DrawPoint(xPos, yPos, Color.White);
                idx++;
            }

            spriteBatch.DrawString(Game1.fonts["normal_font"], averagePercent.ToString("F"), plotBottomRight, Color.Yellow);
        }
    }
}
