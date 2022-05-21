using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.Visual
{
    public interface IAnimation
    {
        public enum FrameType
        {
            Constant,
            Varying,
        }

        public Texture2D Texture { get; }
        public int FrameCount { get; }
        public int FrameHeight { get; }
        public int FrameWidth { get; }
        public float FrameSpeed { get; }
        public List<float> FrameSpeeds { get; }
        public FrameType Type { get; }
    }

    public class Animation : IAnimation
    {

        public Texture2D Texture { get; }
        public int FrameCount { get; }
        public int FrameHeight { get { return Texture.Height; } }
        public int FrameWidth { get { return Texture.Width / FrameCount; } }
        public IAnimation.FrameType Type { get; private set; }

        List<float> _frameSpeeds;
        public List<float> FrameSpeeds
        {
            get
            {
                return _frameSpeeds;
            }
            private set
            {
                _frameSpeeds = value;
                Type = IAnimation.FrameType.Varying;
            }
        }

        float _frameSpeed;
        public float FrameSpeed
        {
            get
            {
                return _frameSpeed;
            }
            private set
            {
                _frameSpeed = value;
                Type = IAnimation.FrameType.Constant;
            }
        }

        public Animation(Texture2D texture, int frameCount, float frameSpeed)
        {
            Texture = texture;
            FrameCount = frameCount;
            FrameSpeed = frameSpeed;
        }

        public Animation(Texture2D texture, int frameCount, List<float> frameSpeeds)
        {
            Texture = texture;
            FrameCount = frameCount;
            FrameSpeeds = frameSpeeds;
        }
    }
}
