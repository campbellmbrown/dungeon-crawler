using System.Collections.Generic;
using DungeonCrawler.GameStates.PlayingState;
using Moq;

namespace DungeonCrawlerTests
{
    public class PathFindingHelper
    {
        List<Mock<IFloor>> _floorsMocks;
        List<IFloor> _floors;

        public PathFindingHelper(List<Mock<IFloor>> floorsMocks, List<IFloor> floors)
        {
            _floorsMocks = floorsMocks;
            _floors = floors;
        }

        public (IFloor orig, IFloor dest) MapStringToFloors(List<string> rows)
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
    }
}