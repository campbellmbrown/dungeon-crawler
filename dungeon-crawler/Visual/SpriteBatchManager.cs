using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using dungeoncrawler.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace dungeoncrawler.Visual
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

        // Layer views
        public ILayerView mainLayerView { get; set; }
        public ILayerView overlayLayerView { get; set; }
        public ILayerView debugLayerView { get; set; }

        // Render targets
        private RenderTarget2D _mainContentTarget;
        private RenderTarget2D _overlayContentTarget;
        private RenderTarget2D _pointLightTarget;
        private RenderTarget2D _viewLightTarget;
        private RenderTarget2D _debugTarget;

        // Effects
        private Effect _pointlightingEffect;
        private Effect _viewlightingEffect;

        Effect currentEffect = null;
        private Color _backgroundColor;

        public Vector2 windowSize
        {
            get
            {
                return new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            }
        }

        public static Vector2 screenSize
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

            _backgroundColor = Color.Black;
        }

        private void LoadRenderTargets()
        {
            PresentationParameters pp = _graphicsDevice.PresentationParameters;

            _mainContentTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _overlayContentTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _debugTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _pointLightTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _viewLightTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        private void LoadEffects(ContentManager content)
        {
            _pointlightingEffect = content.Load<Effect>("effects/lighting");
            _viewlightingEffect = content.Load<Effect>("effects/lighting");
        }

        private void LoadLayerViews(GraphicsDevice graphicsDevice)
        {
            mainLayerView = new LayerView(this, graphicsDevice, 4);
            debugLayerView = new LayerView(this, graphicsDevice, 1);
        }

        public void Start(DrawType drawType)
        {
            currentEffect = null;

            switch (drawType)
            {
                case DrawType.MainContent:
                    _graphicsDevice.SetRenderTarget(_mainContentTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, effect: currentEffect, transformMatrix: mainLayerView.camera.GetViewMatrix());
                    break;
                case DrawType.OverlayContent:
                    _graphicsDevice.SetRenderTarget(_overlayContentTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: currentEffect);
                    break;
                case DrawType.DebugContent:
                    _graphicsDevice.SetRenderTarget(_debugTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: debugLayerView.camera.GetViewMatrix());
                    break;
                case DrawType.ViewLightContent:
                    _graphicsDevice.SetRenderTarget(_viewLightTarget);
                    _graphicsDevice.Clear(Color.Black);
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: mainLayerView.camera.GetViewMatrix());
                    break;
                case DrawType.PointLightContent:
                    _graphicsDevice.SetRenderTarget(_pointLightTarget);
                    _graphicsDevice.Clear(new Color(10, 10, 10));
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: mainLayerView.camera.GetViewMatrix());
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

            // 1. Draw the content target to the tmp target with pointLightEffect
            // 2. Draw the tmp target to the backbuffer with the viewLightEffect

            RenderTarget2D _tmpTarget = new RenderTarget2D(_graphicsDevice, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            _graphicsDevice.SetRenderTarget(_tmpTarget);
            _pointlightingEffect.Parameters["lightMask"].SetValue(_pointLightTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _pointlightingEffect);
            _spriteBatch.Draw(_mainContentTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            // Now we will draw all of the render targets to the screen.
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(_backgroundColor);

            // Targets that have the light applied
            _pointlightingEffect.Parameters["lightMask"].SetValue(_viewLightTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _viewlightingEffect);
            _spriteBatch.Draw(_tmpTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            // Targets that don't have light applied
            // Another option would be to have the effect: _pixelateEffect when drawing to the _mainContentTarget, then
            // the _spriteBatch.Begin()/.End() would only need to be called once here?
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_overlayContentTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(_debugTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

        }
    }
}
