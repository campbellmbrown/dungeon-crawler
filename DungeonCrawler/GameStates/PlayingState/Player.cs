using DungeonCrawler.Management;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class Player : Entity
    {
        private readonly IEntityManager _entityManager;
        private readonly InputManager _inputManager;

        public Player(
            ILogManager logManager,
            IGridManager gridManager,
            IEntityManager entityManager,
            IPathFinding pathFinding,
            IFloor floor) :
            base(logManager, gridManager, pathFinding, floor)
        {
            _entityManager = entityManager;

            _inputManager = new InputManager();
        }

        public void PriorityFrameTick(IGameTimeWrapper gameTime)
        {
            _inputManager.FrameTick();
            // TODO: determine the best place for this to be. Maybe this should happen when the player clicks on a Floor.
            if (QueuedFloors.Count > 0 && DestinationState == DestinationState.AtDestination)
            {
                _entityManager.ActionTick();
            }
            base.FrameTick(gameTime);
        }

        public void PriorityActionTick()
        {
            base.ActionTick();
        }

        public override void FrameTick(IGameTimeWrapper gameTime)
        {
            throw new System.InvalidOperationException("The frame tick should be done before all other entities, performed in the PriorityFrameTick.");
        }

        public override void ActionTick()
        {
            throw new System.InvalidOperationException("The action tick should be done before all other entities, performed in the PriorityActionTick.");
        }
    }
}
