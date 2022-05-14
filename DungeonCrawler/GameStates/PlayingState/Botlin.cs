using System;
using DungeonCrawler.GameStates.PlayingState.PathFinding;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IBotlin : IEntity
    {
    }

    public class Botlin : Entity, IBotlin
    {
        int _range = 4;

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
            var destination = _gridManager.PlayerFloor;
            if (Math.Max(Math.Abs(destination.XIdx - Floor.XIdx), Math.Abs(destination.YIdx - Floor.YIdx)) <= _range)
            {
                SetDestination(destination);
            }
            base.ActionTick();
        }
    }
}
