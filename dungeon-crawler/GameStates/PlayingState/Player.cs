using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Player : Entity
    {
        private readonly PlayingState _playingState;

        private readonly InputManager _inputManager;

        public Player(GridManager gridManager, PlayingState playingState, Floor floor) :
            base(gridManager, floor)
        {
            _playingState = playingState;

            _inputManager = new InputManager();
        }

        public void PriorityFrameTick(GameTime gameTime)
        {
            _inputManager.FrameTick();
            // TODO: determine the best place for this to be. Maybe this should happen when the player clicks on a Floor.
            if (queuedFloors.Count > 0 && destinationState == DestinationState.AtDestination)
            {
                _playingState.ActionTick();
            }
            base.FrameTick(gameTime);
        }

        public void PriorityActionTick()
        {
            base.ActionTick();
        }

        public override void FrameTick(GameTime gameTime)
        {
            // Do nothing. The frame tick should be done before all other entities, performed in the PriorityFrameTick.
        }

        public override void ActionTick()
        {
            // Do nothing. The action tick should be done before all other entities, performed in the PriorityActionTick.
        }
    }
}
