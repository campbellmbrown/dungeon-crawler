using System.Collections.Generic;
using DungeonCrawler;
using DungeonCrawler.Visual;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class AnimationManagerTests
    {
        Mock<IAnimation> _animationMock;
        Mock<IGameTimeWrapper> _gameTimeWrapperMock;
        IAnimationManager _animationManager;

        [SetUp]
        public void Setup()
        {
            _animationMock = new Mock<IAnimation>();
            _gameTimeWrapperMock = new Mock<IGameTimeWrapper>();
            _animationManager = new AnimationManager(_animationMock.Object);
        }

        [Test]
        public void FrameTick_TimerDoesntExceedFrameSpeed()
        {
            // Arrange:
            _animationMock.Setup(animation => animation.Type).Returns(IAnimation.FrameType.Constant);
            _animationMock.Setup(animation => animation.FrameSpeed).Returns(0.5f);
            _gameTimeWrapperMock.Setup(gameTime => gameTime.TimeDiffSec).Returns(0.4f);

            // Act:
            _animationManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_animationManager.CurrentFrame, Is.Zero);
        }

        [Test]
        public void FrameTick_TimerExceedsFrameSpeed()
        {
            // Arrange:
            _animationMock.Setup(animation => animation.Type).Returns(IAnimation.FrameType.Constant);
            _animationMock.Setup(animation => animation.FrameSpeed).Returns(0.5f);
            _animationMock.Setup(animation => animation.FrameCount).Returns(4);
            _gameTimeWrapperMock.Setup(gameTime => gameTime.TimeDiffSec).Returns(0.4f);

            // Act:
            _animationManager.FrameTick(_gameTimeWrapperMock.Object);
            _animationManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_animationManager.CurrentFrame, Is.EqualTo(1));
        }

        [Test]
        public void FrameTick_VaryingAnimation()
        {
            // Arrange:
            _animationMock.Setup(animation => animation.Type).Returns(IAnimation.FrameType.Varying);
            _animationMock.Setup(animation => animation.FrameSpeeds).Returns(new List<float> { 0.4f, 0.5f, 0.5f });
            _animationMock.Setup(animation => animation.FrameCount).Returns(4);
            _gameTimeWrapperMock.Setup(gameTime => gameTime.TimeDiffSec).Returns(0.425f);

            // Act:
            _animationManager.FrameTick(_gameTimeWrapperMock.Object);
            _animationManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_animationManager.CurrentFrame, Is.EqualTo(1));
        }

        [Test]
        public void FrameTick_CurrentFrameExceedsAnimationFrameCount()
        {
            // Arrange:
            _animationMock.Setup(animation => animation.Type).Returns(IAnimation.FrameType.Constant);
            _animationMock.Setup(animation => animation.FrameSpeed).Returns(0.5f);
            _animationMock.Setup(animation => animation.FrameCount).Returns(4);
            _gameTimeWrapperMock.Setup(gameTime => gameTime.TimeDiffSec).Returns(0.4f);

            // Act:
            // 5 * 0.4 = 2000 ms, 0.5 * 4 = 2000 ms
            for (int idx = 0; idx < 5; idx++)
            {
                _animationManager.FrameTick(_gameTimeWrapperMock.Object);
            }

            // Assert:
            Assert.That(_animationManager.CurrentFrame, Is.EqualTo(0));
        }

        [Test]
        public void FrameTick_Play()
        {
            // Arrange:
            var _newAnimation = new Mock<IAnimation>();

            // Act:
            _animationManager.Play(_newAnimation.Object);

            // Arrange:
            Assert.That(_animationManager.Animation, Is.EqualTo(_newAnimation.Object));
            Assert.That(_animationManager.CurrentFrame, Is.Zero);
        }
    }
}
