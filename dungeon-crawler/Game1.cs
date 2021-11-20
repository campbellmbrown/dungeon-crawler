using dungeoncrawler.GameStates.PlayingState;
using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
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
        public static Dictionary<string, BitmapFont> fonts { get; set; }
        public static Random random;

        private static LogManager _log;
        private ViewManager _viewManager;
        private GameState _gameState;
        private PlayingState _playingState;

        private const float HEARTBEAT_TIME = 1f; // sec
        private float _timeSinceLastHeartBeat = HEARTBEAT_TIME;

        public const string VERSION_STR = "v0.2.1";

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

            fonts = new Dictionary<string, BitmapFont>()
            {
                { "normal_font", Content.Load<BitmapFont>("fonts/normal_font") },
            };

            _viewManager = new ViewManager(GraphicsDevice, _graphics, Window);
            _playingState = new PlayingState(_viewManager);
            _log = new LogManager(_viewManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            HeartBeat(gameTime);

            switch (_gameState)
            {
                case GameState.Playing:
                    _playingState.FrameTick(gameTime);
                    break;
                default:
                    Log("Invalid GameState for updating", LogLevel.Error);
                    break;
            }
            _log.FrameTick();
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
                    Log("Invalid GameState for drawing", LogLevel.Error);
                    break;
            }
            _log.Draw(_spriteBatch);
            base.Draw(gameTime);
            _spriteBatch.End();
        }

        public static void Log(string message, LogLevel logLevel = LogLevel.Trace)
        {
            _log.AddLogMessage(message, logLevel);
        }

        private void HeartBeat(GameTime gameTime)
        {
            _timeSinceLastHeartBeat += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceLastHeartBeat >= HEARTBEAT_TIME)
            {
                Log("Heartbeat", LogLevel.Trace);
                _timeSinceLastHeartBeat -= HEARTBEAT_TIME;
            }
        }
    }
}
