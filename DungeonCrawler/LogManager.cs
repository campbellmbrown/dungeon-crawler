using System;
using System.Collections.Generic;
using DungeonCrawler.Management;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace DungeonCrawler
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
        Other = 6,
        LogOff = 7,
    }

    public class LogPrint
    {
        private static Vector2 _textOffset = new Vector2(145, 0);
        private static Vector2 _levelOffset = new Vector2(80, 0);
        private static readonly Dictionary<LogLevel, Color> _logColors = new Dictionary<LogLevel, Color>()
        {
            { LogLevel.Trace, Color.Gray },
            { LogLevel.Debug, Color.LightGreen },
            { LogLevel.Info, Color.LightBlue },
            { LogLevel.Warning, Color.Yellow },
            { LogLevel.Error, Color.Orange },
            { LogLevel.Fatal, Color.Red },
            { LogLevel.Other, Color.White },
        };

        private readonly string _logText;
        private readonly LogLevel _logLevel;
        private readonly DateTime _logTime;

        public LogPrint(string logText, LogLevel logLevel)
        {
            _logText = logText;
            _logLevel = logLevel;
            _logTime = DateTime.Now;
        }

        public void Draw(ISpriteBatchWrapper spriteBatch, Vector2 position, float opacity)
        {
            spriteBatch.SpriteBatch.DrawString(
                Game1.Fonts["normal_font"], _logTime.ToString("HH:mm:ss.fff"),
                position,
                _logColors[_logLevel] * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawOrder.DEFAULT);
            spriteBatch.SpriteBatch.DrawString(
                Game1.Fonts["normal_font"], Enum.GetName(typeof(LogLevel), _logLevel).ToUpper(),
                position + _levelOffset,
                _logColors[_logLevel] * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawOrder.DEFAULT);
            spriteBatch.SpriteBatch.DrawString(
                Game1.Fonts["normal_font"],
                _logText,
                position + _textOffset,
                _logColors[_logLevel] * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawOrder.DEFAULT);
        }
    }

    public interface ILogManager : IFrameTickable, IMyDrawable
    {
        void Log(string message, LogLevel logLevel);
        void Log(string message);
    }

    public class LogManager : ILogManager
    {
        private const int MAX_LOGS = 200;
        private const int ACCEPTABLE_LOGS = 100;
        private const int MAX_LOGS_ON_SCREEN = 50;
        private static Vector2 _cornerBuffer = new Vector2(4, -2);

        private readonly ILayerView _layerView;
        private readonly InputManager _inputManager;
        private readonly List<LogPrint> _logPrints;

        private LogLevel _currentLogLevel;
        private bool _logActive { get { return _currentLogLevel != LogLevel.LogOff; } }

        public LogManager(ILayerView layerView)
        {
            _layerView = layerView;

            _currentLogLevel = LogLevel.LogOff;
            _inputManager = new InputManager();
            _logPrints = new List<LogPrint>();
            _inputManager.AddSingleShotInput(Keys.G, CycleLog);
        }

        public void FrameTick(IGameTimeWrapper gameTime)
        {
            _inputManager.FrameTick();
        }

        public void Draw(ISpriteBatchWrapper spriteBatch)
        {
            if (_logActive)
            {
                int messagesToPrint = Math.Min(_logPrints.Count, MAX_LOGS_ON_SCREEN);

                for (int idx = 0; idx < messagesToPrint; idx++)
                {
                    float opacity = 1.0f;
                    if (idx > MAX_LOGS_ON_SCREEN - 5)
                    {
                        opacity = (MAX_LOGS_ON_SCREEN - idx) / 5.0f;
                    }
                    Vector2 position = _layerView.BottomLeft + _cornerBuffer - new Vector2(0, (1 + idx) * Game1.Fonts["normal_font"].LineHeight);
                    _logPrints[_logPrints.Count - 1 - idx].Draw(spriteBatch, position, opacity);
                }
            }
        }

        public void CycleLog()
        {
            if (_currentLogLevel == LogLevel.LogOff)
            {
                _currentLogLevel = LogLevel.Trace;
            }
            else
            {
                _currentLogLevel++;
            }
            Log("Changed log level to " + Enum.GetName(typeof(LogLevel), _currentLogLevel), LogLevel.Other);
        }

        public void Log(string message)
        {
            Log(message, LogLevel.Trace);
        }

        public void Log(string message, LogLevel logLevel)
        {
            if (logLevel >= _currentLogLevel)
            {
                _logPrints.Add(new LogPrint(message, logLevel));
                if (_logPrints.Count > MAX_LOGS)
                {
                    while (_logPrints.Count > ACCEPTABLE_LOGS)
                    {
                        _logPrints.RemoveAt(0);
                    }
                    Log("Erasing old logs to save memory", LogLevel.Warning);
                }
            }
        }
    }
}
