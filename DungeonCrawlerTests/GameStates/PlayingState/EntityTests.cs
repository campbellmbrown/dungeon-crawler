using System.Collections.Generic;
using System.Linq;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using Microsoft.Xna.Framework;
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
            // Arrange:
            var originalPosition = _entity.Position;
            Assert.That(_entity.DestinationState, Is.EqualTo(DestinationState.AtDestination));
            Mock<IGameTimeWrapper> gameTimeWrapperMock = new Mock<IGameTimeWrapper>();

            // Act:
            _entity.FrameTick(gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_entity.DestinationState, Is.EqualTo(DestinationState.AtDestination));
            Assert.That(_entity.Position, Is.EqualTo(originalPosition));
        }

        [Test]
        public void FrameTick_OffDestination_ChangesPosition()
        {
            // Arrange:
            var originalPosition = _entity.Position;
            var queuedFloorMock = FakeQueuedFloors(1).First();
            queuedFloorMock
                .Setup(queuedFloor => queuedFloor.Position)
                .Returns(new Vector2(100, 0));
            // This should set up the DestinationState to OffDestination
            // and the _destination set to the queuedFloor:
            _entity.ActionTick();
            Mock<IGameTimeWrapper> gameTimeWrapperMock = new Mock<IGameTimeWrapper>();
            gameTimeWrapperMock
                .Setup(gameTimeWrapper => gameTimeWrapper.TimeDiffSec)
                .Returns(0.1f); // 100 ms have passed

            // Act:
            _entity.FrameTick(gameTimeWrapperMock.Object);

            // Assert:
            var expectedDisplacement = new Vector2(IEntity.MOVEMENT_SPEED * 0.1f, 0);
            Assert.That(_entity.Position, Is.EqualTo(expectedDisplacement));
        }

        [Test]
        public void FrameTick_OffDestination_CloseEnoughToDestination_SnapsPosition()
        {
            // Arrange:
            var originalPosition = _entity.Position;
            var queuedFloorMock = FakeQueuedFloors(1).First();
            queuedFloorMock
                .Setup(queuedFloor => queuedFloor.Position)
                .Returns(new Vector2(0.5f, 0)); // Really close to the entities position
            // This should set up the DestinationState to OffDestination
            // and the _destination set to the queuedFloor:
            _entity.ActionTick();
            Mock<IGameTimeWrapper> gameTimeWrapperMock = new Mock<IGameTimeWrapper>();
            gameTimeWrapperMock
                .Setup(gameTimeWrapper => gameTimeWrapper.TimeDiffSec)
                .Returns(0.01f); // 10 ms have passed

            // Act:
            _entity.FrameTick(gameTimeWrapperMock.Object);

            // Assert:
            // IEntity.MOVEMENT_SPEED (80f) * 0.01f = 0.8 pixels of movement
            // This will move the entitiy 0.8 - 0.5 = 0.3 pixels
            // This is close enough to the destination so it should snap to the desination.
            Assert.That(_entity.Position, Is.EqualTo(new Vector2(0.5f, 0)));
        }
    }
}