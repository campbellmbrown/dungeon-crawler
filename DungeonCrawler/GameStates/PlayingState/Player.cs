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

        public override void FrameTick(IGameTimeWrapper gameTime)
        {
            base.FrameTick(gameTime);
            if (_actionManager.ActionState != ActionState.Stopped && !PartakingInActionTick)
            {
                _actionManager.Stop();
            }
        }
    }
}
