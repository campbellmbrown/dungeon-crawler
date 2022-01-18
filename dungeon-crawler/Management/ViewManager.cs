using dungeoncrawler.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace dungeoncrawler.Management
{
    public class ViewManager
    {
        public OrthographicCamera camera { get; set; }

        public const int SCALE_FACTOR = 1;
        public float cameraZoom = 4;

        public Vector2 mousePosition
        {
            get
            {
                Vector2 _mousePos = Conversion.PointToVector2(Mouse.GetState().Position);
                return camera.ScreenToWorld(_mousePos.X, _mousePos.Y);
            }
        }
        public static Vector2 screenSize
        {
            get
            {
                return new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
        }
        private readonly GraphicsDevice _graphicsDevice;
        public Vector2 windowSize
        {
            get
            {
                return new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            }
        }

        public Vector2 topLeft { get { return camera.ScreenToWorld(Vector2.Zero); } }
        public Vector2 bottomLeft { get { return camera.ScreenToWorld(0, windowSize.Y); } }
        public Vector2 bottomRight { get { return camera.ScreenToWorld(windowSize); } }
        public Vector2 topRight { get { return camera.ScreenToWorld(windowSize.X, 0); } }

        public ViewManager(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, GameWindow gameWindow)
        {
            _graphicsDevice = graphicsDevice;

            // Setting up the screen size
            graphicsDeviceManager.PreferredBackBufferWidth = (int)screenSize.X;
            graphicsDeviceManager.PreferredBackBufferHeight = (int)screenSize.Y;
            graphicsDeviceManager.IsFullScreen = true;

            // Some other graphics device settings
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            graphicsDeviceManager.ApplyChanges();

            // Creating camera
            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth / SCALE_FACTOR,
                _graphicsDevice.PresentationParameters.BackBufferHeight / SCALE_FACTOR);
            camera = new OrthographicCamera(viewportAdapter);

            // Note: desiredCameraZoom must be more (or equal to) than scaleFactor
            float zoomAdjustment = cameraZoom - SCALE_FACTOR;
            camera.ZoomIn(zoomAdjustment);
        }

        public void UpdateCameraPosition(Vector2 desiredCenterOfScreen)
        {
            camera.LookAt(desiredCenterOfScreen);
        }

        public Vector2 ScreenToWorld(Vector2 screenVector)
        {
            return camera.ScreenToWorld(screenVector);
        }

        public Vector2 WorldToScreen(Vector2 worldVector)
        {
            return camera.WorldToScreen(worldVector);
        }
    }
}
