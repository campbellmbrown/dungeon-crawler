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

        private List<GridSquare> _drawableGridSquares;

        public GridManager()
        {
            _gridSquares = new List<GridSquare>();
            _drawableGridSquares = new List<GridSquare>();
            // TODO: remove
            for (int xIdx = 0; xIdx < 30; xIdx++)
            {
                for (int yIdx = 0; yIdx < 30; yIdx++)
                {
                    if (Game1.random.Next(0, 101) > 30)
                    {
                        _gridSquares.Add(new GridSquare(this, xIdx, yIdx));
                    }
                }
            }
        }


        /// <summary>
        /// Moves an entity to a random spot in the grid.
        /// Main use would be on creation of the entity where a random position needs to be assigned.
        /// </summary>
        /// <remarks>
        /// Maybe the move entity should be a function in the PlayingState instead of the GridManager.
        /// </remarks>
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

        /// <summary>
        /// Moves an entity by a certain (indexed) difference.
        /// </summary>
        /// <param name="entity">The entity to move.</param>
        /// <param name="deltaX">The desired change in position in the X axis.</param>
        /// <param name="deltaY">The desired change in position in the Y axis.</param>
        public void MoveEntityByDifference(Entity entity, int deltaX, int deltaY)
        {
            // Move in the X direction first.
            for (int xIdx = 0; xIdx < Math.Abs(deltaX); xIdx++)
            {
                GridSquare containingEntity = _gridSquares.Find(sq => sq.entity == entity);
                int desiredX = containingEntity.xIdx + Math.Sign(deltaX);
                int desiredY = containingEntity.yIdx;

                if (!MoveEntityFromTo(containingEntity, desiredX, desiredY))
                {
                    break;
                }
            }

            // Then move in the Y direction.
            for (int yIdx = 0; yIdx < Math.Abs(deltaY); yIdx++)
            {
                GridSquare containingEntity = _gridSquares.Find(sq => sq.entity == entity);
                int desiredX = containingEntity.xIdx;
                int desiredY = containingEntity.yIdx + Math.Sign(deltaY);

                if (!MoveEntityFromTo(containingEntity, desiredX, desiredY))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Try to move an entity from a GridSquare to a new GridSquare at a certain index.
        /// </summary>
        /// <param name="containingEntity">The GridSquare containing the entity to move.</param>
        /// <param name="xIdx">The desired x index of the new location.</param>
        /// <param name="yIdx">The desired y index of the new location.</param>
        /// <returns>True if the GridSquare exists at the index, false otherwise.</returns>
        public bool MoveEntityFromTo(GridSquare containingEntity, int xIdx, int yIdx)
        {
            // TODO: should also check if there is an entity in the square.
            // Or maybe there can be 'swappable' entities, that swap position with the player.
            // In that case, we should call gridSquare.swapEntity(gridSquareContainingEntity) instead of the logic below.
            GridSquare gridSquare = _gridSquares.Find(sq => sq.xIdx == xIdx && sq.yIdx == yIdx);
            if (gridSquare != null)
            {
                gridSquare.entity = containingEntity.entity;
                containingEntity.entity = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void FrameTick(GameTime gameTime)
        {
            foreach (var gridSquare in _gridSquares)
            {
                gridSquare.FrameTick(gameTime);
            }
        }

        public void ActionTick()
        {
            UpdateVisibilityStates();
        }

        public void UpdateVisibilityStates()
        {
            // Any gridSquare within the range of the player is visible.
            GridSquare containingPlayer = _gridSquares.Find(sq => sq.entity is Player);
            foreach (var gridSquare in _gridSquares)
            {
                gridSquare.ActionTick(containingPlayer.xIdx, containingPlayer.yIdx, 4);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var gridSquare in _drawableGridSquares)
            {
                gridSquare.Draw(spriteBatch);
            }
        }

        public void AddToDrawables(GridSquare gridSquare)
        {
            _drawableGridSquares.Add(gridSquare);
        }

        public void RemoveFromDrawables(GridSquare gridSquare)
        {
            _drawableGridSquares.Remove(gridSquare);
        }
    }
}
