using System.Collections.Generic;
using System.Linq;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Dijkstra
    {
        private readonly GridManager _gridManager;

        public Dijkstra(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        private class Vertex
        {
            public const int INF = 9999;

            public int xIdx { get; }
            public int yIdx { get; }
            public int dist { get; set; }
            public int weight { get; set; }
            public Vertex prev { get; set; }

            public Vertex(int xIdx, int yIdx)
            {
                this.xIdx = xIdx;
                this.yIdx = yIdx;
                weight = 1;
                dist = INF;
            }
        }

        public Stack<GridSquare> FindShortestPath(GridSquare orig, GridSquare dest)
        {
            // See https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm for implementation.

            // Create a list of unvisited nodes. Set the distance to 0 for the source node.
            List<Vertex> unvisited = new List<Vertex>();
            foreach (var gs in _gridManager.gridSquares)
            {
                unvisited.Add(new Vertex(gs.xIdx, gs.yIdx));
            }
            Vertex target = unvisited.Find(v => (v.xIdx == dest.xIdx) && (v.yIdx == dest.yIdx));
            Vertex source = unvisited.Find(v => (v.xIdx == orig.xIdx) && (v.yIdx == orig.yIdx));
            source.dist = 0;
            Vertex curr;

            while (unvisited.Count > 0)
            {
                // Choose unvisited node with smallest distance.
                curr = unvisited.Aggregate((c, d) => c.dist < d.dist ? c : d);
                unvisited.Remove(curr);

                if (curr == target)
                {
                    break;
                }

                // Look through all the neighbours and update their distances.
                List<(int, int)> possibleNeighbours = new List<(int, int)>() { (0, -1), (1, 0), (0, 1), (-1, 0) };
                foreach (var pn in possibleNeighbours)
                {
                    Vertex neighbour = unvisited.Find(v => (curr.xIdx + pn.Item1 == v.xIdx) && (curr.yIdx + pn.Item2 == v.yIdx));
                    if (neighbour != null)
                    {
                        int alt = curr.dist + neighbour.weight;
                        if (alt < neighbour.dist)
                        {
                            neighbour.dist = alt;
                            neighbour.prev = curr;
                        }
                    }
                }
            }

            // Create a path of GridSquares.
            Stack<GridSquare> path = new Stack<GridSquare>();
            curr = target;
            if (curr.prev != null || curr == source)
            {
                while (curr != null)
                {
                    path.Push(_gridManager.gridSquares.Find(gs => (gs.xIdx == curr.xIdx) && (gs.yIdx == curr.yIdx)));
                    curr = curr.prev;
                }
            }

            Game1.Log("A total of " + (_gridManager.gridSquares.Count - unvisited.Count).ToString() + "/" + _gridManager.gridSquares.Count.ToString() + " were checked.", LogLevel.Debug);
            // Remove the first one - this should be the origin.
            path.Pop();
            return path;
        }
    }
}
