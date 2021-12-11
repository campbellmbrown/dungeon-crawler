using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Entity
    {
        private enum DestinationState
        {
            AtDestination,
            OffDestination,
        }

        private readonly GridManager _gridManager;

        private const float MOVEMENT_SPEED = 48f; // 16 pixels/second.
        private const float DESTINATION_HYSTERESIS = 0.5f; // How many pixels away for the destination to be considered reached.
        private const float MAX_QUEUED_DESTINATIONS = 3;

        private DestinationState destinationState;
        private Queue<Vector2> _destinations;
        public Vector2 position { get; private set; }
        private GridSquare _gridSquare;

        public Entity(GridManager gridManager, GridSquare gridSquare)
        {
            _gridManager = gridManager;
            _destinations = new Queue<Vector2>();
            _gridSquare = gridSquare;
            gridSquare.entity = this;

            position = _gridSquare.position;
            destinationState = DestinationState.AtDestination;
        }

        /// <summary>
        /// Sets the GridSquare for this Entity, and this Entity for the GridSquare.  
        /// Also clears the Entity from the GridSquare the Entity is originally in. 
        /// </summary>
        /// <param name="gridSquare">The new GridSquare for this entity to belong to.</param>
        public void ChangeGridSquare(GridSquare gridSquare)
        {
            _gridSquare.entity = null;
            _gridSquare = gridSquare;
            _gridSquare.entity = this;
            _destinations.Enqueue(_gridSquare.position);
            destinationState = DestinationState.OffDestination;
        }

        public virtual void FrameTick(GameTime gameTime)
        {
            if (destinationState == DestinationState.OffDestination)
            {
                Vector2 currentDestination = _destinations.Peek();
                position += Vector2.Normalize(currentDestination - position) * MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if ((currentDestination - position).Length() < DESTINATION_HYSTERESIS)
                {
                    position = currentDestination;
                    _destinations.Dequeue();
                    if (_destinations.Count == 0)
                    {
                        destinationState = DestinationState.AtDestination;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, 4, 4), Color.Yellow);
        }

        public virtual void ActionTick()
        {

        }

        // private List<GridSquare> FindPathTo()
        // {

        // }

        private class Vertex
        {
            public static int INF = 9999;

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

        private Stack<GridSquare> Dijkstra(GridSquare dest)
        {
            // See https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm for implementation.

            List<Vertex> Q = new List<Vertex>();

            foreach (var v in _gridManager.gridSquares)
            {
                Q.Add(new Vertex(v.xIdx, v.yIdx));
            }
            Vertex target = Q.Find(v => (v.xIdx == dest.xIdx) && (v.yIdx == dest.yIdx));
            Vertex source = Q.Find(v => (v.xIdx == 0) && (v.yIdx == 0));
            source.dist = 0;
            Vertex u;

            while (Q.Count > 0)
            {
                u = Q.Aggregate((c, d) => c.dist < d.dist ? c : d);
                Q.Remove(u);

                if (u == target)
                {
                    break;
                }

                List<(int, int)> possibleNeighbours = new List<(int, int)>()
                {
                    (0, -1),
                    (1, 0),
                    (0, 1),
                    (-1, 0)
                };

                foreach (var pn in possibleNeighbours)
                {
                    Vertex v = Q.Find(v => (u.xIdx + pn.Item1 == v.xIdx) && (u.yIdx + pn.Item2 == v.yIdx));
                    if (v != null)
                    {
                        int alt = u.dist + v.weight;
                        if (alt < v.dist)
                        {
                            v.dist = alt;
                            v.prev = u;
                        }
                    }
                }
            }

            Stack<GridSquare> S = new Stack<GridSquare>();

            u = target;

            if (u.prev != null || u == source)
            {
                while (u != null)
                {
                    S.Push(_gridManager.gridSquares.Find(gs => (gs.xIdx == u.xIdx) && (gs.yIdx == u.yIdx)));
                    u = u.prev;
                }
            }

            Game1.Log("A total of " + (_gridManager.gridSquares.Count - Q.Count).ToString() + "/" + _gridManager.gridSquares.Count.ToString() + " were checked.");
            return S;
        }

        public void TestDijkstra()
        {
            int idx = Game1.random.Next(_gridManager.gridSquares.Count);
            GridSquare destination = _gridManager.gridSquares[idx];
            Debug.WriteLine("From [0, 0] to [" + destination.xIdx + ", " + destination.yIdx + "]", LogLevel.Info);
            Stack<GridSquare> sequence = Dijkstra(destination);
            while (sequence.Count > 0)
            {
                GridSquare step = sequence.Pop();
                Debug.WriteLine("[" + step.xIdx + ", " + step.yIdx + "]");
                _destinations.Enqueue(step.position);
            }
            Debug.WriteLine(_destinations.Count.ToString(), LogLevel.Debug);
        }
    }
}
