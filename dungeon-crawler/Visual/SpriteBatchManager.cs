using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace dungeoncrawler.Visual
{
    public class SpriteBatchManager
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private RenderTarget2D _mainContentTarget;
        private RenderTarget2D _overlayContentTarget;
        private RenderTarget2D _lightTarget;

        Effect currentEffect = null;

        private Color _backgroundColor;

        private Effect _pixelateEffect;

        public enum DrawType
        {
            MainContent,
            OverlayContent,
            LightContent,
        }

        public SpriteBatchManager(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;

            LoadRenderTargets();
            LoadEffects(content);

            _backgroundColor = Color.Black;
        }

        private void LoadRenderTargets()
        {
            PresentationParameters pp = _graphicsDevice.PresentationParameters;

            _mainContentTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _overlayContentTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _lightTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        private void LoadEffects(ContentManager content)
        {
            _pixelateEffect = content.Load<Effect>("effects/pixelate");
        }

        public void Start(DrawType drawType)
        {
            currentEffect = null;

            // TODO: move
            var camera = new OrthographicCamera(_graphicsDevice);
            camera.LookAt(new Vector2(0, 0));

            switch (drawType)
            {
                case DrawType.MainContent:
                    _graphicsDevice.SetRenderTarget(_mainContentTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: currentEffect, transformMatrix: camera.GetViewMatrix());
                    break;
                case DrawType.OverlayContent:
                    _graphicsDevice.SetRenderTarget(_overlayContentTarget);
                    _graphicsDevice.Clear(Color.Transparent);
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: currentEffect);
                    break;
                case DrawType.LightContent:
                    _graphicsDevice.SetRenderTarget(_lightTarget);
                    _graphicsDevice.Clear(Color.Black);
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, effect: currentEffect, transformMatrix: camera.GetViewMatrix());
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

            // Now we will draw all of the render targets to the screen.
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(_backgroundColor);

            _pixelateEffect.Parameters["lightMask"].SetValue(_lightTarget);

            // Targets that have the light applied
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _pixelateEffect);
            _spriteBatch.Draw(_mainContentTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            // Targets that don't have light applied
            // Another option would be to have the effect: _pixelateEffect when drawing to the _mainContentTarget, then
            // the _spriteBatch.Begin()/.End() would only need to be called once here.
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_overlayContentTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();
        }
    }
}
