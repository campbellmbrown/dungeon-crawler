using System;
using System.Collections.Generic;
using System.Text;
using dungeoncrawler.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace dungeoncrawler.Visual
{
    public interface ILayerView
    {
        float zoom { get; }
        OrthographicCamera camera { get; }
        Vector2 topLeft { get; }
        Vector2 bottomLeft { get; }
        Vector2 bottomRight { get; }
        Vector2 topRight { get; }
        Vector2 middle { get; }
        Vector2 mousePosition { get; }
        void Focus(Vector2 vector);
    }

    public class LayerView : ILayerView
    {
        // Dependencies
        private readonly SpriteBatchManager _spriteBatchManager;

        // Public
        public float zoom { get; }
        public OrthographicCamera camera { get; private set; }
        public Vector2 topLeft => camera.ScreenToWorld(Vector2.Zero);
        public Vector2 topRight => camera.ScreenToWorld(_spriteBatchManager.windowSize.X, 0);
        public Vector2 bottomLeft => camera.ScreenToWorld(0, _spriteBatchManager.windowSize.Y);
        public Vector2 bottomRight => camera.ScreenToWorld(_spriteBatchManager.windowSize);
        public Vector2 middle => camera.ScreenToWorld(_spriteBatchManager.windowSize / 2f);
        public Vector2 mousePosition
        {
            get
            {
                Vector2 _mousePos = Conversion.PointToVector2(Mouse.GetState().Position);
                return camera.ScreenToWorld(_mousePos.X, _mousePos.Y);
            }
        }

        // Private
        private const int SCALE_FACTOR = 1;

        public LayerView(SpriteBatchManager spriteBatchManager, GraphicsDevice graphicsDevice, float zoom)
        {
            _spriteBatchManager = spriteBatchManager;
            this.zoom = zoom;

            // Each layer has a different camera because they can have different positions/zooms.
            camera = new OrthographicCamera(graphicsDevice);

            float zoomAdjustment = zoom - SCALE_FACTOR;
            camera.ZoomIn(zoomAdjustment);
        }

        public void Focus(Vector2 focusPoint)
        {
            camera.LookAt(focusPoint);
        }
    }
}
