using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace DungeonCrawlerTests
{
    public class DijkstraTests
    {
        Mock<ILogManager> _logManagerMock;
        List<Mock<IFloor>> _floorsMocks;
        List<IFloor> _floors;
        IDijkstra _dijkstra;

        [SetUp]
        public void Setup()
        {
            _logManagerMock = new Mock<ILogManager>();
            _floorsMocks = new List<Mock<IFloor>>();
            _floors = new List<IFloor>();
            _dijkstra = new Dijkstra(_logManagerMock.Object, _floors);
        }

        (IFloor orig, IFloor dest) MapStringToFloors(List<string> rows)
        {
            var orig = new Mock<IFloor>().Object;
            var dest = new Mock<IFloor>().Object;
            int rowIdx = 0;
            foreach (var row in rows)
            {
                int columnIdx = 0;
                foreach (var column in row)
                {
                    if (column != '.')
                    {
                        var floorMock = new Mock<IFloor>();
                        floorMock.Setup(floor => floor.XIdx).Returns(columnIdx);
                        floorMock.Setup(floor => floor.YIdx).Returns(rowIdx);
                        _floorsMocks.Add(floorMock);
                        _floors.Add(floorMock.Object);

                        if (column == 'O')
                        {
                            orig = floorMock.Object;
                        }
                        if (column == 'D')
                        {
                            dest = floorMock.Object;
                        }
                    }
                    columnIdx++;
                }
                rowIdx++;
            }
            return (orig, dest);
        }

        static object[] PathIsRightLengthTestCases =
        {
            new object[]
            {
                new List<string>
                {
                    "OxxxxxxxD",
                },
                8
            },
            new object[]
            {
                new List<string>
                {
                    @"DxxxxxxxO",
                },
                8
            },
            new object[]
            {
                new List<string>
                {
                    "Oxxxxxxxx",
                    "xxxxxxxxx",
                    "xxxxxxxxD",
                },
                10
            },
            new object[]
            {
                new List<string>
                {
                    "Oxxxxxxxx",
                    "x.......x",
                    "xxxxxxxxD",
                },
                10
            },
            new object[]
            {
                new List<string>
                {
                    "O.xxx.xxx",
                    "x.x.x.x.x",
                    "xxx.xxx.D",
                },
                18
            },
            new object[]
            {
                new List<string>
                {
                    "xxxxxxxxx",
                    "xOxxxxxDx",
                    "xxxxxxxxx",
                },
                6
            },
            new object[]
            {
                new List<string>
                {
                    "OD",
                },
                1
            },
            new object[]
            {
                new List<string>
                {
                    "xxx..x..xxx...",
                    "xOx.xxx.xDxxxx",
                    "xxxxx.xxxxx..x",
                    ".x...........x",
                    ".xxxxxxxxxxxxx",
                },
                12
            },
        };

        [TestCaseSource(nameof(PathIsRightLengthTestCases))]
        public void FindShortestPath_PathIsRightLength(List<string> mapStr, int expectedPathLength)
        {
            // Arrange:
            (IFloor orig, IFloor dest) = MapStringToFloors(mapStr);

            // Act:
            var result = _dijkstra.FindShortestPath(orig, dest);

            // Assert:
            Assert.That(result.Count(), Is.EqualTo(expectedPathLength));
        }

        public void FindShortestPath_OriginIsDestination()
        {
            // Arrange:
            // Start with this, but afterwards we will set dest to be the same as the orig
            var floorStr = new List<string>
            {
                "Oxxxxxxxx",
                "xxxxxxxxx",
                "xxxxxxxxD",
            };
            (IFloor orig, IFloor dest) = MapStringToFloors(floorStr);
            dest = orig;

            // Act:
            var result = _dijkstra.FindShortestPath(orig, dest);

            // Assert:
            Assert.That(result.Count(), Is.Zero);
        }

        List<string> largeMap = new List<string>
        {
            "xxxxxxxxxxxx................",
            "xxxxxxxxxx.xx...............",
            "xxxxxxxxx...xx..............",
            "...xxxxxx....x..............",
            "...xxxx.x....x..............",
            "...xxxxxxxx.xxx.............",
            "...xxxxxxxxxxxxx............",
            "...xxxxxxxxxxxxxxxxxx.......",
            "...xxxxxxxxxxxxx....x.......",
            ".......xxxxx..xx....x.......",
            "..........xxxx.x....x.......",
            "..........xxxx.x....xx..xxx.",
            "..........xxxxxxxxxxxxxxxxxx",
            "..........xxxx.........xxxxx",
            "..........xxxx.........xxxxx",
        }; // x: 0-27, y: 0-14

        (IFloor orig, IFloor dest) UseBigMap(int origX, int origY, int destX, int destY)
        {
            var largeMapCopy = largeMap.ToList();
            Func<string, int, char, string> replaceInString = (origString, index, character) =>
            {
                var chars = origString.ToCharArray();
                chars[index] = character;
                return new string(chars);
            };
            largeMapCopy[origY] = replaceInString(largeMapCopy[origY], origX, 'O');
            largeMapCopy[destY] = replaceInString(largeMapCopy[destY], destX, 'D');
            return MapStringToFloors(largeMapCopy);
        }

        [TestCase(0, 0, 27, 14)]
        [TestCase(27, 14, 0, 0)]
        public void FindShortestpath_NoDiagonals(int origX, int origY, int destX, int destY)
        {
            // Arrange:
            (IFloor orig, IFloor dest) = UseBigMap(origX, origY, destX, destY);

            // Act:
            var result = _dijkstra.FindShortestPath(orig, dest);

            // Assert:
            var start = result.Pop();
            while (result.Count() > 0)
            {
                var next = result.Pop();
                var difference = new Vector2(next.XIdx - start.XIdx, next.YIdx - start.YIdx);
                Assert.That(difference.Length(), Is.EqualTo(1));
                start = next;
            }
        }
    }
}
