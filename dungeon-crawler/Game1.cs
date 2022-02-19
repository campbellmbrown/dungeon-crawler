using System;
using System.Collections.Generic;
using System.Diagnostics;
using dungeoncrawler.GameStates;
using dungeoncrawler.GameStates.PlayingState;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace dungeoncrawler
{
    public class Game1 : Game
    {
        public enum GameState
        {
            Playing,
        }

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static Dictionary<string, Texture2D> textures { get; set; }
        public static Dictionary<string, BitmapFont> fonts { get; set; }

        public static Random random;

        private static LogManager _log;
        private GameState _gameState;
        private IGameState _playingState;

        private const float HEARTBEAT_TIME = 1f; // sec
        private float _timeSinceLastHeartBeat = HEARTBEAT_TIME;

        public const string VERSION_STR = "v0.3.4";

        private SpriteBatchManager _spriteBatchManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            random = new Random();
        }

        protected override void Initialize()
        {

            _graphics.IsFullScreen = true;
            Vector2 screenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            // TODO: move to spritebatch manager?
            _graphics.PreferredBackBufferWidth = (int)screenSize.X;
            _graphics.PreferredBackBufferHeight = (int)screenSize.Y;

            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();

            var pp = GraphicsDevice.PresentationParameters;

            IsMouseVisible = true;
            IsFixedTimeStep = true;

            _gameState = GameState.Playing;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatchManager = new SpriteBatchManager(GraphicsDevice, _spriteBatch, Content);

            textures = new Dictionary<string, Texture2D>()
            {
                // Tilesheets
                { "gray_brick_walls", Content.Load<Texture2D>("textures/tilesheets/gray_brick_walls") },
                { "gray_brick_floors", Content.Load<Texture2D>("textures/tilesheets/gray_brick_floors") },
                // Masks
                { "medium_light", Content.Load<Texture2D>("textures/masks/medium_light") },
                { "center_view", Content.Load<Texture2D>("textures/masks/center_view") },
            };

            fonts = new Dictionary<string, BitmapFont>()
            {
                { "normal_font", Content.Load<BitmapFont>("fonts/normal_font") },
            };

            _log = new LogManager(_spriteBatchManager.debugLayerView);
            _playingState = new PlayingState(_spriteBatchManager);
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
            _spriteBatchManager.Start(DrawType.MainContent);
            switch (_gameState)
            {
                case GameState.Playing:
                    _playingState.Draw(_spriteBatch);
                    break;
                default:
                    Log("Invalid GameState for drawing", LogLevel.Error);
                    break;
            }

            _spriteBatchManager.Switch(DrawType.DebugContent);
            _log.Draw(_spriteBatch);
            // base.Draw(gameTime); // Does this need to be done?
            _spriteBatchManager.Finish();
        }

        public static void Log(string message, LogLevel logLevel = LogLevel.Trace, bool writeToOutput = false)
        {
            _log.AddLogMessage(message, logLevel);
            if (writeToOutput)
            {
                Debug.WriteLine(message);
            }
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