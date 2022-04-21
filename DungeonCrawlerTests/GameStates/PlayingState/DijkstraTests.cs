using DungeonCrawler.GameStates.PlayingState;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class DijkstraTests
    {
        IDijkstra _dijkstra;
        Mock<IGridManager> _gridManagerMock;

        [SetUp]
        public void Setup()
        {
            _gridManagerMock = new Mock<IGridManager>();
            _dijkstra = new Dijkstra(_gridManagerMock.Object);
        }

        [Test]
        public void TestSomething()
        {
            Assert.Pass();
        }
    }
}
