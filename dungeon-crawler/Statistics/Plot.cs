using dungeoncrawler.Management;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dungeoncrawler.Statistics
{
    public class Plot
    {
        public const float X_AXIS_LEN = 100f;
        public const float Y_AXIS_LEN = 40f;

        private readonly ViewManager _viewManager;
        private readonly string _name;
        private readonly string _units;
        public readonly BitmapFont font;
        public float lifeTime { get; private set; }
        public float scale { get { return 1 / _viewManager.cameraZoom; } }

        // Positional properties
        private Vector2 _originRelativeToBottomRight;
        public Vector2 origin { get { return _viewManager.bottomRight + _originRelativeToBottomRight; } }
        private readonly Vector2 _xAxisEndRelativeToOrigin;
        private readonly Vector2 _yAxisEndRelativeToOrigin;
        public Vector2 xAxisEnd { get { return origin + _xAxisEndRelativeToOrigin; } }
        public Vector2 yAxisEnd { get { return origin + _yAxisEndRelativeToOrigin; } }
        public Vector2 topMiddle { get { return origin + _xAxisEndRelativeToOrigin / 2f + _yAxisEndRelativeToOrigin; } }

        public float yAxisLimit { get; set; }
        public float xAxisLimit { get; }

        private int _seriesNumber = 0;
        private readonly List<Series> _series;

        public Plot(ViewManager viewManager, string name, string units, Vector2 offsetFromBottomRightCorner, float xAxisLimit)
        {
            _viewManager = viewManager;
            _name = name;
            _units = units;
            font = Game1.fonts["normal_font"];
            this.xAxisLimit = xAxisLimit;

            _xAxisEndRelativeToOrigin = new Vector2(X_AXIS_LEN, 0);
            _yAxisEndRelativeToOrigin = new Vector2(0, -Y_AXIS_LEN);
            _originRelativeToBottomRight = -offsetFromBottomRightCorner - _xAxisEndRelativeToOrigin;

            _series = new List<Series>();
        }

        public void AddSeries(string name, Color color, GetStatsFunc getStatsFunc)
        {
            _series.Add(new Series(name, color, getStatsFunc, _seriesNumber, this));
            _seriesNumber++;
        }

        public void Update(float lifeTime)
        {
            this.lifeTime = lifeTime;
            yAxisLimit = _series.Aggregate((largest, next) => next.maxData > largest.maxData ? next : largest).maxData;
            foreach (var series in _series)
            {
                series.Update(lifeTime);
            }
        }

        public void Clear()
        {
            foreach (var series in _series)
            {
                series.Clear();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // X and Y-axis lines
            spriteBatch.DrawLine(origin, xAxisEnd, Color.White, scale, DrawOrder.DEBUG);
            spriteBatch.DrawLine(origin, yAxisEnd, Color.White, scale, DrawOrder.DEBUG);
            // Plot title
            spriteBatch.DrawString(font, _name,
                topMiddle + new Vector2(-font.MeasureString(_name).Width / 2f, -font.LineHeight) * scale,
                Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, DrawOrder.DEBUG);
            // Y-axis label
            spriteBatch.DrawString(font, _units,
                yAxisEnd + new Vector2(-font.MeasureString(_units).Width - 2, 0) * scale,
                Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, DrawOrder.DEBUG);

            foreach (var series in _series)
            {
                series.Draw(spriteBatch);
            }
        }
    }
}
