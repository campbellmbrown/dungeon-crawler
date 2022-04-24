using System.Collections.Generic;
using System.Linq;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class EntityTests
    {
        Mock<ILogManager> _logManagerMock;
        Mock<IGridManager> _gridManagerMock;
        Mock<IPathFinding> _pathFindingMock;
        Mock<IFloor> _floorMock;
        IEntity _entity;

        [SetUp]
        public void Setup()
        {
            _logManagerMock = new Mock<ILogManager>();
            _gridManagerMock = new Mock<IGridManager>();
            _pathFindingMock = new Mock<IPathFinding>();
            _floorMock = new Mock<IFloor>();
            _entity = new Entity(
                _logManagerMock.Object,
                _gridManagerMock.Object,
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

        [Test]
        public void SetDestination_TooManyDestinations()
        {
            // Arrange:
            IFloor floor = new Mock<IFloor>().Object;
            SetupPathFindingPathToDestination(IEntity.MAX_FLOORS_PER_PATHFIND + 1);

            // Act:
            _entity.SetDestination(floor);

            // Assert:
            Assert.That(_entity.QueuedFloors, Is.Empty);
            _logManagerMock.Verify(log => log.Log("The destination is too far away from the source.", LogLevel.Warning), Times.Once);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(IEntity.MAX_FLOORS_PER_PATHFIND)]
        public void SetDestination_StoresPathToDestination(int lengthOfPath)
        {
            // Arrange:
            IFloor floor = new Mock<IFloor>().Object;
            SetupPathFindingPathToDestination(lengthOfPath);

            // Act:
            _entity.SetDestination(floor);

            // Assert:
            Assert.That(_entity.QueuedFloors.Count, Is.EqualTo(lengthOfPath));
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
        public void ActionTick_ThereAreQueuedFloors_SetsDestinationStatus()
        {
            // Arrange:
            FakeQueuedFloors(10);

            // Act:
            _entity.ActionTick();

            // Assert:
            Assert.That(_entity.DestinationState, Is.EqualTo(DestinationState.OffDestination));
        }

        [Test]
        public void FrameTick_AtDestination_DoesNothing()
        {
        }

        [Test]
        public void FrameTick_OffDestination_ChangesPosition()
        {
        }

        [Test]
        public void FrameTick_OffDestination_CloseEnoughToDestination_SnapsPosition()
        {
        }
    }
}