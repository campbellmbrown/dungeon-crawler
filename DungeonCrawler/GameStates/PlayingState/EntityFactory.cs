using DungeonCrawler.GameStates.PlayingState.PathFinding;
using DungeonCrawler.Visual;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntityFactory
    {
        IPlayer CreatePlayer(IPathFinding pathFinding, IFloor floor);
        IBotlin CreateBotlin(IPathFinding pathFinding, IFloor floor);
    }

    public class EntityFactory : IEntityFactory
    {
        readonly ILogManager _logManager;
        readonly IGridManager _gridManager;
        readonly IActionManager _actionManager;
        readonly IAnimationList _animationList;

        public EntityFactory(
            ILogManager logManager,
            IGridManager gridManager,
            IActionManager actionManager,
            IAnimationList animationList)
        {
            _logManager = logManager;
            _gridManager = gridManager;
            _actionManager = actionManager;
            _animationList = animationList;
        }

        public IPlayer CreatePlayer(IPathFinding pathFinding, IFloor floor)
        {
            var animationManager = new AnimationManager(_animationList.Get(AnimationId.PlayerIdleLeft));
            return new Player(
                _logManager,
                _gridManager,
                _actionManager,
                pathFinding,
                floor,
                _animationList,
                animationManager);
        }

        public IBotlin CreateBotlin(IPathFinding pathFinding, IFloor floor)
        {
            return new Botlin(_logManager, _gridManager, _actionManager, pathFinding, floor);
        }
    }
}
