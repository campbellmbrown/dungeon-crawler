using dungeoncrawler.Utility;
using System.Collections.Generic;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class LevelGenerator
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
        private const int MAIN_PATH_MIN_LENGTH = 65;
        private const int MAIN_PATH_MAX_LENGTH = 80;
        private const int BRANCH_MAX_LENGTH = 6;
        private const int BRANCH_MIN_LENGTH = 4;
        private const int MIN_ROOM_WIDTH = 3;
        private const int MAX_ROOM_WIDTH = 5;
        private const int MIN_ROOM_HEIGHT = 3;
        private const int MAX_ROOM_HEIGHT = 5;

        // Percent chances
        private const int BRANCH_AT_DIRECTION_CHANGE_CHANCE = 50; // %
        private const int ROOM_AT_DIRECTION_CHANGE_CHANCE = 10; // %
        private const int SHIFT_CHANCE = 20; // %
        private const int REMOVE_ROOM_CORNER_CHANCE = 30; // %

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

        public LevelGenerator(GridManager gridManager, List<GridSquare> gridSquares)
        {
            _gridManager = gridManager;
            _gridSquares = gridSquares;
        }

        public void GenerateLevel()
        {
            GenerateFloor();
        }

        private void GenerateFloor()
        {
            GridSquare currentGridSquare = TryCreateNewGridSquare(GridManager.STARTING_X, GridManager.STARTING_Y);
            CreateRoom(currentGridSquare);
            Direction currentDirection = RNG.RandomEnum<Direction>();
            UpdateDirectionWeights(currentDirection);
            int mainPathLength = Game1.random.Next(MAIN_PATH_MIN_LENGTH, MAIN_PATH_MAX_LENGTH + 1);
            _distanceUntilDirectionChange = NewDistanceUntilDirectionChange();

            for (int idx = 0; idx < mainPathLength; idx++)
            {
                if (_distanceUntilDirectionChange == 0)
                {
                    currentDirection = ChangeDirection(currentGridSquare, currentDirection);
                }

                if (RNG.PercentChance(SHIFT_CHANCE))
                {
                    currentGridSquare = ShiftSidewaysOne(currentGridSquare, currentDirection);
                }

                currentGridSquare = CreateGridSquareInDirection(currentGridSquare, currentDirection);
                _distanceUntilDirectionChange--;
            }
            CreateRoom(currentGridSquare);
        }

        /// <summary>
        /// Changes the direction that the main path is heading in. Can also branch and create rooms.
        /// Will only turn 90 degrees from the current direction.
        /// </summary>
        /// <param name="currentGridSquare">The current GridSquare in the main path.</param>
        /// <param name="currentDirection">The current direction of the main path</param>
        /// <returns>A new direction.</returns>
        private Direction ChangeDirection(GridSquare currentGridSquare, Direction currentDirection)
        {
            Direction previousDirection = currentDirection;
            Direction newDirection = ChooseNewDirection(currentDirection);
            _distanceUntilDirectionChange = NewDistanceUntilDirectionChange();
            if (RNG.PercentChance(BRANCH_AT_DIRECTION_CHANGE_CHANCE))
            {
                List<Direction> options = new List<Direction>()
                {
                    previousDirection,
                    _oppositeDirection[newDirection]
                };
                CreateBranch(
                    currentGridSquare,
                    RNG.ChooseWeighted(options, _branchDirectionWeights)
                );
            }
            if (RNG.PercentChance(ROOM_AT_DIRECTION_CHANGE_CHANCE))
            {
                CreateRoom(currentGridSquare);
            }
            return newDirection;
        }

        /// <summary>
        /// Creates a single-direction branch from a certain point 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="branchDirection"></param>
        private void CreateBranch(GridSquare start, Direction branchDirection)
        {
            GridSquare branchCurrentGridSquare = start;
            int branch_length = Game1.random.Next(BRANCH_MIN_LENGTH, BRANCH_MAX_LENGTH + 1);
            for (int idx = 0; idx < branch_length; idx++)
            {
                branchCurrentGridSquare = CreateGridSquareInDirection(branchCurrentGridSquare, branchDirection);
            }
            CreateRoom(branchCurrentGridSquare);
        }

        /// <summary>
        /// Generates a number that determines how many GridSquares until the direction will be changed.
        /// </summary>
        /// <returns>How many GridSquares until a direction change.</returns>
        private int NewDistanceUntilDirectionChange()
        {
            return RNG.Guassian(DISTANCE_UNTIL_TURN_MEAN, DISTANCE_UNTIL_TURN_STD_DEV, DISTANCE_UNTIL_TURN_MIN);
        }

        /// <summary>
        /// Chooses a new direction. The new direction will always be 90 degrees from the current position, and will be weighted.
        /// </summary>
        /// <param name="currentDirection">The current direction.</param>
        /// <returns>The new direction.</returns>
        private Direction ChooseNewDirection(Direction currentDirection)
        {
            return RNG.ChooseWeighted(_turn90DegreesOptions[currentDirection], _changeDirectionWeights);
        }

        /// <summary>
        /// Creates a room centered around a particular position. The corners of the room have a chance to not be generated
        /// to have a rugged look.
        /// </summary>
        /// <param name="center">The center of the room. If the room has an even dimension, this will be favored towards the top left.</param>
        /// <param name="minW">The minimum width of the room.</param>
        /// <param name="maxW">The maximum width of the room.</param>
        /// <param name="minH">The minimum height of the room.</param>
        /// <param name="maxH">The maximum height of the room.</param>
        private void CreateRoom(GridSquare center, int minW = MIN_ROOM_WIDTH, int maxW = MAX_ROOM_WIDTH, int minH = MIN_ROOM_HEIGHT, int maxH = MAX_ROOM_HEIGHT)
        {
            int roomHeight = Game1.random.Next(minH, maxH + 1);
            int roomWidth = Game1.random.Next(minW, maxW + 1);

            // The center will always be closer to the top left, e.g.
            //    +-------+   +-------+   +-----+
            //    |X X X X|   |X X X X|   |X X X|
            //    |X C X X|   |X C X X|   |X C X|
            //    |X X X X|   |X X X X|   |X X X|
            //    |X X X X|   +-------+   +-----+
            //    +-------+

            int originX = center.xIdx + 1 - ((roomWidth + 1) / 2);
            int originY = center.yIdx + 1 - ((roomHeight + 1) / 2);

            for (int xIdx = 0; xIdx < roomWidth; xIdx++)
            {
                for (int yIdx = 0; yIdx < roomHeight; yIdx++)
                {
                    bool corner = (xIdx == 0 || xIdx == (roomWidth - 1)) && (yIdx == 0 || yIdx == (roomHeight - 1));
                    if (!(corner && RNG.PercentChance(REMOVE_ROOM_CORNER_CHANCE)))
                    {
                        TryCreateNewGridSquare(originX + xIdx, originY + yIdx);
                    }
                }
            }
        }

        /// <summary>
        /// Puts weights on a particular direction. The direction is always diagonal, and will always be in the
        /// direction of the current direction.
        /// </summary>
        /// <param name="currentDirection">The current direction.</param>
        private void UpdateDirectionWeights(Direction currentDirection)
        {
            Direction weightedDirection1 = currentDirection;
            Direction weightedDirection2 = RNG.ChooseRandom(_turn90DegreesOptions[currentDirection]);
            _changeDirectionWeights[weightedDirection1].weight = PRIORITY_DIRECTION_WEIGHT;
            _changeDirectionWeights[weightedDirection2].weight = PRIORITY_DIRECTION_WEIGHT;
            _branchDirectionWeights[_oppositeDirection[weightedDirection1]].weight = PRIORITY_DIRECTION_WEIGHT;
            _branchDirectionWeights[_oppositeDirection[weightedDirection2]].weight = PRIORITY_DIRECTION_WEIGHT;
        }

        /// <summary>
        /// Moves one GridSquare to the side.
        /// </summary>
        /// <param name="currentGridSquare">The GridSquare to shift to the side of.</param>
        /// <param name="currentDirection">The current direction.</param>
        /// <returns>The new GridSquare, or an existing GridSquare if there is one already at the index.</returns>
        private GridSquare ShiftSidewaysOne(GridSquare currentGridSquare, Direction currentDirection)
        {
            return CreateGridSquareInDirection(currentGridSquare, RNG.ChooseRandom(_turn90DegreesOptions[currentDirection]));
        }

        /// <summary>
        /// Creates a GridSquare in a particular direction.
        /// </summary>
        /// <param name="current">The GridSquare to move from.</param>
        /// <param name="direction">The direction to move into.</param>
        /// <returns>The new GridSquare, or an existing GridSquare if there is one already at the index.</returns>
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
