using System.Collections.Generic;
using System.Linq;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IDijkstra
    {
        Stack<IFloor> FindShortestPath(IFloor orig, IFloor dest);
    }

    public class Dijkstra : IDijkstra
    {
        readonly ILogManager _logManager;
        readonly IList<IFloor> _floors;

        public Dijkstra(ILogManager logManager, IList<IFloor> floors)
        {
            _logManager = logManager;
            _floors = floors;
        }

        private class Vertex
        {
            public const int INF = 9999;

            public int XIdx { get; }
            public int YIdx { get; }
            public int Dist { get; set; }
            public int Weight { get; set; }
            public Vertex Prev { get; set; }

            public Vertex(int xIdx, int yIdx)
            {
                XIdx = xIdx;
                YIdx = yIdx;
                Weight = 1;
                Dist = INF;
            }
        }

        public Stack<IFloor> FindShortestPath(IFloor orig, IFloor dest)
        {
            // See https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm for implementation.

            // Create a list of unvisited nodes. Set the distance to 0 for the source node.
            List<Vertex> unvisited = new List<Vertex>();
            foreach (var floor in _floors)
            {
                unvisited.Add(new Vertex(floor.XIdx, floor.YIdx));
            }
            Vertex target = unvisited.Find(v => (v.XIdx == dest.XIdx) && (v.YIdx == dest.YIdx));
            Vertex source = unvisited.Find(v => (v.XIdx == orig.XIdx) && (v.YIdx == orig.YIdx));
            source.Dist = 0;
            Vertex curr;

            while (unvisited.Count > 0)
            {
                // Choose unvisited node with smallest distance.
                curr = unvisited.Aggregate((c, d) => c.Dist < d.Dist ? c : d);
                unvisited.Remove(curr);

                if (curr == target)
                {
                    break;
                }

                // Look through all the neighbours and update their distances.
                List<(int, int)> possibleNeighbours = new List<(int, int)>() { (0, -1), (1, 0), (0, 1), (-1, 0) };
                foreach (var pn in possibleNeighbours)
                {
                    Vertex neighbour = unvisited.Find(v => (curr.XIdx + pn.Item1 == v.XIdx) && (curr.YIdx + pn.Item2 == v.YIdx));
                    if (neighbour != null)
                    {
                        int alt = curr.Dist + neighbour.Weight;
                        if (alt < neighbour.Dist)
                        {
                            neighbour.Dist = alt;
                            neighbour.Prev = curr;
                        }
                    }
                }
            }

            // Create a path of Floors.
            Stack<IFloor> path = new Stack<IFloor>();
            curr = target;
            if (curr.Prev != null || curr == source)
            {
                while (curr != null)
                {
                    path.Push(_floors.ToList().Find(gs => (gs.XIdx == curr.XIdx) && (gs.YIdx == curr.YIdx)));
                    curr = curr.Prev;
                }
            }

            _logManager.Log("A total of " + (_floors.Count - unvisited.Count).ToString() + "/" + _floors.Count.ToString() + " were checked.", LogLevel.Debug);
            // Remove the first one - this should be the origin.
            path.Pop();
            return path;
        }
    }
}
