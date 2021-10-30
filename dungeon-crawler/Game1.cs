using dungeoncrawler.GameStates.PlayingState;
using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace dungeoncrawler
{
    public class Game1 : Game
    {
        public enum GameState
        {
            Playing,
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Color _backgroundColor;

        public static Dictionary<string, Texture2D> textures { get; set; }
        public static Random random;

        private ViewManager _viewManager;
        private GameState _gameState;
        private PlayingState _playingState;

        public const string VERSION_STR = "v0.1.4";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            random = new Random();
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            _backgroundColor = new Color(10, 10, 12);
            _gameState = GameState.Playing;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            textures = new Dictionary<string, Texture2D>();

            _viewManager = new ViewManager(GraphicsDevice, _graphics, Window);
            _playingState = new PlayingState(_viewManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch (_gameState)
            {
                case GameState.Playing:
                    _playingState.FrameTick(gameTime);
                    break;
                default:
                    // TODO: add logging warning here
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                transformMatrix: _viewManager.camera.GetViewMatrix()
            );
            GraphicsDevice.Clear(_backgroundColor);
            switch (_gameState)
            {
                case GameState.Playing:
                    _playingState.Draw(_spriteBatch);
                    break;
                default:
                    // TODO: add logging warning here
                    break;
            }
            _viewManager.UpdateCameraPosition(Vector2.Zero); // TODO: move out of here to the player class
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
