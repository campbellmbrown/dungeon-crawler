using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Management;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IGridManager : IMyDrawable
    {
        List<IFloor> Floors { get; }
        List<IWall> Walls { get; }
        int MinY { get; }
        int MaxY { get; }
        IFloor PlayerFloor { get; }
        IFloor StartingFloor { get; }

        void SetPlayerDestination(IFloor floor);
        bool DoesGridSquareExistAt(int xIdx, int yIdx);
        IFloor FindFloor(int xIdx, int yIdx);
    }

    public class GridManager : IGridManager
    {
        private readonly IPlayingState _playingState;
        private readonly ILevelGenerator _levelGenerator;

        public const int STARTING_X = 0;
        public const int STARTING_Y = 0;
        public const int VIEW_RANGE = 6;

        public List<IFloor> Floors { get; private set; } = new List<IFloor>();
        public List<IWall> Walls { get; private set; } = new List<IWall>();

        public int MinY { get; private set; }
        public int MaxY { get; private set; }
        public IFloor PlayerFloor { get => Floors.Find(floor => floor.Entity is IPlayer ); }
        public IFloor StartingFloor { get => FindFloor(STARTING_X, STARTING_Y); }

        public GridManager(IPlayingState playingState, ILevelGenerator levelGenerator)
        {
            _playingState = playingState;
            _levelGenerator = levelGenerator;
            levelGenerator.GenerateLevel(this);
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

        public void Draw(ISpriteBatchWrapper spriteBatch)
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

        public void SetPlayerDestination(IFloor floor)
        {
            _playingState.SetPlayerDestination(floor);
            // TODO: Add range limiting back.
            // Game1.Log("The destination isn't visible.", LogLevel.Warning);
        }

        public IFloor FindFloor(int xIdx, int yIdx)
        {
            return Floors.Find(floor => floor.XIdx == xIdx && floor.YIdx == yIdx);
        }
    }
}
