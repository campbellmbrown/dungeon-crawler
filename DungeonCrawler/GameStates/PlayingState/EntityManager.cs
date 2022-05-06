using System.Collections.Generic;
using DungeonCrawler.GameStates.PlayingState.PathFinding;
using DungeonCrawler.Utility;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntityManager : IMyDrawable, IFrameTickable
    {
        Player Player { get; }
    }

    public class EntityManager : IEntityManager
    {
        public Player Player { get; private set; }

        List<IEntity> _entities;

        public EntityManager(ILogManager logManager, GridManager gridManager, IActionManager actionManager)
        {
            IPathFinding dijkstra = new Dijkstra(logManager, gridManager.Floors);
            IPathFinding simpleMove = new SimpleMove(gridManager.Floors);
            Player = new Player(logManager, gridManager, actionManager, dijkstra, gridManager.GetStartingFloor());
            _entities = new List<IEntity>();

            // Temporary
            for (int idx = 0; idx < 2; idx++)
            {
                _entities.Add(
                    new Botlin(logManager, gridManager, actionManager, simpleMove, RNG.ChooseRandom(gridManager.Floors))
                );
            }
        }

        public void FrameTick(IGameTimeWrapper gameTime)
        {
            Player.FrameTick(gameTime);
            foreach (var entity in _entities)
            {
                entity.FrameTick(gameTime);
            }
        }

        public void Draw(ISpriteBatchWrapper spriteBatch)
        {
            Player.Draw(spriteBatch);
            foreach (var entity in _entities)
            {
                entity.Draw(spriteBatch);
            }
        }
    }
}
