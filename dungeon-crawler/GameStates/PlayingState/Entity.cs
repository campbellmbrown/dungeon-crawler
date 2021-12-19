using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Entity
    {
        protected enum DestinationState
        {
            AtDestination,
            OffDestination,
        }

        private readonly GridManager _gridManager;

        private const float MOVEMENT_SPEED = 48f; // 48 pixels/second.
        private const float DESTINATION_HYSTERESIS = 0.5f; // How many pixels away for the destination to be considered reached.
        private const int MAX_GRIDSQUARES_PER_PATHFIND = 10;

        protected DestinationState destinationState;
        private Vector2 _destination;
        public Queue<GridSquare> queuedGridSquares { get; }
        public Vector2 position { get; private set; }
        private GridSquare _gridSquare;

        public Entity(GridManager gridManager, GridSquare gridSquare)
        {
            _gridManager = gridManager;
            queuedGridSquares = new Queue<GridSquare>();
            _gridSquare = gridSquare;
            gridSquare.entity = this;

            position = _gridSquare.position;
            destinationState = DestinationState.AtDestination;
        }

        public const int FRAME_TICKS_PER_STEP = 10;

        public virtual void FrameTick(GameTime gameTime)
        {
            // If not at destination
            if (destinationState == DestinationState.OffDestination)
            {
                Vector2 diff = _destination - position;
                if (diff.Length() > 0) // Cannot normalize a vector of length 0
                {
                    position += Vector2.Normalize(diff) * MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if ((_destination - position).Length() < DESTINATION_HYSTERESIS)
                {
                    position = _destination;
                    destinationState = DestinationState.AtDestination;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, 4, 4), Color.Yellow);
        }

        public virtual void ActionTick()
        {
            Game1.Log("Entity " + GetHashCode().ToString() + " ActionTick triggered.", LogLevel.Debug);
            if (queuedGridSquares.Count > 0)
            {
                _gridSquare.entity = null;
                _gridSquare = queuedGridSquares.Dequeue();
                _gridSquare.entity = this;
                _destination = _gridSquare.position;
                destinationState = DestinationState.OffDestination;
            }
        }

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

        private Stack<GridSquare> Dijkstra(GridSquare orig, GridSquare dest)
        {
            // See https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm for implementation.

            List<Vertex> Q = new List<Vertex>();

            foreach (var v in _gridManager.gridSquares)
            {
                Q.Add(new Vertex(v.xIdx, v.yIdx));
            }
            Vertex target = Q.Find(v => (v.xIdx == dest.xIdx) && (v.yIdx == dest.yIdx));
            Vertex source = Q.Find(v => (v.xIdx == orig.xIdx) && (v.yIdx == orig.yIdx));
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

                List<(int, int)> possibleNeighbours = new List<(int, int)>() { (0, -1), (1, 0), (0, 1), (-1, 0) };

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
            // Remove the first one - this should be the source.
            S.Pop();
            return S;
        }

        public void SetDestination(GridSquare destination)
        {
            Stack<GridSquare> sequence = Dijkstra(_gridSquare, destination);
            if (sequence.Count() > MAX_GRIDSQUARES_PER_PATHFIND)
            {
                Game1.Log("The destination is too far away from the source.", LogLevel.Warning);
            }
            else
            {
                while (sequence.Count > 0)
                {
                    GridSquare step = sequence.Pop();
                    queuedGridSquares.Enqueue(step);
                }
            }
        }

        public bool Busy()
        {
            return destinationState != DestinationState.AtDestination;
        }
    }
}
