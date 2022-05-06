using System;
using System.Collections.Generic;

namespace DungeonCrawler.GameStates.PlayingState.PathFinding
{
    public class SimpleMove : IPathFinding
    {
        readonly List<IFloor> _floors;

        public SimpleMove(List<IFloor> floors)
        {
            _floors = floors;
        }

        enum Direction
        {
            UpDown,
            LeftRight,
        }

        IFloor FloorAt(int xIdx, int yIdx)
        {
            return _floors.Find(
                gs => (gs.XIdx == xIdx) && (gs.YIdx == yIdx)
            );
        }

        public Stack<IFloor> FindShortestPath(IFloor orig, IFloor dest)
        {
            var stack = new Stack<IFloor>();
            var xDiff = dest.XIdx - orig.XIdx;
            var yDiff = dest.YIdx - orig.YIdx;

            // Try move in the direction with the greatest difference
            if (Math.Abs(xDiff) >= Math.Abs(yDiff))
            {
                var xDisplacement = Math.Sign(xDiff);
                var floorInX = FloorAt(orig.XIdx + xDisplacement, orig.YIdx);
                if (floorInX != null)
                {
                    stack.Push(floorInX);
                    return stack;
                }
            }
            // If we can't move in that direction, try in the other direction
            if (Math.Abs(yDiff) > 0)
            {
                var yDisplacement = Math.Sign(yDiff);
                var floorInY = FloorAt(orig.XIdx, orig.YIdx + yDisplacement);
                if (floorInY != null)
                {
                    stack.Push(floorInY);
                    return stack;
                }
            }
            return stack;
        }
    }
}
