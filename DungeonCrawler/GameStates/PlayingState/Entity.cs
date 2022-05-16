using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.GameStates.PlayingState.PathFinding;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntity : IMyDrawable, IActionTickable, IFrameTickable
    {
        const int MAX_FLOORS_PER_PATHFIND = 15;

        void SetDestination(IFloor destination);
        void SwapWith(IEntity entity);
        void Attack(IFloor floor);

        Queue<IFloor> QueuedFloors { get; set; }
        bool PartakingInActionTick { get; }
        Vector2 Position { get; }
        IFloor Floor { get; }
    }

    public class Entity : IEntity
    {
        protected readonly ILogManager _logManager;
        protected readonly IGridManager _gridManager;
        protected readonly IActionManager _actionManager;
        readonly IPathFinding _pathFinding;

        public Queue<IFloor> QueuedFloors { get; set; } = new Queue<IFloor>();
        public bool PartakingInActionTick { get; protected set; } = false;
        public Vector2 Position { get; private set; }
        public IFloor Floor { get; protected set; }

        protected Vector2 _origPosition;
        protected Vector2 _attackDirection;

        // Flags
        protected bool _beenSwapped = false;
        bool _attacking = false;
        bool _goingToAttack = false;

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
            Floor = floor;
            Floor.Entity = this;

            Position = Floor.Position;
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
                if (_attacking)
                {
                    // Move in a quadratic pattern
                    var displacement = CalculateAttackDisplacement(_actionManager.DecimalComplete);
                    Position = _origPosition + _attackDirection * displacement;
                }
                else
                {
                    Position = _origPosition + _actionManager.DecimalComplete * (Floor.Position - _origPosition);
                }
            }
            if (_actionManager.ActionState == ActionState.Restarting)
            {
                ActionTick();
            }
        }

        float CalculateAttackDisplacement(float decimalComplete)
        {
            return (float)(GridSquare.GRID_SQUARE_SIZE * 2 * (-Math.Pow(decimalComplete, 2) + decimalComplete));
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

        void ActionTickSwapping()
        {
            PartakingInActionTick = true;
            QueuedFloors.Clear();
        }

        void ActionTickAttacking()
        {
            PartakingInActionTick = true;
            _origPosition = Floor.Position;
            _attacking = true;
        }

        void ActionTickMoving()
        {
            // Encountered another entity, don't move any more
            if (QueuedFloors.Peek().Entity != null)
            {
                QueuedFloors.Clear();
            }
            else
            {
                PartakingInActionTick = true;
                _origPosition = Floor.Position;
                Floor.Entity = null;
                Floor = QueuedFloors.Dequeue();
                Floor.Entity = this;
            }
        }

        public virtual void ActionTick()
        {
            _attacking = false;
            PartakingInActionTick = false;

            if (_beenSwapped)
            {
                ActionTickSwapping();
            }
            else if (_goingToAttack)
            {
                ActionTickAttacking();
            }
            else if (QueuedFloors.Count > 0)
            {
                ActionTickMoving();
            }

            // Reset flags
            _beenSwapped = false;
            _goingToAttack = false;
        }

        /// <summary>
        /// Swaps this entity with another entity.
        /// </summary>
        /// <remarks>
        /// Steps that this should be called:
        /// - Entity A should set it's original position
        /// - Entity A should store the floor of this entity (entity B)
        /// - Entity A should call SwapWith(entityA) to allow entity B to update it's floor
        /// - Entity A should then update it's flored based on the stored floors
        /// <param name="entity">The entity to swap with.</param>
        public void SwapWith(IEntity entity)
        {
            _origPosition = Floor.Position;
            Floor = entity.Floor;
            Floor.Entity = this;
            _beenSwapped = true;
        }

        public void Attack(IFloor floor)
        {
            _goingToAttack = true;
            _attackDirection = new Vector2(floor.XIdx - Floor.XIdx, floor.YIdx - Floor.YIdx);
            if (_attackDirection.Length() != 1)
            {
                throw new InvalidOperationException("The attacking entity wasn't next to the victim.");
            }
        }

        public virtual void SetDestination(IFloor destination)
        {
            if (QueuedFloors.Count > 0)
            {
                throw new InvalidOperationException("Cannot add floors to the queue that's already filled.");
            }

            var sequence = _pathFinding.FindShortestPath(Floor, destination);
            if (sequence.Count() > IEntity.MAX_FLOORS_PER_PATHFIND)
            {
                _logManager.Log("The destination is too far away from the source.", LogLevel.Warning);
                return;
            }
            while (sequence.Count > 0)
            {
                var step = sequence.Pop();
                QueuedFloors.Enqueue(step);
            }
        }
    }
}
