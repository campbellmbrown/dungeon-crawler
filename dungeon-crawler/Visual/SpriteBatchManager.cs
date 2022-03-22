using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.Visual
{
    public enum DrawType
    {
        MainContent,
        OverlayContent,
        DebugContent,
        PointLightContent,
        ViewLightContent,
    }

    public class SpriteBatchManager
    {
        // Dependencies
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        // Public
        public Vector2 WindowSize => new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

        // Layer views
        public ILayerView MainLayerView { get; set; }
        public ILayerView OverlayLayerView { get; set; }
        public ILayerView DebugLayerView { get; set; }

        /* Render targets
         *
         * Instead of drawing our sprites to the back buffer we can instruct the GraphicsDevice to draw
         * to a render target instead. A render target is essentially an image that we are drawing to,
         * and then when we are done drawing to that render target we can draw it to the back buffer like
         * a regular texture. This is useful for:
         *
         * - Applying effects to a specific layer
         * - Applying global effects to all the layers (or specific layers) when we draw to the back buffer
         * - Having different cameras for each layer (e.g. a menu overlay vs game content)
         *
         * If a render target is going to be drawn to the entire screen it should be created with the
         * same resolution as the screen.
         */
        /// <summary>
        /// Render target for the main content.
        /// </summary>
        private RenderTarget2D _mainContentTarget;

        /// <summary>
        /// Render target for content that doesn't move with the player.
        /// </summary>
        private RenderTarget2D _overlayContentTarget;

        /// <summary>
        /// A special render target for drawing lights for the main content target.
        /// This render target will be an input to the light effect.
        /// </summary>
        private RenderTarget2D _pointLightTarget;

        /// <summary>
        /// A special render target for drawing shapes that narrow the view.
        /// This render target will be an input to the light effect and applied after
        /// the point light target.
        /// This means it will block out the lights from the point light target.
        /// </summary>
        private RenderTarget2D _viewLightTarget;

        /// <summary>
        /// Render target for debug prints/info.
        /// Could be removed to increase performance if needed.
        /// </summary>
        private RenderTarget2D _debugTarget;

        /// <summary>
        /// A temporary render target. This is used because we want to apply two light affects.
        /// First we draw the main content target to this target with the point light effect.
        /// Then we draw this target to the back buffer with the view light effect.
        /// </summary>
        private RenderTarget2D _tmpTarget;

        // Effects
        private Effect _pointlightingEffect;
        private Effect _viewlightingEffect;

        private Color _pointLightClearColor = new Color(30, 50, 100);
        private Color _viewLightClearColor = Color.Black;

        public static Vector2 ScreenSize
        {
            get
            {
                return new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
        }

        public SpriteBatchManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;

            LoadRenderTargets();
            LoadEffects(content);
            LoadLayerViews(graphicsDevice);
        }

        private void LoadRenderTargets()
        {
            PresentationParameters pp = _graphicsDevice.PresentationParameters;

            _mainContentTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _overlayContentTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _debugTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _pointLightTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _viewLightTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _tmpTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        private void LoadEffects(ContentManager content)
        {
            _pointlightingEffect = content.Load<Effect>("effects/lighting");
            _viewlightingEffect = content.Load<Effect>("effects/lighting");
        }

        private void LoadLayerViews(GraphicsDevice graphicsDevice)
        {
            MainLayerView = new LayerView(this, graphicsDevice, 4);
            DebugLayerView = new LayerView(this, graphicsDevice, 1);
        }

        public void Start(DrawType drawType)
        {
            switch (drawType)
            {
                case DrawType.MainContent:
                    _graphicsDevice.SetRenderTarget(_mainContentTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: MainLayerView.Camera.GetViewMatrix());
                    break;
                case DrawType.OverlayContent:
                    _graphicsDevice.SetRenderTarget(_overlayContentTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                    break;
                case DrawType.DebugContent:
                    _graphicsDevice.SetRenderTarget(_debugTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: DebugLayerView.Camera.GetViewMatrix());
                    break;
                case DrawType.ViewLightContent:
                    _graphicsDevice.SetRenderTarget(_viewLightTarget);
                    _graphicsDevice.Clear(_viewLightClearColor);
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: MainLayerView.Camera.GetViewMatrix());
                    break;
                case DrawType.PointLightContent:
                    _graphicsDevice.SetRenderTarget(_pointLightTarget);
                    _graphicsDevice.Clear(_pointLightClearColor);
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: MainLayerView.Camera.GetViewMatrix());
                    break;
            }
        }

        public void Switch(DrawType drawType)
        {
            _spriteBatch.End();
            Start(drawType);
        }

        public void Finish()
        {
            _spriteBatch.End();

            // (1) Draw the main content to the temporary target with the point light as a mask.
            _graphicsDevice.SetRenderTarget(_tmpTarget);
            _pointlightingEffect.Parameters["lightMask"].SetValue(_pointLightTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _pointlightingEffect);
            _spriteBatch.Draw(_mainContentTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Black);

            // (2) Draw the main content (on the temporary target) to the back buffer with the view light as a mask.
            _pointlightingEffect.Parameters["lightMask"].SetValue(_viewLightTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _viewlightingEffect);
            _spriteBatch.Draw(_tmpTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            // (3) Draw the rest of the sprite batches to the back buffer.
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_overlayContentTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(_debugTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();
        }
    }
}
