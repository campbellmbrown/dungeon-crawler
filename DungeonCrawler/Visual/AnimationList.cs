using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.Visual
{
    public enum AnimationId
    {
        PlayerIdleLeft,
        PlayerIdleRight,
        PlayerIdleUp,
        PlayerIdleDown,
    }

    public interface IAnimationList
    {
        IAnimation Get(AnimationId animation);
    }

    public class AnimationList : IAnimationList
    {
        readonly Dictionary<AnimationId, IAnimation> _animations;

        public AnimationList(ContentManager content)
        {
            _animations = new Dictionary<AnimationId, IAnimation>()
            {
                { AnimationId.PlayerIdleLeft, new Animation(
                    content.Load<Texture2D>("textures/animations/player_idle_left"), 8,
                    new List<float> { 1f, 0.1f, 0.1f, 0.1f, 1f, 0.1f, 0.10f, 0.1f } )
                },
                { AnimationId.PlayerIdleRight, new Animation(
                    content.Load<Texture2D>("textures/animations/player_idle_right"), 8,
                    new List<float> { 1f, 0.1f, 0.1f, 0.1f, 1f, 0.1f, 0.10f, 0.1f } )
                },
                { AnimationId.PlayerIdleUp, new Animation(
                    content.Load<Texture2D>("textures/animations/player_idle_up"), 6,
                    new List<float> { 1.1f, 0.1f, 0.1f, 0.1f, 1f, 0.2f } )
                },
                { AnimationId.PlayerIdleDown, new Animation(
                    content.Load<Texture2D>("textures/animations/player_idle_down"), 7,
                    new List<float> { 1f, 0.1f, 0.1f, 0.1f, 1f, 0.1f, 0.2f } )
                },
            };
            CheckAllAnimationsLoaded();
        }

        public void CheckAllAnimationsLoaded()
        {
            foreach (AnimationId animationId in Enum.GetValues(typeof(AnimationId)))
            {
                Debug.Assert(_animations.ContainsKey(animationId), $"The animation is not loaded from the ContentManager for {animationId}");
            }
        }

        public IAnimation Get(AnimationId animation)
        {
            return _animations[animation];
        }
    }
}
