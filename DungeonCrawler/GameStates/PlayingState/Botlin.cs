using System;
using DungeonCrawler.GameStates.PlayingState.PathFinding;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class Botlin : Entity, IEntity
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
            if (Math.Max(Math.Abs(destination.XIdx - _floor.XIdx), Math.Abs(destination.YIdx - _floor.YIdx)) <= _range)
            {
                SetDestination(destination);
            }
            base.ActionTick();
        }
    }
}
