using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.Visual
{
    public enum AnimationId
    {
        PlayerIdleLeft,
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
            };
        }

        public IAnimation Get(AnimationId animation)
        {
            return _animations[animation];
        }
    }
}
