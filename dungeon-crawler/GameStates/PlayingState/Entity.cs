using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntity : IMyDrawable, IActionTickable, IFrameTickable
    {
        void SetDestination(Floor destination);
    }

    public class Entity : IEntity
    {
        protected enum DestinationState
        {
            AtDestination,
            OffDestination,
        }

        readonly GridManager _gridManager;
        readonly Dijkstra _pathFinding;

        const float MOVEMENT_SPEED = 80f; // 80 pixels/second.
        const float DESTINATION_HYSTERESIS = 0.5f; // How many pixels away for the destination to be considered reached.
        const int MAX_FLOORS_PER_PATHFIND = 15;

        protected DestinationState _destinationState;
        Vector2 _destination;
        public Queue<Floor> QueuedFloors { get; }
        public Vector2 Position { get; private set; }
        Floor _floor;

        public Entity(GridManager gridManager, Floor floor)
        {
            _gridManager = gridManager;
            _pathFinding = new Dijkstra(_gridManager);
            QueuedFloors = new Queue<Floor>();
            _floor = floor;
            _floor.Entity = this;

            Position = _floor.Position;
            _destinationState = DestinationState.AtDestination;
        }

        public const int FRAME_TICKS_PER_STEP = 10;

        public virtual void FrameTick(GameTime gameTime)
        {
            // If not at destination
            if (_destinationState == DestinationState.OffDestination)
            {
                Vector2 diff = _destination - Position;
                if (diff.Length() > 0) // Cannot normalize a vector of length 0
                {
                    Position += Vector2.Normalize(diff) * MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if ((_destination - Position).Length() < DESTINATION_HYSTERESIS)
                {
                    Position = _destination;
                    _destinationState = DestinationState.AtDestination;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(Position.X, Position.Y, 16, 16), Color.Yellow, 1f, FindLayerDepth());
        }

        // Temporary - create proper function (with an interface?) when drawing an actual sprite.
        public float FindLayerDepth()
        {
            return DrawOrder.FOREGROUND_CONTENT_BOTTOM + ((Position.Y - _gridManager.MinY) / (_gridManager.MaxY - _gridManager.MinY) * (DrawOrder.FOREGROUND_CONTENT_TOP - DrawOrder.FOREGROUND_CONTENT_BOTTOM));
        }

        public virtual void ActionTick()
        {
            Game1.Log("Entity " + GetHashCode().ToString() + " ActionTick triggered.", LogLevel.Debug);
            if (QueuedFloors.Count > 0)
            {
                _floor.Entity = null;
                _floor = QueuedFloors.Dequeue();
                _floor.Entity = this;
                _destination = _floor.Position;
                _destinationState = DestinationState.OffDestination;
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
                    QueuedFloors.Enqueue(step);
                }
            }
        }

        public bool Busy()
        {
            return _destinationState != DestinationState.AtDestination;
        }
    }
}
