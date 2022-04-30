using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntity : IMyDrawable, IActionTickable, IFrameTickable
    {
        const int MAX_FLOORS_PER_PATHFIND = 15;

        void SetDestination(IFloor destination);

        Queue<IFloor> QueuedFloors { get; set; }
        bool PartakingInActionTick { get; }
        Vector2 Position { get; }
    }

    public class Entity : IEntity
    {
        readonly ILogManager _logManager;
        readonly IGridManager _gridManager;
        protected readonly IActionManager _actionManager;
        readonly IPathFinding _pathFinding;

        public Queue<IFloor> QueuedFloors { get; set; } = new Queue<IFloor>();
        public bool PartakingInActionTick { get; private set; } = false;
        public Vector2 Position { get; private set; }

        IFloor _floor;
        Vector2 _origPosition;

        public Entity(
            ILogManager logManager,
            IGridManager gridManager,
            IActionManager actionManager,
            IPathFinding pathfinding,
            IFloor floor)
        {
            _logManager = logManager;
            _gridManager = gridManager;
            _actionManager = actionManager;
            _pathFinding = pathfinding;
            _floor = floor;
            _floor.Entity = this;

            Position = _floor.Position;
        }

        public virtual void FrameTick(IGameTimeWrapper gameTime)
        {
            if (_actionManager.ActionState == ActionState.Stopped)
            {
                return;
            }
            if (_actionManager.ActionState == ActionState.Starting)
            {
                ActionTick();
            }
            if (PartakingInActionTick)
            {
                Position = _origPosition + _actionManager.DecimalComplete * (_floor.Position - _origPosition);
            }
            if (_actionManager.ActionState == ActionState.Restarting)
            {
                ActionTick();
            }
        }

        public virtual void Draw(ISpriteBatchWrapper spriteBatch)
        {
            spriteBatch.SpriteBatch.DrawRectangle(new RectangleF(Position.X, Position.Y, 16, 16), Color.Yellow, 1f, FindLayerDepth());
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
                PartakingInActionTick = true;
                _origPosition = _floor.Position;
                _floor.Entity = null;
                _floor = QueuedFloors.Dequeue();
                _floor.Entity = this;
            }
            else
            {
                PartakingInActionTick = false;
            }
        }

        public void SetDestination(IFloor destination)
        {
            if (QueuedFloors.Count > 0)
            {
                throw new InvalidOperationException("Cannot add floors to the queue that's already filled.");
            }

            var sequence = _pathFinding.FindShortestPath(_floor, destination);
            if (sequence.Count() > IEntity.MAX_FLOORS_PER_PATHFIND)
            {
                _logManager.Log("The destination is too far away from the source.", LogLevel.Warning);
                return 0;
            }
            while (sequence.Count > 0)
            {
                var step = sequence.Pop();
                QueuedFloors.Enqueue(step);
            }
            return QueuedFloors.Count;
        }
    }
}
