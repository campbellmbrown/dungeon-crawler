using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DungeonCrawler.GameStates.PlayingState
{
    public enum DestinationState
    {
        AtDestination,
        OffDestination,
    }

    public interface IEntity : IMyDrawable, IActionTickable, IFrameTickable, ICouldBeBusy
    {
        const int MAX_FLOORS_PER_PATHFIND = 15;

        void SetDestination(IFloor destination);

        Queue<IFloor> QueuedFloors { get; set; }
        DestinationState DestinationState { get; }
    }

    public class Entity : IEntity
    {
        readonly IGridManager _gridManager;
        readonly ILogManager _logManager;
        readonly IPathFinding _pathFinding;
        IFloor _floor;

        const float MOVEMENT_SPEED = 80f; // 80 pixels/second.
        const float DESTINATION_HYSTERESIS = 0.5f; // How many pixels away for the destination to be considered reached.

        Vector2 _destination;
        public Queue<IFloor> QueuedFloors { get; set; } = new Queue<IFloor>();
        public Vector2 Position { get; private set; }
        public DestinationState DestinationState { get; set; } = DestinationState.AtDestination;

        public Entity(
            ILogManager logManager,
            IGridManager gridManager,
            IPathFinding pathfinding,
            IFloor floor)
        {
            _logManager = logManager;
            _gridManager = gridManager;
            _pathFinding = pathfinding;
            _floor = floor;
            _floor.Entity = this;

            Position = _floor.Position;
        }

        public const int FRAME_TICKS_PER_STEP = 10;

        public virtual void FrameTick(GameTime gameTime)
        {
            // If not at destination
            if (DestinationState == DestinationState.OffDestination)
            {
                Vector2 diff = _destination - Position;
                if (diff.Length() > 0) // Cannot normalize a vector of length 0
                {
                    Position += Vector2.Normalize(diff) * MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if ((_destination - Position).Length() < DESTINATION_HYSTERESIS)
                {
                    Position = _destination;
                    DestinationState = DestinationState.AtDestination;
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
            _logManager.Log("Entity " + GetHashCode().ToString() + " ActionTick triggered.", LogLevel.Debug);
            if (QueuedFloors.Count > 0)
            {
                _floor.Entity = null;
                _floor = QueuedFloors.Dequeue();
                _floor.Entity = this;
                _destination = _floor.Position;
                DestinationState = DestinationState.OffDestination;
            }
        }

        public void SetDestination(IFloor destination)
        {
            var sequence = _pathFinding.FindShortestPath(_floor, destination);
            if (sequence.Count() > IEntity.MAX_FLOORS_PER_PATHFIND)
            {
                _logManager.Log("The destination is too far away from the source.", LogLevel.Warning);
            }
            else
            {
                while (sequence.Count > 0)
                {
                    var step = sequence.Pop();
                    QueuedFloors.Enqueue(step);
                }
            }
        }

        public bool IsBusy()
        {
            return DestinationState != DestinationState.AtDestination;
        }
    }
}
