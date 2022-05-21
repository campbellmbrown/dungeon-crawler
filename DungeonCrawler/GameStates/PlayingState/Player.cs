using DungeonCrawler.GameStates.PlayingState.PathFinding;
using DungeonCrawler.Visual;

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
            IAnimationList animationList,
            IAnimationManager animationManager) :
            base(logManager, gridManager, actionManager, pathFinding, floor)
        {
            _animationManager = animationManager;
            _animationManager.Play(animationList.Get(AnimationId.PlayerIdleLeft));
        }

        public override void Draw(ISpriteBatchWrapper spriteBatch)
        {
            // TODO update draw order.
            _animationManager.Draw(spriteBatch, Position, _gridManager.FindLayerDepth(Position.Y));
            base.Draw(spriteBatch);
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
