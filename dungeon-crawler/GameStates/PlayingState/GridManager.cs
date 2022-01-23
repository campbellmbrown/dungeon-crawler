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

        public List<Floor> floors { get; set; }
        public List<Wall> walls { get; set; }

        public int minY { get; set; }
        public int maxY { get; set; }

        public GridManager(PlayingState playingState, ClickManager clickManager)
        {
            _playingState = playingState;
            floors = new List<Floor>();
            walls = new List<Wall>();
            _drawableGridSquares = new List<GridSquare>();

            LevelGenerator levelGenerator = new LevelGenerator(this, clickManager);
            levelGenerator.GenerateLevel();
            minY = (int)Math.Min(floors.Min(fl => fl.position.Y), walls.Min(wa => wa.position.Y));
            maxY = (int)Math.Max(floors.Max(fl => fl.position.Y), walls.Max(wa => wa.position.Y));
        }

        public bool DoesGridSquareExistAt(int xIdx, int yIdx)
        {
            bool exists = false;
            exists |= floors.Any(floor => floor.xIdx == xIdx && floor.yIdx == yIdx);
            exists |= walls.Any(wall => wall.xIdx == xIdx && wall.yIdx == yIdx);
            return exists;
        }

        public Floor GetStartingFloor()
        {
            return floors.Find(sq => sq.xIdx == STARTING_X && sq.yIdx == STARTING_Y);
        }

        public void FrameTick(GameTime gameTime)
        {
            foreach (var floor in floors)
            {
                floor.FrameTick(gameTime);
            }
            foreach (var wall in walls)
            {
                wall.FrameTick(gameTime);
            }
        }

        public void ActionTick()
        {
            UpdateVisibilityStates();
        }

        public void UpdateVisibilityStates()
        {
            // Any GridSquare within the range of the player is visible.
            GridSquare containingPlayer = floors.Find(sq => sq.entity is Player);
            foreach (var floor in floors)
            {
                floor.ActionTick(containingPlayer.xIdx, containingPlayer.yIdx, 4);
            }
            foreach (var wall in walls)
            {
                wall.ActionTick(containingPlayer.xIdx, containingPlayer.yIdx, 4);
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

        public void SetPlayerDestination(Floor floor)
        {
            if (_drawableGridSquares.Contains(floor))
            {
                _playingState.SetPlayerDestination(floor);
            }
            else
            {
                Game1.Log("The destination isn't visible.", LogLevel.Warning);
            }
        }

        public bool Busy()
        {
            bool busy = false;
            foreach (var floor in floors)
            {
                busy |= floor.Busy();
            }
            return busy;
        }

        public float GetNumberDrawableGridSquares()
        {
            return _drawableGridSquares.Count;
        }
    }
}
