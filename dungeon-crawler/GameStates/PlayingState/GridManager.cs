using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class GridManager
    {
        private readonly PlayingState _playingState;
        private readonly List<GridSquare> _drawableGridSquares;

        public const int STARTING_X = 0;
        public const int STARTING_Y = 0;
        public const int VIEW_RANGE = 6;

        public List<Floor> Floors { get; set; }
        public List<Wall> Walls { get; set; }

        public int MinY { get; set; }
        public int MaxY { get; set; }

        public GridManager(PlayingState playingState, ClickManager clickManager)
        {
            _playingState = playingState;
            Floors = new List<Floor>();
            Walls = new List<Wall>();
            _drawableGridSquares = new List<GridSquare>();

            LevelGenerator levelGenerator = new LevelGenerator(this, clickManager);
            levelGenerator.GenerateLevel();
            MinY = (int)Math.Min(Floors.Min(fl => fl.Position.Y), Walls.Min(wa => wa.Position.Y));
            MaxY = (int)Math.Max(Floors.Max(fl => fl.Position.Y), Walls.Max(wa => wa.Position.Y));
        }

        public bool DoesGridSquareExistAt(int xIdx, int yIdx)
        {
            bool exists = false;
            exists |= Floors.Any(floor => floor.XIdx == xIdx && floor.YIdx == yIdx);
            exists |= Walls.Any(wall => wall.XIdx == xIdx && wall.YIdx == yIdx);
            return exists;
        }

        public Floor GetStartingFloor()
        {
            return Floors.Find(sq => sq.XIdx == STARTING_X && sq.YIdx == STARTING_Y);
        }

        public void FrameTick(GameTime gameTime)
        {
            foreach (var floor in Floors)
            {
                floor.FrameTick(gameTime);
            }
            foreach (var wall in Walls)
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
            GridSquare containingPlayer = Floors.Find(sq => sq.Entity is Player);
            foreach (var floor in Floors)
            {
                floor.ActionTick(containingPlayer.XIdx, containingPlayer.YIdx, VIEW_RANGE);
            }
            foreach (var wall in Walls)
            {
                wall.ActionTick(containingPlayer.XIdx, containingPlayer.YIdx, VIEW_RANGE);
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
            foreach (var floor in Floors)
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
