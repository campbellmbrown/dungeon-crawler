using System.Collections.Generic;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IEntityManager : IMyDrawable, IActionTickable, IFrameTickable, ICouldBeBusy
    {
        Player Player { get; }
    }

    public class EntityManager : IEntityManager
    {
        public Player Player { get; private set; }

        List<IEntity> _entities;

        public EntityManager(GridManager gridManager)
        {
            Player = new Player(gridManager, this, gridManager.GetStartingFloor());
            _entities = new List<IEntity>();
        }

        public void FrameTick(GameTime gameTime)
        {
            Player.PriorityFrameTick(gameTime);
            foreach (var entity in _entities)
            {
                entity.ActionTick();
            }
        }

        public void ActionTick()
        {
            Player.PriorityActionTick();
            foreach (var entity in _entities)
            {
                entity.ActionTick();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Player.Draw(spriteBatch);
            foreach (var entity in _entities)
            {
                entity.Draw(spriteBatch);
            }
        }

        public bool IsBusy()
        {
            if (Player.IsBusy())
            {
                return true;
            }
            foreach (var entity in _entities)
            {
                if (entity.IsBusy())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
