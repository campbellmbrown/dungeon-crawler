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

        bool CheckForPlayerToAttack()
        {
            var playerFloor = _gridManager.PlayerFloor;
            bool playerClose = false;
            if (_gridManager.FindFloor(Floor.XIdx - 1, Floor.YIdx) == playerFloor)
            {
                playerClose = true;
            }
            else if (_gridManager.FindFloor(Floor.XIdx + 1, Floor.YIdx) == playerFloor)
            {
                playerClose = true;
            }
            else if (_gridManager.FindFloor(Floor.XIdx, Floor.YIdx - 1) == playerFloor)
            {
                playerClose = true;
            }
            else if (_gridManager.FindFloor(Floor.XIdx, Floor.YIdx + 1) == playerFloor)
            {
                playerClose = true;
            }
            if (playerClose)
            {
                Attack(playerFloor);
            }
            return playerClose;
        }

        void ExtraActionTick()
        {
            // First check if the player is nearby so we can attack
            if (CheckForPlayerToAttack())
            {
                return;
            }
            // Otherwise move towards the player
            var destination = _gridManager.PlayerFloor;
            if (Math.Max(Math.Abs(destination.XIdx - Floor.XIdx), Math.Abs(destination.YIdx - Floor.YIdx)) <= _range)
            {
                SetDestination(destination);
            }
        }

        public override void ActionTick()
        {
            ExtraActionTick();
            base.ActionTick();
        }
    }
}
