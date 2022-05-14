using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IFocusManager : IFrameTickable
    {
        void Focus(Vector2 position);
    }

    public class FocusManager : IFocusManager
    {
        readonly ISpriteBatchManager _spriteBatchManager;

        Vector2 _focusPoint;
        Vector2 _currentFocusPoint;
        const float PGain = 0.08f;

        public FocusManager(ISpriteBatchManager spriteBatchManager)
        {
            _spriteBatchManager = spriteBatchManager;
            _focusPoint = Vector2.Zero;
            _currentFocusPoint = Vector2.Zero;
        }

        public void Focus(Vector2 position)
        {
            _focusPoint = position;
        }

        public void FrameTick(IGameTimeWrapper gameTime)
        {
            _currentFocusPoint += (_focusPoint - _currentFocusPoint) * PGain;
            _spriteBatchManager.MainLayerView.Focus(_currentFocusPoint);
        }
    }
}
