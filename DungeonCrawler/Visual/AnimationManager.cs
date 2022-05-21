using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.Visual
{
    public interface IAnimationManager : IFrameTickable
    {
        int CurrentFrame { get; }
        IAnimation Animation { get; }
        Vector2 CurrentFrameRelativeBottomMiddle { get; }

        void Play(IAnimation animation);
        void Draw(ISpriteBatchWrapper spriteBatch, Vector2 position, float layer);
    }

    public class AnimationManager : IAnimationManager
    {
        public int CurrentFrame { get; private set; } = 0;

        IAnimation _animation;
        public IAnimation Animation
        {
            get
            {
                return _animation;
            }
            private set
            {
                _animation = value;
                CurrentFrame = 0;
                _timer = 0;
            }
        }

        float _frameSpeed
        {
            get
            {
                switch (Animation.Type)
                {
                    case IAnimation.FrameType.Constant:
                        return Animation.FrameSpeed;
                    case IAnimation.FrameType.Varying:
                        return Animation.FrameSpeeds[CurrentFrame];
                    default:
                        return 0;
                }
            }
        }

        public Vector2 CurrentFrameRelativeBottomMiddle => new Vector2(Animation.FrameWidth / 2f, Animation.FrameHeight);

        float _timer;
        Rectangle _drawRectangle => new Rectangle(CurrentFrame * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight);

        public AnimationManager(IAnimation animation)
        {
            Animation = animation;
        }

        public void FrameTick(IGameTimeWrapper gameTime)
        {
            _timer += gameTime.TimeDiffSec;
            var frameSpeed = _frameSpeed;
            if (_timer >= frameSpeed)
            {
                _timer -= frameSpeed;
                if (++CurrentFrame >= Animation.FrameCount)
                {
                    CurrentFrame = 0;
                }
            }
        }

        public void Play(IAnimation animation)
        {
            Animation = animation;
        }

        public void Draw(ISpriteBatchWrapper spriteBatch, Vector2 position, float layer)
        {
            spriteBatch.SpriteBatch.Draw(
                _animation.Texture,
                position,
                _drawRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                layer
            );
        }
    }
}
