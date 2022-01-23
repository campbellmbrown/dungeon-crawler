using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dungeoncrawler.GameStates.PlayingState.Tiles
{
    public class BitMask
    {
        public enum BitMaskType
        {
            Bits4,
            Bits8,
        }

        private static Dictionary<int, int> _8BitRemap = new Dictionary<int, int>()
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
            where T : GridSquare
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

        private int FindValueWith4Bits<T>(List<T> _gridSquares, GridSquare gridSquare)
            where T : GridSquare
        {
            bool above = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx && gs.yIdx == gridSquare.yIdx - 1);
            bool right = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx + 1 && gs.yIdx == gridSquare.yIdx);
            bool below = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx && gs.yIdx == gridSquare.yIdx + 1);
            bool left = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx - 1 && gs.yIdx == gridSquare.yIdx);
            int val = above ? 1 : 0;
            val += right ? 2 : 0;
            val += below ? 4 : 0;
            val += left ? 8 : 0;
            return val;
        }

        private int FindValueWith8Bits<T>(List<T> _gridSquares, GridSquare gridSquare)
            where T : GridSquare
        {
            bool above = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx && gs.yIdx == gridSquare.yIdx - 1);
            bool right = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx + 1 && gs.yIdx == gridSquare.yIdx);
            bool below = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx && gs.yIdx == gridSquare.yIdx + 1);
            bool left = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx - 1 && gs.yIdx == gridSquare.yIdx);
            bool aboveLeft = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx - 1 && gs.yIdx == gridSquare.yIdx - 1);
            bool aboveRight = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx + 1 && gs.yIdx == gridSquare.yIdx - 1);
            bool belowLeft = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx - 1 && gs.yIdx == gridSquare.yIdx + 1);
            bool belowRight = _gridSquares.Any(gs => gs.xIdx == gridSquare.xIdx + 1 && gs.yIdx == gridSquare.yIdx + 1);

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
