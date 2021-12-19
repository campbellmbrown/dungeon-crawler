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
        private readonly Dijkstra _pathFinding;

        private const float MOVEMENT_SPEED = 80f; // 80 pixels/second.
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
            _pathFinding = new Dijkstra(_gridManager);
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

        public void SetDestination(GridSquare destination)
        {
            Stack<GridSquare> sequence = _pathFinding.FindShortestPath(_gridSquare, destination);
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
