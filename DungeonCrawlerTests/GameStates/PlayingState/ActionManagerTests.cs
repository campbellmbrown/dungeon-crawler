using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class ActionManagerTests
    {
        Mock<IGameTimeWrapper> _gameTimeWrapperMock;
        IActionManager _actionManager;

        [SetUp]
        public void Setup()
        {
            _gameTimeWrapperMock = new Mock<IGameTimeWrapper>();
            _actionManager = new ActionManager();
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.NotActive));
        }

        [Test]
        public void Start_ChangesStateToInProgress()
        {
            // Arrange:
            // Act:
            _actionManager.Start();

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));
        }

        [Test]
        public void Stop_ChangesStateToNotActive()
        {
            // Arrange:
            _actionManager.Start();

            // Act:
            _actionManager.Stop();

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.NotActive));
        }

        [Test]
        public void FrameTick_StateIsNotActive_DoesNothing()
        {
            // Arrange:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.NotActive));

            // Act:
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.NotActive));
        }

        [Test]
        public void FrameTick_StateIsInProgress_Progresses()
        {
            // Arrange:
            _actionManager.Start();
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
        public void FrameTick_FinishesAction_ChangesState()
        {
            // Arrange:
            _actionManager.Start();
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));

            // Act:
            _gameTimeWrapperMock
                .Setup(gameTime => gameTime.TimeDiffSec)
                .Returns(IActionManager.SecondsPerAction + 0.01f);
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.DecimalComplete, Is.EqualTo(1));
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.Finished));
        }

        [Test]
        public void FrameTick_FinishedState_StartsAgain()
        {
            // Arrange:
            FrameTick_FinishesAction_ChangesState(); // Now in the finished state

            // Act:
            _actionManager.FrameTick(_gameTimeWrapperMock.Object);

            // Assert:
            Assert.That(_actionManager.DecimalComplete, Is.EqualTo(0));
            Assert.That(_actionManager.ActionState, Is.EqualTo(ActionState.InProgress));
        }
    }
}