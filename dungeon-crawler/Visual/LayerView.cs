using DungeonCrawler.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace DungeonCrawler.Visual
{
    public interface ILayerView
    {
        float Zoom { get; }
        OrthographicCamera Camera { get; }
        Vector2 TopLeft { get; }
        Vector2 BottomLeft { get; }
        Vector2 BottomRight { get; }
        Vector2 TopRight { get; }
        Vector2 Middle { get; }
        Vector2 MousePosition { get; }
        void Focus(Vector2 vector);
    }

    public class LayerView : ILayerView
    {
        // Dependencies
        readonly SpriteBatchManager _spriteBatchManager;

        // Public
        public float Zoom { get; }
        public OrthographicCamera Camera { get; private set; }
        public Vector2 TopLeft => Camera.ScreenToWorld(Vector2.Zero);
        public Vector2 TopRight => Camera.ScreenToWorld(_spriteBatchManager.WindowSize.X, 0);
        public Vector2 BottomLeft => Camera.ScreenToWorld(0, _spriteBatchManager.WindowSize.Y);
        public Vector2 BottomRight => Camera.ScreenToWorld(_spriteBatchManager.WindowSize);
        public Vector2 Middle => Camera.ScreenToWorld(_spriteBatchManager.WindowSize / 2f);
        public Vector2 MousePosition
        {
            get
            {
                Vector2 mousePos = Conversion.PointToVector2(Mouse.GetState().Position);
                return Camera.ScreenToWorld(mousePos.X, mousePos.Y);
            }
        }

        // Private
        private const int SCALE_FACTOR = 1;

        public LayerView(SpriteBatchManager spriteBatchManager, GraphicsDevice graphicsDevice, float zoom)
        {
            _spriteBatchManager = spriteBatchManager;
            this.Zoom = zoom;

            // Each layer has a different camera because they can have different positions/zooms.
            Camera = new OrthographicCamera(graphicsDevice);

            float zoomAdjustment = zoom - SCALE_FACTOR;
            Camera.ZoomIn(zoomAdjustment);
        }

        public void Focus(Vector2 focusPoint)
        {
            Camera.LookAt(focusPoint);
        }
    }
}
