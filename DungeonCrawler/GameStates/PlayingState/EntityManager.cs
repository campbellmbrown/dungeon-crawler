using System.Collections.Generic;

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
            Player = new Player(logManager, gridManager, actionManager, dijkstra, gridManager.GetStartingFloor());
            _entities = new List<IEntity>();
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
