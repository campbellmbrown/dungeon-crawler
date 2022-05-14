using DungeonCrawler.GameStates.PlayingState.PathFinding;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IPlayer : IEntity
    {
    }

    public class Player : Entity, IPlayer
    {
        public Player(
            ILogManager logManager,
            IGridManager gridManager,
            IActionManager actionManager,
            IPathFinding pathFinding,
            IFloor floor) :
            base(logManager, gridManager, actionManager, pathFinding, floor)
        {
        }

        public override void FrameTick(IGameTimeWrapper gameTime)
        {
            base.FrameTick(gameTime);
            if (_actionManager.ActionState != ActionState.Stopped && !PartakingInActionTick)
            {
                _actionManager.Stop();
            }
        }

        public override void SetDestination(IFloor destination)
        {
            base.SetDestination(destination);
            if (QueuedFloors.Count == 1 && QueuedFloors.Peek().Entity != null)
            {
                PartakingInActionTick = true;
                _origPosition = Floor.Position;
                _beenSwapped = true;

                var swapTarget = QueuedFloors.Dequeue().Entity;
                var swapTargetFloor = swapTarget.Floor;

                swapTarget.SwapWith(this);
                Floor = swapTargetFloor;
                Floor.Entity = this;
            }
        }
    }
}
