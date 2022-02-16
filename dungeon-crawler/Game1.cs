using System;
using System.Collections.Generic;
using System.Diagnostics;
using dungeoncrawler.GameStates.PlayingState;
using dungeoncrawler.Management;
using dungeoncrawler.Utility;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private ViewManager _viewManager;
        private GameState _gameState;
        private PlayingState _playingState;

        private const float HEARTBEAT_TIME = 1f; // sec
        private float _timeSinceLastHeartBeat = HEARTBEAT_TIME;

        public const string VERSION_STR = "v0.3.3";

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

            Console.WriteLine(_graphics.PreferredBackBufferWidth + " " + _graphics.PreferredBackBufferHeight);

            var pp = GraphicsDevice.PresentationParameters;
            Debug.WriteLine(pp.BackBufferWidth + " " + pp.BackBufferHeight);

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
            };

            fonts = new Dictionary<string, BitmapFont>()
            {
                { "normal_font", Content.Load<BitmapFont>("fonts/normal_font") },
            };

            // TODO: add back
            // _viewManager = new ViewManager(GraphicsDevice, _graphics, Window);
            // _playingState = new PlayingState(_viewManager);
            // _playingState = new PlayingState();
            // _log = new LogManager(_viewManager);
            // _log = new LogManager(_viewManager);
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
                    // _playingState.FrameTick(gameTime);
                    break;
                default:
                    Log("Invalid GameState for updating", LogLevel.Error);
                    break;
            }
            // _log.FrameTick();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatchManager.Start(SpriteBatchManager.DrawType.MainContent);

            // TODO: remove
            int size = 40;
            for (int idx = 0; idx < 1000; idx += size)
            {
                for (int jdx = 0; jdx < 1000; jdx += size)
                {
                    _spriteBatch.Draw(textures["gray_brick_walls"], new Vector2(idx, jdx), new Rectangle(0, 0, size, size), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }

            // TODO: remove
            _spriteBatchManager.Switch(SpriteBatchManager.DrawType.OverlayContent);
            _spriteBatch.Draw(textures["gray_brick_walls"], new Vector2(4, 4), new Rectangle(0, 0, size, size), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // TODO: remove
            _spriteBatchManager.Switch(SpriteBatchManager.DrawType.LightContent);
            _spriteBatch.Draw(textures["medium_light"], new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(textures["medium_light"], new Vector2(100, 100), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(textures["medium_light"], new Vector2(150, 20), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(textures["medium_light"], new Vector2(30, 300), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            _spriteBatchManager.Finish();

            return;

            // TODO: add back
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

        public static void Log(string message, LogLevel logLevel = LogLevel.Trace, bool writeToOutput = false)
        {
            // _log.AddLogMessage(message, logLevel);
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
