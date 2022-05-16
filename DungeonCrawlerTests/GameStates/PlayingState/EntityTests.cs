using System.Collections.Generic;
using System.Linq;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using DungeonCrawler.GameStates.PlayingState.PathFinding;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class EntityTests
    {
        Mock<ILogManager> _logManagerMock;
        Mock<IGridManager> _gridManagerMock;
        Mock<IActionManager> _actionManagerMock;
        Mock<IPathFinding> _pathFindingMock;
        Mock<IFloor> _floorMock;
        IEntity _entity;

        [SetUp]
        public void Setup()
        {
            _logManagerMock = new Mock<ILogManager>();
            _gridManagerMock = new Mock<IGridManager>();
            _actionManagerMock = new Mock<IActionManager>();
            _pathFindingMock = new Mock<IPathFinding>();
            _floorMock = new Mock<IFloor>();
            _entity = new Entity(
                _logManagerMock.Object,
                _gridManagerMock.Object,
                _actionManagerMock.Object,
                _pathFindingMock.Object,
                _floorMock.Object);
        }

        [Test]
        public void EntityConstuctor()
        {
            // Assert:
            Assert.That(_entity.QueuedFloors, Is.Empty);
            _floorMock.VerifySet(floor => floor.Entity = (Entity)_entity);
        }

        void SetupPathFindingPathToDestination(int length)
        {
            var pathToDestination = new Stack<IFloor>();
            for (int idx = 0; idx < length; idx++)
            {
                pathToDestination.Push(new Mock<IFloor>().Object);
            }
            _pathFindingMock
                .Setup(pathFinding => pathFinding.FindShortestPath(It.IsAny<IFloor>(), It.IsAny<IFloor>()))
                .Returns(pathToDestination);
        }

        [Test]
        public void SetDestination_AlreadyThere_QueuedFloorsDoesntIncrease()
        {
            // Arrange:
            IFloor floor = new Mock<IFloor>().Object;
            SetupPathFindingPathToDestination(0);

            // Act:
            _entity.SetDestination(floor);

            // Assert:
            Assert.That(_entity.QueuedFloors, Is.Empty);
        }

        [TestCase(1, 1)]
        [TestCase(5, 5)]
        [TestCase(IEntity.MAX_FLOORS_PER_PATHFIND, IEntity.MAX_FLOORS_PER_PATHFIND)]
        [TestCase(IEntity.MAX_FLOORS_PER_PATHFIND + 1, 0)] // Too many floors
        public void SetDestination_StoresPathToDestination(int lengthOfPath, int expectedQueueCount)
        {
            // Arrange:
            IFloor floor = new Mock<IFloor>().Object;
            SetupPathFindingPathToDestination(lengthOfPath);

            // Act:
            _entity.SetDestination(floor);

            // Assert:
            Assert.That(_entity.QueuedFloors.Count, Is.EqualTo(expectedQueueCount));
        }

        List<Mock<IFloor>> FakeQueuedFloors(int count)
        {
            var floorMocks = new List<Mock<IFloor>>();
            var queuedFloors = new Queue<IFloor>();
            for (int idx = 0; idx < count; idx++)
            {
                var floorMock = new Mock<IFloor>();
                floorMocks.Add(floorMock);
                queuedFloors.Enqueue(floorMock.Object);
            }
            _entity.QueuedFloors = queuedFloors;
            return floorMocks;
        }

        [Test]
        public void ActionTick_NoQueuedFloors_DoesNothing()
        {
            // Arrange:
            FakeQueuedFloors(0);

            // Act:
            _entity.ActionTick();

            // Assert: the floor this entity is on doesn't change
            _floorMock.VerifySet(floor => floor.Entity = null, Times.Never());
            Assert.That(_entity.PartakingInActionTick, Is.False);
        }

        [Test]
        public void ActionTick_ThereAreQueuedFloors_ChangesFloor()
        {
            // Arrange:
            FakeQueuedFloors(10);

            // Act:
            _entity.ActionTick();

            // Assert:
            Assert.That(_entity.QueuedFloors.Count, Is.EqualTo(9));
        }

        [Test]
        public void ActionTick_ThereAreQueuedFloors_PartakesInActionTick()
        {
            // Arrange:
            FakeQueuedFloors(10);

            // Act:
            _entity.ActionTick();

            // Assert:
            Assert.That(_entity.PartakingInActionTick, Is.True);
        }

        [Test]
        public void ActionTick_EmptiesQueuedFloors()
        {
            // Arrange:
            FakeQueuedFloors(10);

            // Act:
            for (int idx = 0; idx < 10; idx++)
            {
                _entity.ActionTick();
            }

            // Assert:
            Assert.That(_entity.QueuedFloors, Is.Empty);
        }

        [Test]
        public void ActionTick_ThereAreQueuedFloors_NewFloorNowHasEntity()
        {
            // Arrange:
            var floorMocks = FakeQueuedFloors(10);
            var nextFloorMock = floorMocks.First();

            // Act:
            _entity.ActionTick();

            // Assert:
            _floorMock.VerifySet(floor => floor.Entity = null, Times.Once);
            nextFloorMock.VerifySet(floor => floor.Entity = (IEntity)_entity, Times.Once);
        }

        [Test]
        public void ActionTick_NextFloorHasAnEntity_StopsPartaking()
        {
            // Arrange:
            var floorMocks = FakeQueuedFloors(10);
            var nextFloorMock = floorMocks.First();
            nextFloorMock
                .Setup(floor => floor.Entity)
                .Returns(new Mock<IEntity>().Object);

            // Act:
            _entity.ActionTick();

            // Assert:
            Assert.That(_entity.QueuedFloors.Count, Is.Zero);
            Assert.That(_entity.PartakingInActionTick, Is.False);
        }

        [Test]
        public void FrameTick_ActionManagerStopped_DoesNothing()
        {
            // Arrange:
            var originalPosition = _entity.Position;
            Mock<IGameTimeWrapper> gameTimeWrapperMock = new Mock<IGameTimeWrapper>();
            _actionManagerMock
                .Setup(actionManager => actionManager.ActionState)
                .Returns(ActionState.Stopped);

            // Act:
            _entity.FrameTick(gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_entity.Position, Is.EqualTo(originalPosition));
            Assert.That(_entity.PartakingInActionTick, Is.False);
        }

        [Test]
        public void FrameTick_ActionManagerStarting_TriggersActionTick()
        {
            // Arrange:
            FakeQueuedFloors(1);
            _actionManagerMock
                .Setup(actionManager => actionManager.ActionState)
                .Returns(ActionState.Starting);

            // Act:
            _entity.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            Assert.That(_entity.PartakingInActionTick, Is.True);
        }

        [Test]
        public void FrameTick_ActionManagerInProgress_PartakingInActionTick()
        {
            // Arrange:
            Assert.That(_entity.Position, Is.EqualTo(new Vector2()));

            // Setup the queue of floors:
            var queuedFloorMock = FakeQueuedFloors(1).First();
            queuedFloorMock
                .Setup(queuedFloor => queuedFloor.Position)
                .Returns(new Vector2(10, 0));

            _entity.ActionTick();
            Assert.That(_entity.PartakingInActionTick, Is.True);

            // Setup the action manager:
            _actionManagerMock
                .Setup(actionManager => actionManager.ActionState)
                .Returns(ActionState.InProgress);
            _actionManagerMock
                .Setup(actionManager => actionManager.DecimalComplete)
                .Returns(0.75f);

            // Act:
            _entity.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            Assert.That(_entity.Position, Is.EqualTo(new Vector2(7.5f, 0)));
        }

        [Test]
        public void FrameTick_ActionManagerInProgress_NotPartakingInActionTick()
        {
            // Arrange:
            var originalPosition = _entity.Position;
            _actionManagerMock
                .Setup(actionManager => actionManager.ActionState)
                .Returns(ActionState.InProgress);
            _entity.ActionTick();
            Assert.That(_entity.PartakingInActionTick, Is.False);

            // Act:
            _entity.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            Assert.That(_entity.Position, Is.EqualTo(originalPosition));
        }

        [TestCase(0, -1, 0, -16)]
        [TestCase(0, 1, 0, 16)]
        [TestCase(-1, 0, -16, 0)]
        [TestCase(1, 0, 16, 0)]
        public void Attack_Upwards(
            int victimFloorX,
            int victimFloorY,
            int expectedPeakX,
            int expectedPeakY)
        {
            // Arrange:
            Assert.That(_entity.Position, Is.EqualTo(new Vector2()));
            var victimFloorMock = new Mock<IFloor>();
            victimFloorMock.Setup(floor => floor.XIdx).Returns(victimFloorX);
            victimFloorMock.Setup(floor => floor.YIdx).Returns(victimFloorY);
            _entity.Attack(victimFloorMock.Object);
            _entity.ActionTick();
            Assert.That(_entity.PartakingInActionTick, Is.True);

            // Setup the action manager:
            _actionManagerMock
                .Setup(actionManager => actionManager.ActionState)
                .Returns(ActionState.InProgress);
            _actionManagerMock
                .Setup(actionManager => actionManager.DecimalComplete)
                .Returns(0.5f);

            // Act:
            _entity.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            Assert.That(_entity.Position, Is.EqualTo(new Vector2(expectedPeakX, expectedPeakY)));
        }
    }
}
