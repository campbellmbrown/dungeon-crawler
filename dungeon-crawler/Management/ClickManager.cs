using System.Collections.Generic;
using DungeonCrawler.Utility;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace DungeonCrawler.Management
{
    public class ClickManager
    {
        public delegate void MethodDelegate();

        public class Click
        {
            public RectangleF ClickArea { get; set; }
            public MethodDelegate OutputFunc { get; }

            public Click(RectangleF clickArea, MethodDelegate outputFunc)
            {
                ClickArea = clickArea;
                OutputFunc = outputFunc;
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
                        if (leftClick.ClickArea.Contains(Conversion.Vector2ToPoint(_layerView.MousePosition)))
                        {
                            leftClick.OutputFunc();
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
