using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class GridManager
    {
        private List<GridSquare> _gridSquares;

        public GridManager()
        {
            _gridSquares = new List<GridSquare>();
            _gridSquares.Add(new GridSquare(0, 0));
            _gridSquares.Add(new GridSquare(0, 1));
            _gridSquares.Add(new GridSquare(0, 2));
            _gridSquares.Add(new GridSquare(1, 0));
            _gridSquares.Add(new GridSquare(1, 1));
            _gridSquares.Add(new GridSquare(1, 2));
            _gridSquares.Add(new GridSquare(2, 0));
            _gridSquares.Add(new GridSquare(2, 1));
            _gridSquares.Add(new GridSquare(2, 2));
        }

        /// <summary>
        /// Moves an entity to a random spot in the grid.
        /// Main use would be on creation of the entity where a random position needs to be assigned.
        /// </summary>
        /// <param name="entity">The entity to move to a random spot.</param>
        public void MoveToRandom(Entity entity)
        {
            List<GridSquare> gridSquaresWithoutEntities = _gridSquares.Where(sq => !sq.hasEntity).ToList();
            if (gridSquaresWithoutEntities.Count > 0)
            {
                int idx = Game1.random.Next(gridSquaresWithoutEntities.Count);
                gridSquaresWithoutEntities[idx].entity = entity;
            }
            else
            {
                // TODO: raise a warning here.
            }
        }

        public void MoveEntity(Entity entity, int deltaX, int deltaY)
        {
            GridSquare containingEntity = _gridSquares.Find(sq => sq.entity == entity);

            for (int xIdx = 0; xIdx < Math.Abs(deltaX); xIdx++)
            {
                GridSquare inXDirection = _gridSquares.Find(sq => sq.xIdx == (containingEntity.xIdx + Math.Sign(deltaX)) && sq.yIdx == containingEntity.yIdx);
                if (inXDirection != null)
                {
                    inXDirection.entity = containingEntity.entity;
                    containingEntity.entity = null;
                    containingEntity = inXDirection;
                }
                else
                {
                    break;
                }
            }

            for (int yIdx = 0; yIdx < Math.Abs(deltaY); yIdx++)
            {
                GridSquare inYDirection = _gridSquares.Find(sq => sq.yIdx == (containingEntity.yIdx + Math.Sign(deltaY)) && sq.xIdx == containingEntity.xIdx);
                if (inYDirection != null)
                {
                    inYDirection.entity = containingEntity.entity;
                    containingEntity.entity = null;
                    containingEntity = inYDirection;
                }
                else
                {
                    break;
                }
            }
        }

        public void FrameTick(GameTime gameTime)
        {
            foreach (var gridSquare in _gridSquares)
            {
                gridSquare.FrameTick(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var gridSquare in _gridSquares)
            {
                gridSquare.Draw(spriteBatch);
            }
        }
    }
}
