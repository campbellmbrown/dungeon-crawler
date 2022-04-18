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

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var floor in Floors)
            {
                floor.Draw(spriteBatch);
            }
            foreach (var wall in Walls)
            {
                wall.Draw(spriteBatch);
            }
        }

        public void SetPlayerDestination(Floor floor)
        {
            _playingState.SetPlayerDestination(floor);
            // TODO: Add range limiting back.
            // Game1.Log("The destination isn't visible.", LogLevel.Warning);
        }
    }
}
