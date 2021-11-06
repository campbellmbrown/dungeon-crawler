using dungeoncrawler.Utility;
using System.Collections.Generic;
using System.Diagnostics;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class RoomGenerator
    {
        private enum Direction
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
        }

        // Takes in a Direction, returns a tuple containing the xDelta and yDelta for one step in that direction.
        private static Dictionary<Direction, (int, int)> _indexDeltas = new Dictionary<Direction, (int, int)>()
        {
            {  Direction.Up, (0, -1) },
            {  Direction.Down, (0, 1) },
            {  Direction.Left, (-1, 0) },
            {  Direction.Right, (1, 0) },
        };
        private static Dictionary<Direction, Direction> _oppositeDirection = new Dictionary<Direction, Direction>()
        {
            { Direction.Up, Direction.Down },
            { Direction.Down, Direction.Up },
            { Direction.Left, Direction.Right },
            { Direction.Right, Direction.Left },
        };
        private static Dictionary<Direction, List<Direction>> _turn90DegreesOptions = new Dictionary<Direction, List<Direction>>()
        {
            { Direction.Up, new List<Direction>(){ Direction.Left, Direction.Right } },
            { Direction.Down, new List<Direction>(){ Direction.Left, Direction.Right } },
            { Direction.Left, new List<Direction>(){ Direction.Up, Direction.Down } },
            { Direction.Right, new List<Direction>(){ Direction.Up, Direction.Down } },
        };

        private readonly GridManager _gridManager;
        private readonly List<GridSquare> _gridSquares;

        private int _distanceUntilDirectionChange;

        // Min/max constants
        private const int MAIN_PATH_MIN_LENGTH = 70;
        private const int MAIN_PATH_MAX_LENGTH = 90;
        private const int BRANCH_MAX_LENGTH = 6;
        private const int BRANCH_MIN_LENGTH = 4;
        private const int MIN_ROOM_WIDTH = 2;
        private const int MAX_ROOM_WIDTH = 4;
        private const int MIN_ROOM_HEIGHT = 2;
        private const int MAX_ROOM_HEIGHT = 4;

        // Percent chances
        private const int BRANCH_AT_DIRECTION_CHANGE_CHANCE = 50; // %

        // Guassian constants
        private const int DISTANCE_UNTIL_TURN_MEAN = 6;
        private const int DISTANCE_UNTIL_TURN_STD_DEV = 1;
        private const int DISTANCE_UNTIL_TURN_MIN = 2;

        // Direction weightings
        private const int DEFAULT_DIRECTION_WEIGHT = 1;
        private const int PRIORITY_DIRECTION_WEIGHT = 3;
        private Dictionary<Direction, RNG.Weight> _changeDirectionWeights = new Dictionary<Direction, RNG.Weight>()
        {
            { Direction.Up, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
            { Direction.Down, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
            { Direction.Left, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
            { Direction.Right, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
        };
        private Dictionary<Direction, RNG.Weight> _branchDirectionWeights = new Dictionary<Direction, RNG.Weight>()
        {
            { Direction.Up, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
            { Direction.Down, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
            { Direction.Left, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
            { Direction.Right, new RNG.Weight() { weight = DEFAULT_DIRECTION_WEIGHT } },
        };

        public RoomGenerator(GridManager gridManager, List<GridSquare> gridSquares)
        {
            _gridManager = gridManager;
            _gridSquares = gridSquares;
        }

        public void GenerateRoom()
        {
            GenerateFloor();
        }

        private int NewDistanceUntilDirectionChange()
        {
            return RNG.Guassian(DISTANCE_UNTIL_TURN_MEAN, DISTANCE_UNTIL_TURN_STD_DEV, DISTANCE_UNTIL_TURN_MIN);
        }

        private Direction ChooseNewDirection(Direction currentDirection)
        {
            return RNG.ChooseWeighted(_turn90DegreesOptions[currentDirection], _changeDirectionWeights);
        }

        public void GenerateFloor()
        {
            // (1) Create the original square
            GridSquare currentGridSquare = TryCreateNewGridSquare(0, 0);

            // (2) Create the room at the beginning
            // CreateRoom(gridManager, gridSquares, currentGridSquare);

            // (3) Choose a direction to start
            Direction currentDirection = RNG.RandomEnum<Direction>();

            // (4) Choose the priority directions
            UpdateDirectionWeights(currentDirection);

            // (5) Choose a length for the main path
            int mainPathLength = Game1.random.Next(MAIN_PATH_MIN_LENGTH, MAIN_PATH_MAX_LENGTH + 1);

            // (6) Calculate the distance until a direction change
            _distanceUntilDirectionChange = NewDistanceUntilDirectionChange();

            // (7) Create the main branch
            for (int idx = 0; idx < mainPathLength; idx++)
            {
                // (7a) Check a direction change
                if (_distanceUntilDirectionChange == 0)
                {
                    currentDirection = ChangeDirection(currentGridSquare, currentDirection);
                }

                // (7c) Create a GridSquare in the direction we are facing
                currentGridSquare = CreateGridSquareInDirection(currentGridSquare, currentDirection);

                // (7d) Lower distance for direction change
                _distanceUntilDirectionChange--;
            }

            // (8) Create a room at the end
        }

        private Direction ChangeDirection(GridSquare currentGridSquare, Direction currentDirection)
        {
            Direction previousDirection = currentDirection;
            Direction newDirection = ChooseNewDirection(currentDirection);
            _distanceUntilDirectionChange = NewDistanceUntilDirectionChange();
            if (RNG.PercentChance(BRANCH_AT_DIRECTION_CHANGE_CHANCE))
            {
                CreateBranch(
                    currentGridSquare,
                    RNG.ChooseWeighted(
                        new List<Direction>()
                        {
                            _oppositeDirection[previousDirection],
                            _oppositeDirection[newDirection]
                        },
                        _branchDirectionWeights)
                    );
            }
            return newDirection;
        }

        private void CreateBranch(GridSquare start, Direction direct)
        {
            GridSquare branchCurrentGridSquare = start;
            int branch_length = Game1.random.Next(BRANCH_MIN_LENGTH, BRANCH_MAX_LENGTH + 1);
            for (int idx = 0; idx < branch_length; idx++)
            {
                branchCurrentGridSquare = CreateGridSquareInDirection(branchCurrentGridSquare, direct);
            }
            // CreateRoom(gridManager, gridSquares, currentGridSquare);
        }

#if false
        private void CreateRoom(
            GridManager gridManager,
            List<GridSquare> gridSquares,
            GridSquare center,
            int minW = MIN_ROOM_WIDTH,
            int maxW = MAX_ROOM_WIDTH,
            int minH = MIN_ROOM_HEIGHT,
            int maxH = MAX_ROOM_HEIGHT)
        {
            int roomHeight = Game1.random.Next(minH, maxH + 1);
            int roomWidth = Game1.random.Next(minW, maxW + 1);

            // If the height/width is odd the center will be true.
            // e.g. for a width of 5 the origin will be 1 - (5 + 1)/2 = -2
            // [-2] [-1] [center] [1] [2]

            // If the height/width is even the center will be slightly off. It will be closer to the top left.
            // e.g. for a width of 6 the origin will be 1 - (6 + 1)/2 = -2
            // [-2] [-1] [center] [1] [2] [3]
            int originX = center.xIdx + 1 - ((roomWidth + 1) / 2);
            int originY = center.yIdx + 1 - ((roomHeight + 1) / 2);

            for (int xIdx = 0; xIdx < roomWidth; xIdx++)
            {
                for (int yIdx = 0; yIdx < roomHeight; yIdx++)
                {
                    CreateNewTile(gridManager, gridSquares, originX + xIdx, originY + yIdx);
                }
            }
        }
#endif

        private void UpdateDirectionWeights(Direction currentDirection)
        {
            Direction weightedDirection1 = currentDirection;
            Direction weightedDirection2 = RNG.ChooseRandom(_turn90DegreesOptions[currentDirection]);
            _changeDirectionWeights[weightedDirection1].weight = PRIORITY_DIRECTION_WEIGHT;
            _changeDirectionWeights[weightedDirection2].weight = PRIORITY_DIRECTION_WEIGHT;
            _branchDirectionWeights[_oppositeDirection[weightedDirection1]].weight = PRIORITY_DIRECTION_WEIGHT;
            _branchDirectionWeights[_oppositeDirection[weightedDirection2]].weight = PRIORITY_DIRECTION_WEIGHT;
        }

        private GridSquare CreateGridSquareInDirection(GridSquare current, Direction direction)
        {
            (int, int) idxDelta = _indexDeltas[direction];
            int newXIdx = current.xIdx + idxDelta.Item1;
            int newYIdx = current.yIdx + idxDelta.Item2;
            return TryCreateNewGridSquare(newXIdx, newYIdx);
        }

        /// <summary>
        /// Creates a new GridSquare at a specific location, or gets a GridSquare if it already exists.
        /// </summary>
        /// <param name="xIdx">X index of the new GridSquare.</param>
        /// <param name="yIdx">Y index of the new GridSquare</param>
        /// <returns>The new GridSquare, or an existing GridSquare if there is one already at the index.</returns>
        private GridSquare TryCreateNewGridSquare(int xIdx, int yIdx)
        {
            GridSquare gridSquareExists = _gridSquares.Find(sq => sq.xIdx == xIdx && sq.yIdx == yIdx);
            if (gridSquareExists == null)
            {
                GridSquare newGridSquare = new GridSquare(_gridManager, xIdx, yIdx);
                _gridSquares.Add(newGridSquare);
                return newGridSquare;
            }
            else
            {
                return gridSquareExists;
            }
        }
    }
}
