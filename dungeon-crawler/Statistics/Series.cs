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
    public class Series
    {
        private readonly Plot _plot;

        private readonly string _name;
        private Color _color;
        private readonly GetStatsFunc _getStatsFunc;
        private readonly int _seriesNumber;

        // Variable, time
        private readonly List<(float, float)> _samples;

        public float maxData
        {
            get
            {
                if (_samples.Count > 0)
                {
                    return _samples.Aggregate((largest, next) => next.Item1 > largest.Item1 ? next : largest).Item1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public Series(string name, Color color, GetStatsFunc getStatsFunc, int seriesNumber, Plot plot)
        {
            _getStatsFunc = getStatsFunc;

            _name = name;
            _color = color;
            _plot = plot;
            _seriesNumber = seriesNumber;

            _samples = new List<(float, float)>();
        }

        public void Update(float lifeTime)
        {
            float data = _getStatsFunc();
            _samples.Add((data, lifeTime));
            CutOldData();
        }

        public void Clear()
        {
            _samples.Clear();
        }

        private void CutOldData()
        {
            _samples.RemoveAll(sample => sample.Item2 < _plot.lifeTime - _plot.xAxisLimit);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Drawing lines
            Vector2 point1 = CalculatePositionOnPlot(_samples[0].Item2, _samples[0].Item1);
            Vector2 point2;
            for (int idx = 1; idx < _samples.Count; idx++)
            {
                point2 = CalculatePositionOnPlot(_samples[idx].Item2, _samples[idx].Item1);
                spriteBatch.DrawLine(point1, point2, _color, _plot.scale, DrawOrder.DEBUG);
                point1 = point2;
            }
            DrawLegendLabel(spriteBatch);
            DrawEndValue(spriteBatch);
        }

        private Vector2 CalculatePositionOnPlot(float xVal, float yVal)
        {
            Vector2 pos = new Vector2(
                pos.X = Plot.X_AXIS_LEN * (1 - (_plot.lifeTime - xVal) / _plot.xAxisLimit),
                pos.Y = Plot.Y_AXIS_LEN * -yVal / Math.Max(1, _plot.yAxisLimit)
            );
            return _plot.origin + pos;
        }

        private void DrawLegendLabel(SpriteBatch spriteBatch)
        {
            // Label bar
            spriteBatch.DrawLine(
                _plot.yAxisEnd + new Vector2(1, (_seriesNumber + 0.5f) * _plot.font.LineHeight * _plot.scale),
                _plot.yAxisEnd + new Vector2(5, (_seriesNumber + 0.5f) * _plot.font.LineHeight * _plot.scale),
                _color, _plot.scale, DrawOrder.DEBUG);
            // Label text
            spriteBatch.DrawString(_plot.font,
                _name,
                _plot.yAxisEnd + new Vector2(6, _seriesNumber * _plot.font.LineHeight * _plot.scale),
                _color, 0f, Vector2.Zero, _plot.scale, SpriteEffects.None, DrawOrder.DEBUG);
        }

        private void DrawEndValue(SpriteBatch spriteBatch)
        {
            if (_samples.Count > 0)
            {
                spriteBatch.DrawString(
                    _plot.font, _samples[^1].Item1.ToString(),
                    CalculatePositionOnPlot(_samples[^1].Item2, _samples[^1].Item1 - (_plot.font.LineHeight * _plot.scale / 2f)),
                    _color, 0f, Vector2.Zero, _plot.scale, SpriteEffects.None, DrawOrder.DEBUG);
            }
        }
    }
}
