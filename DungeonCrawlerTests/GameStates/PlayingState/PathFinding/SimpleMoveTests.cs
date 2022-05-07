using System.Collections.Generic;
using DungeonCrawler.GameStates.PlayingState;
using DungeonCrawler.GameStates.PlayingState.PathFinding;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class SimpleMoveTests
    {
        List<Mock<IFloor>> _floorsMocks;
        List<IFloor> _floors;
        IPathFinding _simpleMove;
        PathFindingHelper _pathFindingHelper;

        [SetUp]
        public void Setup()
        {
            _floorsMocks = new List<Mock<IFloor>>();
            _floors = new List<IFloor>();
            _simpleMove = new SimpleMove(_floors);
            _pathFindingHelper = new PathFindingHelper(_floorsMocks, _floors);
        }

        static object[] _moveInCorrectDirectionTestCases =
        {
            new object[]
            {
                new List<string>
                {
                    "OxxxxxxxD",
                },
                1, 0
            },
            new object[]
            {
                new List<string>
                {
                    "Oxxx.xxxD",
                },
                1, 0
            },
            new object[]
            {
                new List<string>
                {
                    "Oxxx",
                    "xxxx",
                    "xxxx",
                    "Dxxx",
                },
                0, 1
            },
            new object[]
            {
                new List<string>
                {
                    "Oxxx",
                    "xxxx",
                    "xxxx",
                    "xxxD",
                },
                1, 0
            },
            new object[]
            {
                new List<string>
                {
                    "Dxxx",
                    "xxxx",
                    "xxxx",
                    "xxxO",
                },
                2, 3
            },
        };

        [TestCaseSource(nameof(_moveInCorrectDirectionTestCases))]
        public void FindShortestPath_ReturnsCorrectQueue(List<string> mapStr, int expectedX, int expectedY)
        {
            // Arrange:
            (IFloor orig, IFloor dest) = _pathFindingHelper.MapStringToFloors(mapStr);

            // Act:
            var result = _simpleMove.FindShortestPath(orig, dest);

            // Assert:
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Peek().XIdx, Is.EqualTo(expectedX));
            Assert.That(result.Peek().YIdx, Is.EqualTo(expectedY));
        }

        static object[] _noPathFoundTestCases =
        {
            new object[] { new List<string>
                {
                    "xxxx",
                    "xO.x",
                    "x..x",
                    "xxDx",
                    "xxxx",
                }},
            new object[] { new List<string>
                {
                    "xxxx",
                    "x..D",
                    "xO.x",
                    "xxxx",
                }},
            new object[] { new List<string>
                {
                    "xxxxx",
                    "xxOxx",
                    "x...x",
                    "xxxxx",
                    "xxDxx",
                    "xxxxx",
                }},
            new object[] { new List<string>
                {
                    "xxxxxx",
                    "xx.xxx",
                    "xO.xDx",
                    "xx.xxx",
                    "xxxxxx",
                }},
        };

        [TestCaseSource(nameof(_noPathFoundTestCases))]
        public void FindShortestPath_NoPathFound(List<string> mapStr)
        {
            // Arrange:
            (IFloor orig, IFloor dest) = _pathFindingHelper.MapStringToFloors(mapStr);

            // Act:
            var result = _simpleMove.FindShortestPath(orig, dest);

            // Assert:
            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}
