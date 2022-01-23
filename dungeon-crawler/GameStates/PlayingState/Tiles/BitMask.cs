using System;
using System.Collections.Generic;
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

        public BitMask()
        {
            //  _gridSquares = gridSquares;
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
            GridSquare above = _gridSquares.Find(gs => gs.xIdx == gridSquare.xIdx && gs.yIdx == gridSquare.yIdx - 1);
            GridSquare right = _gridSquares.Find(gs => gs.xIdx == gridSquare.xIdx + 1 && gs.yIdx == gridSquare.yIdx);
            GridSquare below = _gridSquares.Find(gs => gs.xIdx == gridSquare.xIdx && gs.yIdx == gridSquare.yIdx + 1);
            GridSquare left = _gridSquares.Find(gs => gs.xIdx == gridSquare.xIdx - 1 && gs.yIdx == gridSquare.yIdx);
            int val = (above != null) ? 1 : 0;
            val += (right != null) ? 2 : 0;
            val += (below != null) ? 4 : 0;
            val += (left != null) ? 8 : 0;
            return val;
        }

        private int FindValueWith8Bits<T>(List<T> _gridSquares, GridSquare gridSquare)
            where T : GridSquare
        {
            return -1;
        }
    }
}
