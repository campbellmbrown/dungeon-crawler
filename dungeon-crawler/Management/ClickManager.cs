using System.Collections.Generic;
using dungeoncrawler.Utility;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace dungeoncrawler.Management
{
    public class ClickManager
    {
        public delegate void MethodDelegate();

        public class Click
        {
            public RectangleF clickArea { get; set; }
            public MethodDelegate outputFunc { get; }

            public Click(RectangleF clickArea, MethodDelegate outputFunc)
            {
                this.clickArea = clickArea;
                this.outputFunc = outputFunc;
            }
        }

        // Dependencies
        private readonly ILayerView _layerView;

        // Private
        private readonly List<Click> _leftClicks;
        private bool _leftClickHeld = false;

        public ClickManager(ILayerView layerView)
        {
            _layerView = layerView;
            _leftClicks = new List<Click>();
        }

        public void FrameTick()
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!_leftClickHeld)
                {
                    foreach (var leftClick in _leftClicks)
                    {
                        if (leftClick.clickArea.Contains(Conversion.Vector2ToPoint(_layerView.mousePosition)))
                        {
                            leftClick.outputFunc();
                        }
                    }
                }
                _leftClickHeld = true;
            }
            else
            {
                _leftClickHeld = false;
            }
        }

        public void AddLeftClick(RectangleF clickArea, MethodDelegate func)
        {
            _leftClicks.Add(new Click(clickArea, func));
        }
    }
}
