using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Player : Entity
    {
        private readonly GridManager _gridManager;
        private readonly PlayingState _playingState;

        private readonly InputManager _inputManager;

        public Player(GridManager gridManager, PlayingState playingState, GridSquare gridSquare) :
            base(gridManager, gridSquare)
        {
            _gridManager = gridManager;
            _playingState = playingState;

            _inputManager = new InputManager();
        }

        public override void FrameTick(GameTime gameTime)
        {
            _inputManager.FrameTick();
            // TODO: determine the best place for this to be. Maybe this should happen when the player clicks on a grid square.
            if (queuedGridSquares.Count > 0 && destinationState == DestinationState.AtDestination)
            {
                _playingState.ActionTick();
            }
            base.FrameTick(gameTime);
        }

        public override void ActionTick()
        {
            base.ActionTick();
        }
    }
}
