using DungeonCrawler.GameStates.PlayingState.PathFinding;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class Botlin : Entity, IEntity
    {
        public Botlin(
            ILogManager logManager,
            IGridManager gridManager,
            IActionManager actionManager,
            IPathFinding pathfinding,
            IFloor floor) :
            base(logManager, gridManager, actionManager, pathfinding, floor)
        {
        }

        public override void ActionTick()
        {
            SetDestination(_gridManager.PlayerFloor);
            base.ActionTick();
        }
    }
}
