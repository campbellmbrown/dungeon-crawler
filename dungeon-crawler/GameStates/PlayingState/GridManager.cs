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

            RoomGenerator roomGenerator = new RoomGenerator();
            roomGenerator.GenerateFloor(this, _gridSquares);
        }

        /// <summary>
        /// Gets a random GridSquare from the list.
        /// </summary>
        /// <remarks>
        /// In the future this could have an input which specifies the type of GridSquare to receive.
        /// </remarks>
        /// <returns>A random GridSquare if successful, null if unsuccessful</returns>
        public GridSquare GetRandomGridSquare()
        {
            List<GridSquare> gridSquaresWithoutEntities = _gridSquares.Where(sq => !sq.hasEntity).ToList();
            if (gridSquaresWithoutEntities.Count > 0)
            {
                int idx = Game1.random.Next(0, gridSquaresWithoutEntities.Count);
                return gridSquaresWithoutEntities[idx];
            }
            else
            {
                return null;
                // TODO: raise a warning here.
            }
        }

        public GridSquare GetStartingTile()
        {
            return _gridSquares.Find(sq => sq.xIdx == 0 && sq.yIdx == 0);
        }

        /// <summary>
        /// Moves an entity by a certain (indexed) difference.
        /// </summary>
        /// <param name="entity">The entity to move.</param>
        /// <param name="deltaX">The desired change in index in the X axis.</param>
        /// <param name="deltaY">The desired change in index in the Y axis.</param>
        public MovementStatus MoveEntityDiagonally(Entity entity, int deltaX, int deltaY)
        {
            bool finished = false;
            int leftToMoveInX = Math.Abs(deltaX);
            int leftToMoveInY = Math.Abs(deltaY);

            // Result of this function.
            MovementStatus ret = MovementStatus.Success;
            // Result of the individual movements.
            MovementStatus xStatus = MovementStatus.Blocked;
            MovementStatus yStatus = MovementStatus.Blocked;

            while (!finished)
            {
                if (leftToMoveInX > 0)
                {
                    int moveInXBy = (deltaX > 0) ? 1 : -1;
                    xStatus = MoveEntityInStraightLine(entity, moveInXBy, 0);
                    if (xStatus == MovementStatus.Success) { leftToMoveInX--; }
                }

                if (leftToMoveInY > 0)
                {
                    int moveInYBy = (deltaY > 0) ? 1 : -1;
                    yStatus = MoveEntityInStraightLine(entity, 0, moveInYBy);
                    if (yStatus == MovementStatus.Success) { leftToMoveInY--; }
                }

                if (xStatus == MovementStatus.Blocked && yStatus == MovementStatus.Blocked)
                {
                    ret = MovementStatus.Blocked;
                    break;
                }

                if (leftToMoveInX == 0 && leftToMoveInY == 0)
                {
                    break;
                }
            }

            return ret;
        }

        public enum MovementStatus
        {
            Success,
            Blocked,
            DestinationNonExistant,
        }

        private MovementStatus MoveEntityInStraightLine(Entity entity, int deltaX, int deltaY)
        {
            GridSquare containingEntity = _gridSquares.Find(sq => sq.entity == entity);
            int desiredX = containingEntity.xIdx + deltaX;
            int desiredY = containingEntity.yIdx + deltaY;
            return MoveEntityFromTo(containingEntity, desiredX, desiredY);
        }

        /// <summary>
        /// Try to move an entity from a GridSquare to a new GridSquare at a certain index.
        /// </summary>
        /// <param name="containingEntity">The GridSquare containing the entity to move.</param>
        /// <param name="xIdx">The desired x index of the new location.</param>
        /// <param name="yIdx">The desired y index of the new location.</param>
        /// <returns>True if the GridSquare exists at the index, false otherwise.</returns>
        public MovementStatus MoveEntityFromTo(GridSquare containingEntity, int xIdx, int yIdx)
        {
            // TODO: should also check if there is something that can block the entity, such as another entity or a wall.
            // Or maybe there can be 'swappable' entities, that swap position with the player.
            // In that case, we should call gridSquare.swapEntity(gridSquareContainingEntity) instead of the logic below.
            GridSquare gridSquare = _gridSquares.Find(sq => sq.xIdx == xIdx && sq.yIdx == yIdx);
            if (gridSquare != null)
            {
                gridSquare.entity = containingEntity.entity;
                containingEntity.entity = null;
                return MovementStatus.Success;
            }
            else
            {
                // TODO: this should actually be MovementStatus.DestinationNonExistant, but we don't have walls yet.
                return MovementStatus.Blocked;
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
