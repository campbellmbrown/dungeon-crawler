using System.Collections.Generic;
using System.Linq;

namespace DungeonCrawler.GameStates.PlayingState.Tiles
{
    public class BitMask
    {
        public enum BitMaskType
        {
            Bits4,
            Bits8,
        }

        static Dictionary<int, int> _8BitRemap = new Dictionary<int, int>()
        {
            { 2, 1 },
            { 8, 2 },
            { 10, 3 },
            { 11, 4 },
            { 16, 5 },
            { 18, 6 },
            { 22, 7 },
            { 24, 8 },
            { 26, 9 },
            { 27, 10 },
            { 30, 11 },
            { 31, 12 },
            { 64, 13 },
            { 66, 14 },
            { 72, 15 },
            { 74, 16 },
            { 75, 17 },
            { 80, 18 },
            { 82, 19 },
            { 86, 20 },
            { 88, 21 },
            { 90, 22 },
            { 91, 23 },
            { 94, 24 },
            { 95, 25 },
            { 104, 26 },
            { 106, 27 },
            { 107, 28 },
            { 120, 29 },
            { 122, 30 },
            { 123, 31 },
            { 126, 32 },
            { 127, 33 },
            { 208, 34 },
            { 210, 35 },
            { 214, 36 },
            { 216, 37 },
            { 218, 38 },
            { 219, 39 },
            { 222, 40 },
            { 223, 41 },
            { 248, 42 },
            { 250, 43 },
            { 251, 44 },
            { 254, 45 },
            { 255, 46 },
            { 0, 47 }
        };

        public BitMask()
        {
        }

        public int FindValue<T>(BitMaskType bitMaskType, List<T> gridSquares, T gridSquare)
            where T : IGridSquare
        {
            switch (bitMaskType)
            {
                case BitMaskType.Bits4:
                    return FindValueWith4Bits(gridSquares, gridSquare);
                case BitMaskType.Bits8:
                    return FindValueWith8Bits(gridSquares, gridSquare);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Finds the value of the GridSquare, considering only the directly connected 4 tiles.
        /// </summary>
        ///
        /// The GridSquares have the following values:
        ///     [1]
        /// [2] [ ] [4]
        ///     [8]
        /// If a neighboring tile is present, it contributes to the value. Therefore, there are
        /// 16 unique textures.
        int FindValueWith4Bits<T>(List<T> gridSquares, IGridSquare gridSquare)
            where T : IGridSquare
        {
            bool above = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx && gs.YIdx == gridSquare.YIdx - 1);
            bool right = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx + 1 && gs.YIdx == gridSquare.YIdx);
            bool below = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx && gs.YIdx == gridSquare.YIdx + 1);
            bool left = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx - 1 && gs.YIdx == gridSquare.YIdx);
            int val = above ? 1 : 0;
            val += right ? 2 : 0;
            val += below ? 4 : 0;
            val += left ? 8 : 0;
            return val;
        }

        /// <summary>
        /// Finds the value of the GridSquare, considering all surrounding 8 tiles.
        /// </summary>
        ///
        /// The GridSquares have the following values:
        /// [ 1 ] [ 2 ] [ 4 ]
        /// [ 8 ] [   ] [ 16]
        /// [ 32] [ 64] [128]
        ///
        /// There is something else to consider though: When a corner is present, without ANY of it's horizontal or
        /// vertical neighbors, it doesn't contribute towards the value. E.g. if There was the top-left (1), top-
        /// middle (2), and middle-right (16), then this would look identical to a value of 18 (2 + 16). So the 1
        /// isn't counted.
        ///
        /// Therefore, there are only 47 unique textures.
        /// The _8BitRemap is used to convert the large numbers into this range.
        private int FindValueWith8Bits<T>(List<T> gridSquares, IGridSquare gridSquare)
            where T : IGridSquare
        {
            bool above = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx && gs.YIdx == gridSquare.YIdx - 1);
            bool right = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx + 1 && gs.YIdx == gridSquare.YIdx);
            bool below = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx && gs.YIdx == gridSquare.YIdx + 1);
            bool left = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx - 1 && gs.YIdx == gridSquare.YIdx);
            bool aboveLeft = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx - 1 && gs.YIdx == gridSquare.YIdx - 1);
            bool aboveRight = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx + 1 && gs.YIdx == gridSquare.YIdx - 1);
            bool belowLeft = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx - 1 && gs.YIdx == gridSquare.YIdx + 1);
            bool belowRight = gridSquares.Any(gs => gs.XIdx == gridSquare.XIdx + 1 && gs.YIdx == gridSquare.YIdx + 1);

            int val = (aboveLeft && above && left) ? 1 : 0;
            val += above ? 2 : 0;
            val += (aboveRight && above && right) ? 4 : 0;
            val += left ? 8 : 0;
            val += right ? 16 : 0;
            val += (belowLeft && below && left) ? 32 : 0;
            val += below ? 64 : 0;
            val += (belowRight && below && right) ? 128 : 0;

            return _8BitRemap[val];
        }
    }
}
