using System.Collections.Generic;
using System.Linq;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

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
        private const int MAX_FLOORS_PER_PATHFIND = 15;

        protected DestinationState destinationState;
        private Vector2 _destination;
        public Queue<Floor> queuedFloors { get; }
        public Vector2 position { get; private set; }
        private Floor _floor;

        public Entity(GridManager gridManager, Floor floor)
        {
            _gridManager = gridManager;
            _pathFinding = new Dijkstra(_gridManager);
            queuedFloors = new Queue<Floor>();
            _floor = floor;
            _floor.entity = this;

            position = _floor.position;
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
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, 16, 16), Color.Yellow, 1f, FindLayerDepth());
        }

        // Temporary - create proper function (with an interface?) when drawing an actual sprite.
        public float FindLayerDepth()
        {
            return DrawOrder.FOREGROUND_CONTENT_BOTTOM + ((position.Y - _gridManager.minY) / (_gridManager.maxY - _gridManager.minY) * (DrawOrder.FOREGROUND_CONTENT_TOP - DrawOrder.FOREGROUND_CONTENT_BOTTOM));
        }

        public virtual void ActionTick()
        {
            Game1.Log("Entity " + GetHashCode().ToString() + " ActionTick triggered.", LogLevel.Debug);
            if (queuedFloors.Count > 0)
            {
                _floor.entity = null;
                _floor = queuedFloors.Dequeue();
                _floor.entity = this;
                _destination = _floor.position;
                destinationState = DestinationState.OffDestination;
            }
        }

        public void SetDestination(Floor destination)
        {
            Stack<Floor> sequence = _pathFinding.FindShortestPath(_floor, destination);
            if (sequence.Count() > MAX_FLOORS_PER_PATHFIND)
            {
                Game1.Log("The destination is too far away from the source.", LogLevel.Warning);
            }
            else
            {
                while (sequence.Count > 0)
                {
                    Floor step = sequence.Pop();
                    queuedFloors.Enqueue(step);
                }
            }
        }

        public bool Busy()
        {
            return destinationState != DestinationState.AtDestination;
        }
    }
}
