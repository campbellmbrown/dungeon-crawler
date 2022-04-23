using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler;
using DungeonCrawler.GameStates.PlayingState;
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

        (IFloor orig, IFloor dest) MapStringToFloors(string mapStr)
        {
            var rows = mapStr.Replace(" ", "").Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
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

        [TestCase(
            @"OxxxxxxxD", 8)]
        [TestCase(
            @"DxxxxxxxO", 8)]
        [TestCase(
            @"Oxxxxxxxx
              xxxxxxxxx
              xxxxxxxxD", 10)]
        [TestCase(
            @"Oxxxxxxxx
              x.......x
              xxxxxxxxD", 10)]
        [TestCase(
            @"O.xxx.xxx
              x.x.x.x.x
              xxx.xxx.D", 18)]
        [TestCase(
            @"xxxxxxxxx
              xOxxxxxDx
              xxxxxxxxx", 6)]
        [TestCase(
            @"OD", 1)]
        [TestCase(
            @"xxx..x..xxx
              xOx.xxx.xDxxxx
              xxxxx.xxxxx..x
              .x...........x
              .xxxxxxxxxxxxx", 12)]
        public void FindShortestPath_PathIsRightLength(string mapStr, int expectedPathLength)
        {
            // Arrange:
            (IFloor orig, IFloor dest) = MapStringToFloors(mapStr);

            // Act:
            var result = _dijkstra.FindShortestPath(orig, dest);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(expectedPathLength));
        }

        public void FindShortestPath_OriginIsDestination()
        {
            // arrange:
            // Start with this, but afterwards we will set dest to be the same as the orig
            var floorStr =
            @"Oxxxxxxxx
              xxxxxxxxx
              xxxxxxxxD";
            (IFloor orig, IFloor dest) = MapStringToFloors(floorStr);
            dest = orig;

            // Act
            var result = _dijkstra.FindShortestPath(orig, dest);

            // Assert
            Assert.That(result.Count(), Is.Zero);
        }
    }
}
