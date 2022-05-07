using System.Collections.Generic;

namespace DungeonCrawler.GameStates.PlayingState.PathFinding
{
    public interface IPathFinding
    {
        Stack<IFloor> FindShortestPath(IFloor orig, IFloor dest);
    }
}
