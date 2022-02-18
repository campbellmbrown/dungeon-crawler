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
        Vector2 TopLeft();
        Vector2 BottomLeft();
        Vector2 BottomRight();
        Vector2 TopRight();
        Vector2 Middle();
        Vector2 GetMousePosition();
        void Focus(Vector2 vector);
    }

    public class LayerView : ILayerView
    {
        // Dependencies
        private readonly SpriteBatchManager _spriteBatchManager;

        // Public
        public float zoom { get; }
        public OrthographicCamera camera { get; private set; }

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

        public Vector2 BottomLeft()
        {
            return camera.ScreenToWorld(0, _spriteBatchManager.windowSize.Y);
        }

        public Vector2 BottomRight()
        {
            return camera.ScreenToWorld(_spriteBatchManager.windowSize);
        }

        public Vector2 GetMousePosition()
        {
            Vector2 _mousePos = Conversion.PointToVector2(Mouse.GetState().Position);
            return camera.ScreenToWorld(_mousePos.X, _mousePos.Y);
        }

        public Vector2 TopLeft()
        {
            return camera.ScreenToWorld(Vector2.Zero);
        }

        public Vector2 TopRight()
        {
            return camera.ScreenToWorld(_spriteBatchManager.windowSize.X, 0);
        }

        public Vector2 Middle()
        {
            return camera.ScreenToWorld(_spriteBatchManager.windowSize / 2f);
        }

        public void Focus(Vector2 focusPoint)
        {
            camera.LookAt(focusPoint);
        }
    }
}
