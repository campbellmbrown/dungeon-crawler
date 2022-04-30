using DungeonCrawler.Management;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class Player : Entity
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

        public override void ActionTick()
        {
            if (QueuedFloors.Count == 0)
            {
                _actionManager.Stop();
            }
            base.ActionTick();
        }
    }
}
