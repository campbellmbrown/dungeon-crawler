using DungeonCrawler.GameStates.PlayingState;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class GridManagerTests
    {
        IGridManager _gridManager;
        Mock<IPlayingState> _playingStateMock;
        Mock<ILevelGenerator> _levelGeneratorMock;

        [SetUp]
        public void Setup()
        {
            _playingStateMock = new Mock<IPlayingState>();
            _levelGeneratorMock = new Mock<ILevelGenerator>();
            _levelGeneratorMock
                .Setup(levelGenerator => levelGenerator.GenerateLevel(It.IsAny<IGridManager>()))
                .Callback<IGridManager>((gridManager) => {
                    gridManager.Floors.Add(CreateMockFloor(0, 0).Object);
                    gridManager.Walls.Add(CreateMockWall(1, 0).Object);
                });
            CreateGridManager();
        }

        void CreateGridManager()
        {
            _gridManager = new GridManager(
                _playingStateMock.Object,
                _levelGeneratorMock.Object);
        }

        Mock<IFloor> CreateMockFloor(int xIdx, int yIdx)
        {
            var floorMock = new Mock<IFloor>();
            floorMock.Setup(floor => floor.XIdx).Returns(xIdx);
            floorMock.Setup(floor => floor.YIdx).Returns(yIdx);
            floorMock.Setup(floor => floor.Position)
                .Returns(new Vector2(xIdx * GridSquare.GRID_SQUARE_SIZE, yIdx * GridSquare.GRID_SQUARE_SIZE));
            return floorMock;
        }

        Mock<IWall> CreateMockWall(int xIdx, int yIdx)
        {
            var wallMock = new Mock<IWall>();
            wallMock.Setup(wall => wall.XIdx).Returns(xIdx);
            wallMock.Setup(wall => wall.YIdx).Returns(yIdx);
            wallMock.Setup(wall => wall.Position)
                .Returns(new Vector2(xIdx * GridSquare.GRID_SQUARE_SIZE, yIdx * GridSquare.GRID_SQUARE_SIZE));
            return wallMock;
        }

        [Test]
        public void DoesGridSquareExistAt_NonExistant()
        {
            // Arrange: Grid squares created in Setup
            // Act:
            var doesGridSquareExistAt = _gridManager.DoesGridSquareExistAt(1, 1);

            // Assert:
            Assert.That(doesGridSquareExistAt, Is.False);
        }

        [Test]
        public void DoesGridSquareExistAt_Exists()
        {
            // Arrange: Grid squares created in Setup
            // Act:
            var doesGridSquareExistAt = _gridManager.DoesGridSquareExistAt(1, 0);

            // Assert:
            Assert.That(doesGridSquareExistAt, Is.True);
        }

        [Test]
        public void FindFloor_DoesntFindFloor()
        {
            // Arrange: Grid squares created in Setup
            // Act:
            var floor = _gridManager.FindFloor(3, 5);

            // Assert:
            Assert.That(floor, Is.Null);
        }

        [Test]
        public void FindFloor_FindsFloor()
        {
            // Arrange:
            var floorMock = CreateMockFloor(3, 5);
            _gridManager.Floors.Add(floorMock.Object);

            // Act:
            var floor = _gridManager.FindFloor(3, 5);

            // Assert:
            Assert.That(floor, Is.EqualTo(floorMock.Object));
        }

        [Test]
        public void SetPlayerDestination()
        {
            // Arrange:
            var floorMock = CreateMockFloor(3, 5);

            // Act:
            _gridManager.SetPlayerDestination(floorMock.Object);

            // Assert:
            _playingStateMock
                .Verify(playingState => playingState.SetPlayerDestination(floorMock.Object));
        }

        [TestCase(80, 0.8f)] // 5 * 16 = 80, this is the bottom-most GridSquare position
        [TestCase(40, 0.65f)]
        [TestCase(20, 0.575f)]
        [TestCase(0, 0.5f)] // This is the middle GridSquare position
        [TestCase(-20, 0.425f)]
        [TestCase(-40, 0.35f)]
        [TestCase(-80, 0.2f)] // -5 * 16 = -80, this is the top-most GridSquare position
        public void FindLayerDepth(float yPos, float expecedLayerDepth)
        {
            // Arrange:
            _levelGeneratorMock
                .Setup(levelGenerator => levelGenerator.GenerateLevel(It.IsAny<IGridManager>()))
                .Callback<IGridManager>(gridManager => {
                    gridManager.Floors.Add(CreateMockFloor(0, -5).Object);
                    gridManager.Walls.Add(CreateMockWall(0, 5).Object);
                });
            CreateGridManager();

            // Act:
            var layerDepth = _gridManager.FindLayerDepth(yPos);

            // Assert:
            Assert.That(layerDepth, Is.EqualTo(expecedLayerDepth).Within(0.001f));
        }
    }
}
