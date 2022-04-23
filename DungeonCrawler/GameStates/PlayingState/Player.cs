using DungeonCrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class Player : Entity
    {
        private readonly IEntityManager _entityManager;
        private readonly InputManager _inputManager;

        public Player(GridManager gridManager, IEntityManager entityManager, IFloor floor) :
            base(gridManager, floor)
        {
            _entityManager = entityManager;

            _inputManager = new InputManager();
        }

        public void PriorityFrameTick(GameTime gameTime)
        {
            _inputManager.FrameTick();
            // TODO: determine the best place for this to be. Maybe this should happen when the player clicks on a Floor.
            if (QueuedFloors.Count > 0 && _destinationState == DestinationState.AtDestination)
            {
                _entityManager.ActionTick();
            }
            base.FrameTick(gameTime);
        }

        public void PriorityActionTick()
        {
            base.ActionTick();
        }

        public override void FrameTick(GameTime gameTime)
        {
            throw new System.InvalidOperationException("The frame tick should be done before all other entities, performed in the PriorityFrameTick.");
        }

        public override void ActionTick()
        {
            throw new System.InvalidOperationException("The action tick should be done before all other entities, performed in the PriorityActionTick.");
        }
    }
}
