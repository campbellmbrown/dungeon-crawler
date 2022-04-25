using System;
using System.Collections.Generic;
using System.Diagnostics;
using DungeonCrawler.GameStates;
using DungeonCrawler.GameStates.PlayingState;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace DungeonCrawler
{
    public class Game1 : Game
    {
        public enum GameState
        {
            Playing,
        }

        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        IGameTimeWrapper _gameTimeWrapper = new GameTimeWrapper();
        ISpriteBatchWrapper _spriteBatchWrapper;

        public static Dictionary<string, Texture2D> Textures { get; set; }
        public static Dictionary<string, BitmapFont> Fonts { get; set; }

        public static Random Random { get; private set; }

        ILogManager _logManager;
        static PerformanceManager _performanceManager;
        GameState _gameState;
        IGameState _playingState;

        const float HEARTBEAT_TIME = 1f; // sec
        float _timeSinceLastHeartBeat = HEARTBEAT_TIME;

        public const string VERSION_STR = "v0.3.4";

        SpriteBatchManager _spriteBatchManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Random = new Random();
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

            IsMouseVisible = true;
            IsFixedTimeStep = true;

            _gameState = GameState.Playing;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatchManager = new SpriteBatchManager(GraphicsDevice, _spriteBatch, Content);
            _spriteBatchWrapper = new SpriteBatchWrapper(_spriteBatch);

            Textures = new Dictionary<string, Texture2D>()
            {
                // Tilesheets
                { "gray_brick_walls", Content.Load<Texture2D>("textures/tilesheets/gray_brick_walls") },
                { "gray_brick_floors", Content.Load<Texture2D>("textures/tilesheets/gray_brick_floors") },
                // Masks
                { "medium_light", Content.Load<Texture2D>("textures/masks/medium_light") },
                { "center_view", Content.Load<Texture2D>("textures/masks/center_view") },
            };

            Fonts = new Dictionary<string, BitmapFont>()
            {
                { "normal_font", Content.Load<BitmapFont>("fonts/normal_font") },
            };

            _logManager = new LogManager(_spriteBatchManager.DebugLayerView);
            _performanceManager = new PerformanceManager(_spriteBatchManager.DebugLayerView);
            _playingState = new PlayingState(_logManager, _spriteBatchManager);
        }

        protected override void Update(GameTime gameTime)
        {
            _gameTimeWrapper.GameTime = gameTime;

            _performanceManager.Start();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            HeartBeat(gameTime);

            switch (_gameState)
            {
                case GameState.Playing:
                    _playingState.FrameTick(_gameTimeWrapper);
                    break;
                default:
                    _logManager.Log("Invalid GameState for updating", LogLevel.Error);
                    break;
            }
            _logManager.FrameTick(_gameTimeWrapper);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatchManager.Start(DrawType.MainContent);
            switch (_gameState)
            {
                case GameState.Playing:
                    _playingState.Draw(_spriteBatchWrapper);
                    break;
                default:
                    _logManager.Log("Invalid GameState for drawing", LogLevel.Error);
                    break;
            }

            _spriteBatchManager.Switch(DrawType.DebugContent);
            _logManager.Draw(_spriteBatchWrapper);
            _performanceManager.Draw(_spriteBatch);
            // base.Draw(gameTime); // Does this need to be done?
            _spriteBatchManager.Finish();
            _performanceManager.Stop();
        }

        private void HeartBeat(GameTime gameTime)
        {
            _timeSinceLastHeartBeat += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceLastHeartBeat >= HEARTBEAT_TIME)
            {
                _logManager.Log("Heartbeat", LogLevel.Trace);
                _timeSinceLastHeartBeat -= HEARTBEAT_TIME;
            }
        }
    }
}
