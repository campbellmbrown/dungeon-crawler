using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class FocusManagerTests
    {
        IFocusManager _focusManager;
        Mock<ISpriteBatchManager> _spriteBatchManagerMock;

        [SetUp]
        public void SetUp()
        {
            _spriteBatchManagerMock = new Mock<ISpriteBatchManager>();
            _focusManager = new FocusManager(_spriteBatchManagerMock.Object);
            _spriteBatchManagerMock
                .Setup(spriteBatchManager => spriteBatchManager.MainLayerView)
                .Returns(new Mock<ILayerView>().Object);
        }

        [Test]
        public void FrameTick_NotFocused_StaysStill()
        {
            // Arrange:
            // Act:
            _focusManager.FrameTick(new Mock<IGameTimeWrapper>().Object);
            _focusManager.FrameTick(new Mock<IGameTimeWrapper>().Object);
            _focusManager.FrameTick(new Mock<IGameTimeWrapper>().Object);

            _spriteBatchManagerMock
                .Verify(spriteBatchManager => spriteBatchManager.MainLayerView.Focus(
                    new Vector2()
                ), Times.Exactly(3));
        }

        [Test]
        public void FrameTick_FocusCalled_GoesInPositiveDirection()
        {
            // Arrange:
            // Act:
            _focusManager.Focus(new Vector2(20, 30));
            _focusManager.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            _spriteBatchManagerMock
                .Verify(spriteBatchManager => spriteBatchManager.MainLayerView.Focus(
                    It.Is<Vector2>(focus => focus.X > 0 && focus.Y > 0)
                ), Times.Once);
        }

        [Test]
        public void FrameTick_FocusCalled_GoesInNegativeDirection()
        {
            // Arrange:
            // Act:
            _focusManager.Focus(new Vector2(-20, -30));
            _focusManager.FrameTick(new Mock<IGameTimeWrapper>().Object);

            // Assert:
            _spriteBatchManagerMock
                .Verify(spriteBatchManager => spriteBatchManager.MainLayerView.Focus(
                    It.Is<Vector2>(focus => focus.X < 0 && focus.Y < 0)
                ), Times.Once);
        }
    }
}
