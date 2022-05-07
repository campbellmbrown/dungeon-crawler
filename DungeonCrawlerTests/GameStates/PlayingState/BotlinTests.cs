using System.Collections.Generic;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using DungeonCrawler.GameStates.PlayingState.PathFinding;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class BotlinTests
    {
        Botlin _botlin;
        Mock<ILogManager> _logManagerMock;
        Mock<IGridManager> _gridManagerMock;
        Mock<IActionManager> _actionManagerMock;
        Mock<IPathFinding> _pathFindingMock;
        Mock<IFloor> _floorMock;

        [SetUp]
        public void Setup()
        {
            _logManagerMock = new Mock<ILogManager>();
            _gridManagerMock = new Mock<IGridManager>();
            _actionManagerMock = new Mock<IActionManager>();
            _pathFindingMock = new Mock<IPathFinding>();
            _floorMock = new Mock<IFloor>();
            _botlin = new Botlin(
                _logManagerMock.Object,
                _gridManagerMock.Object,
                _actionManagerMock.Object,
                _pathFindingMock.Object,
                _floorMock.Object);

            _pathFindingMock
                .Setup(pathFinding => pathFinding.FindShortestPath(It.IsAny<IFloor>(), It.IsAny<IFloor>()))
                .Returns(new Stack<IFloor>());
        }

        [TestCase(0, 5)]
        [TestCase(0, -5)]
        [TestCase(5, 0)]
        [TestCase(-5, 0)]
        public void ActionTick_PlayerOutsideRange_DoesntSetDestination(
            int playerXIdx,
            int playerYIdx)
        {
            // Arrange
            _floorMock.Setup(floor => floor.XIdx).Returns(0);
            _floorMock.Setup(floor => floor.YIdx).Returns(0);
            var _playerFloorMock = new Mock<IFloor>();
            _playerFloorMock.Setup(playerFloor => playerFloor.XIdx).Returns(playerXIdx);
            _playerFloorMock.Setup(playerFloor => playerFloor.YIdx).Returns(playerYIdx);
            _gridManagerMock
                .Setup(gridmanager => gridmanager.PlayerFloor)
                .Returns(_playerFloorMock.Object);

            // Act:
            _botlin.ActionTick();

            // Assert:
            _pathFindingMock
                .Verify(
                    pathFinding => pathFinding.FindShortestPath(It.IsAny<IFloor>(), It.IsAny<IFloor>()),
                    Times.Never);
        }

        [TestCase(0, 4)]
        [TestCase(0, -4)]
        [TestCase(4, 0)]
        [TestCase(-4, 0)]
        public void ActionTick_PlayerInsideRange_SetsDestination(
            int playerXIdx,
            int playerYIdx)
        {
            // Arrange
            _floorMock.Setup(floor => floor.XIdx).Returns(0);
            _floorMock.Setup(floor => floor.YIdx).Returns(0);
            var _playerFloorMock = new Mock<IFloor>();
            _playerFloorMock.Setup(playerFloor => playerFloor.XIdx).Returns(playerXIdx);
            _playerFloorMock.Setup(playerFloor => playerFloor.YIdx).Returns(playerYIdx);
            _gridManagerMock
                .Setup(gridmanager => gridmanager.PlayerFloor)
                .Returns(_playerFloorMock.Object);

            // Act:
            _botlin.ActionTick();

            // Assert:
            _pathFindingMock
                .Verify(
                    pathFinding => pathFinding.FindShortestPath(It.IsAny<IFloor>(), It.IsAny<IFloor>()),
                    Times.Once);
        }
    }
}