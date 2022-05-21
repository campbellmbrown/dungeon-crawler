using DungeonCrawler.GameStates.PlayingState.PathFinding;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IPlayer : IEntity
    {
    }

    public class Player : Entity, IPlayer
    {
        readonly IAnimationManager _animationManager;

        public Player(
            ILogManager logManager,
            IGridManager gridManager,
            IActionManager actionManager,
            IPathFinding pathFinding,
            IFloor floor,
            IAnimationManager animationManager) :
            base(logManager, gridManager, actionManager, pathFinding, floor)
        {
            _animationManager = animationManager;
            _animationManager.Play(AnimationId.PlayerIdleLeft);
        }

        public override void Draw(ISpriteBatchWrapper spriteBatch)
        {
            var alignment = new Vector2(0.5f, 0.9f);
            var adjustment = CalculatePositionAdjustment(alignment);
            _animationManager.Draw(spriteBatch, Position + adjustment, _gridManager.FindLayerDepth(Position.Y));
        }

        /// <summary>
        /// Calculates the position adjustment to align the bottom-center of the entity to the position specified.
        /// </summary>
        /// <param name="alignment">A Vector2 specifying the position to align to. This should be match the coordinates
        /// (i.e. X +ve towards the right of the screen, Y +ve towards the bottom of the screen) and should be from
        /// [0, 0] to [1, 1].</param>
        /// <returns>The position adjustment.</returns>
        Vector2 CalculatePositionAdjustment(Vector2 alignment)
        {
            var alignmentPos = alignment * GridSquare.GRID_SQUARE_SIZE;
            return alignmentPos - _animationManager.CurrentFrameRelativeBottomMiddle;
        }

        public override void FrameTick(IGameTimeWrapper gameTime)
        {
            _animationManager.FrameTick(gameTime);
            base.FrameTick(gameTime);
            if (_actionManager.ActionState != ActionState.Stopped && !PartakingInActionTick)
            {
                _actionManager.Stop();
            }
        }

        public override void SetDestination(IFloor destination)
        {
            base.SetDestination(destination);
            if (QueuedFloors.Count == 1 && QueuedFloors.Peek().Entity != null)
            {
                PartakingInActionTick = true;
                _origPosition = Floor.Position;
                _beenSwapped = true;

                var swapTarget = QueuedFloors.Dequeue().Entity;
                var swapTargetFloor = swapTarget.Floor;

                swapTarget.SwapWith(this);
                Floor = swapTargetFloor;
                Floor.Entity = this;
            }
        }
    }
}
