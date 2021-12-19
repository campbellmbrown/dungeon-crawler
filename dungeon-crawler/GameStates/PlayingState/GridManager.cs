using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class GridManager
    {
        private readonly PlayingState _playingState;
        private readonly List<GridSquare> _drawableGridSquares;

        public const int STARTING_X = 0;
        public const int STARTING_Y = 0;

        public List<GridSquare> gridSquares { get; set; }

        public GridManager(PlayingState playingState, ClickManager clickManager)
        {
            _playingState = playingState;
            gridSquares = new List<GridSquare>();
            _drawableGridSquares = new List<GridSquare>();

            LevelGenerator levelGenerator = new LevelGenerator(this, gridSquares, clickManager);
            levelGenerator.GenerateLevel();
        }

        public GridSquare GetStartingTile()
        {
            return gridSquares.Find(sq => sq.xIdx == STARTING_X && sq.yIdx == STARTING_Y);
        }

        public void FrameTick(GameTime gameTime)
        {
            foreach (var gridSquare in gridSquares)
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
            GridSquare containingPlayer = gridSquares.Find(sq => sq.entity is Player);
            foreach (var gridSquare in gridSquares)
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

        public void SetPlayerDestination(GridSquare gridSquare)
        {
            if (_drawableGridSquares.Contains(gridSquare))
            {
                _playingState.SetPlayerDestination(gridSquare);
            }
            else
            {
                Game1.Log("The destination isn't visible.", LogLevel.Warning);
            }
        }

        public bool Busy()
        {
            bool busy = false;
            foreach (var gs in gridSquares)
            {
                busy |= gs.Busy();
            }
            return busy;
        }
    }
}
