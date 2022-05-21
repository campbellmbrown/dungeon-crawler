using System;
using System.Collections.Generic;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using DungeonCrawler.GameStates.PlayingState.PathFinding;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class DungeonCrawlerTests
    {
        IEntityManager _entityManager;
        Mock<ILogManager> _logManagerMock;
        Mock<IGridManager> _gridManagerMock;
        Mock<IActionManager> _actionManagerMock;
        Mock<IEntityFactory> _entityFactoryMock;
        Mock<IPlayer> _playerMock;

        [SetUp]
        public void Setup()
        {
            _logManagerMock = new Mock<ILogManager>();
            _gridManagerMock = new Mock<IGridManager>();
            _actionManagerMock = new Mock<IActionManager>();
            _entityFactoryMock = new Mock<IEntityFactory>();
            _playerMock = new Mock<IPlayer>();
            _entityFactoryMock
                .Setup(entityFactory => entityFactory.CreatePlayer(
                    It.IsAny<IPathFinding>(),
                    It.IsAny<IFloor>()
                ))
                .Returns(_playerMock.Object);

            _entityManager = new EntityManager(
                _logManagerMock.Object,
                _gridManagerMock.Object,
                _actionManagerMock.Object,
                _entityFactoryMock.Object);
        }

        [Test]
        public void FrameTick_CallsPlayerFrameTick()
        {
            // Arrange:
            // Act:
            _entityManager.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            _playerMock.Verify(player => player.FrameTick(It.IsAny<IGameTimeWrapper>()), Times.Once);
        }

        [Test]
        public void FrameTick_CallsAllEntityFrameTicks()
        {
            // Arrange:
            var entityMocks = new List<Mock<IEntity>>();
            for (int count = 0; count < 10; count++)
            {
                var entityMock = new Mock<IEntity>();
                entityMocks.Add(entityMock);
                _entityManager.AddEntity(entityMock.Object);
            }

            // Act:
            _entityManager.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            foreach (var entityMock in entityMocks)
            {
                entityMock.Verify(entity => entity.FrameTick(It.IsAny<IGameTimeWrapper>()), Times.Once);
            }
        }

        [Test]
        public void Draw_CallsPlayerDraw()
        {
            // Arrange:
            // Act:
            _entityManager.Draw(new Mock<ISpriteBatchWrapper>().Object);

            // Assert:
            _playerMock.Verify(player => player.Draw(It.IsAny<ISpriteBatchWrapper>()), Times.Once);
        }

        [Test]
        public void Draw_CallsAllEntityDraws()
        {
            // Arrange:
            var entityMocks = new List<Mock<IEntity>>();
            for (int count = 0; count < 10; count++)
            {
                var entityMock = new Mock<IEntity>();
                entityMocks.Add(entityMock);
                _entityManager.AddEntity(entityMock.Object);
            }

            // Act:
            _entityManager.Draw(new Mock<ISpriteBatchWrapper>().Object);

            // Assert:
            foreach (var entityMock in entityMocks)
            {
                entityMock.Verify(entity => entity.Draw(It.IsAny<ISpriteBatchWrapper>()), Times.Once);
            }
        }
    }
}
