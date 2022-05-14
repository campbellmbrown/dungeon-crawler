using System;
using System.Collections.Generic;
using DungeonCrawler.GameStates.PlayingState.PathFinding;
using DungeonCrawler.Utility;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntityManager : IMyDrawable, IFrameTickable
    {
        IPlayer Player { get; }

        void AddEntity(IEntity entity);
        void CreateTempEnemies();
    }

    public class EntityManager : IEntityManager
    {
        readonly ILogManager _logManager;
        readonly IGridManager _gridManager;
        readonly IActionManager _actionManager;
        readonly IEntityFactory _entityFactory;

        public IPlayer Player { get; private set; }

        List<IEntity> _entities;

        public EntityManager(
            ILogManager logManager,
            IGridManager gridManager,
            IActionManager actionManager,
            IEntityFactory entityFactory)
        {
            _logManager = logManager;
            _gridManager = gridManager;
            _actionManager = actionManager;
            _entityFactory = entityFactory;

            IPathFinding dijkstra = new Dijkstra(_logManager, _gridManager.Floors);
            Player = entityFactory.CreatePlayer(_logManager, _gridManager, _actionManager, dijkstra, gridManager.StartingFloor);
            _entities = new List<IEntity>();
        }

        public void AddEntity(IEntity entity)
        {
            _entities.Add(entity);
        }

        public void CreateTempEnemies()
        {
            // Temporary, do not test this
            IPathFinding simpleMove = new SimpleMove(_gridManager.Floors);
            for (int idx = 0; idx < 10; idx++)
            {
                _entities.Add(
                    _entityFactory.CreateBotlin(_logManager, _gridManager, _actionManager, simpleMove, RNG.ChooseRandom(_gridManager.Floors))
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
