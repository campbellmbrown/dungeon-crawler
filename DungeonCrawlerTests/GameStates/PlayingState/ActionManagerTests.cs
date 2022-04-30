using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class ActionManagerTests
    {
        Mock<IGameTimeWrapper> _gameTimeWrapperMock;
        Mock<ILogManager> _logManagerMock;
        IActionManager _actionManager;

        [SetUp]
        public void Setup()
        {
            _gameTimeWrapperMock = new Mock<IGameTimeWrapper>();
            _logManagerMock = new Mock<ILogManager>();

            _actionManager = new ActionManager(_logManagerMock.Object);

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Stopped));
        }

        [Test]
        public void Start_ChangesStateToStarting()
        {
            // Arrange:
            // Act:
            _actionManager.Start();

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Starting));
        }

        [Test]
        public void Stop_ChangesStateToStopping()
        {
            // Arrange:
            _actionManager.Start();

            // Act:
            _actionManager.Stop();

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Stopping));
        }

        [Test]
        public void FrameTick_Stopping_ChangesStateToStopped()
        {
            // Arrange:
            _actionManager.Stop();
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Stopping));

            // Act:
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Stopped));
        }

        [Test]
        public void FrameTick_StateIsStopped_DoesNothing()
        {
            // Arrange:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Stopped));

            // Act:
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Stopped));
        }

        [Test]
        public void FrameTick_StateIsInProgress_Progresses()
        {
            // Arrange:
            _actionManager.Start();
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));

            // Act:
            _gameTimeWrapperMock
                .Setup(gameTime => gameTime.TimeDiffSec)
                .Returns(0.01f);
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));
            var expectedDecimal = 0.01f / IActionManager.SecondsPerAction;
            Assert.That(_actionManager.DecimalComplete, Is.EqualTo(expectedDecimal));
        }

        [Test]
        public void Start_ResetsTimePassed()
        {
            // Arrange:
            _actionManager.Start();
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));
            _gameTimeWrapperMock
                .Setup(gameTime => gameTime.TimeDiffSec)
                .Returns(0.01f);
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);
            Assert.That(_actionManager.DecimalComplete, Is.Not.EqualTo(0));

            // Act:
            _actionManager.Start();

            // Assert:
            Assert.That(_actionManager.DecimalComplete, Is.EqualTo(0));
        }

        [Test]
        public void FrameTick_FinishesAction_ChangesStateToRestarting()
        {
            // Arrange:
            _actionManager.Start();
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));
            _gameTimeWrapperMock
                .Setup(gameTime => gameTime.TimeDiffSec)
                .Returns(IActionManager.SecondsPerAction + 0.01f);

            // Act:
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.DecimalComplete, Is.EqualTo(1));
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Restarting));
        }

        [Test]
        public void FrameTick_RestartingState_StartsAgain()
        {
            // Arrange:
            FrameTick_FinishesAction_ChangesStateToRestarting(); // Now in the restarting state

            // Act:
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.DecimalComplete, Is.EqualTo(0));
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));
        }
    }
}
