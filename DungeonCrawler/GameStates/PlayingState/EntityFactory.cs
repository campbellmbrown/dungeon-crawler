using DungeonCrawler.GameStates.PlayingState.PathFinding;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntityFactory
    {
        IPlayer CreatePlayer(ILogManager logManager, IGridManager gridManager, IActionManager actionManager, IPathFinding pathFinding, IFloor floor);
        IBotlin CreateBotlin(ILogManager logManager, IGridManager gridManager, IActionManager actionManager, IPathFinding pathFinding, IFloor floor);
    }

    public class EntityFactory : IEntityFactory
    {
        public IPlayer CreatePlayer(ILogManager logManager, IGridManager gridManager, IActionManager actionManager, IPathFinding pathFinding, IFloor floor)
        {
            return new Player(logManager, gridManager, actionManager, pathFinding, floor);
        }

        public IBotlin CreateBotlin(ILogManager logManager, IGridManager gridManager, IActionManager actionManager, IPathFinding pathFinding, IFloor floor)
        {
            return new Botlin(logManager, gridManager, actionManager, pathFinding, floor);
        }
    }
}
