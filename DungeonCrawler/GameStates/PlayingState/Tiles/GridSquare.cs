using System;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IGridSquare
    {
        Vector2 Position { get; }
        int XIdx { get; set; }
        int YIdx { get; set; }
    }

    public class GridSquare : IGridSquare
    {
        protected readonly IGridManager _gridManager;

        public const int GRID_SQUARE_SIZE = 16;

        public Vector2 Position { get { return GRID_SQUARE_SIZE * new Vector2(XIdx, YIdx); } }
        public int XIdx { get; set; }
        public int YIdx { get; set; }

        public GridSquare(IGridManager gridManager, int xIdx, int yIdx)
        {
            this._gridManager = gridManager;
            this.XIdx = xIdx;
            this.YIdx = yIdx;
        }

        /// <summary>
        /// Finds the layer depth value for drawing.
        /// </summary>
        /// Calculates the layer depth by using the min and max position of the GridSquares, finding where
        /// the position of interest lies between these two values, and converts it to a value between
        /// FOREGROUND_CONTENT_TOP and FOREGROUND_CONTENT_BOTTOM.
        public float FindLayerDepth()
        {
            float positionRatio = (Position.Y - _gridManager.MinY) / (_gridManager.MaxY - _gridManager.MinY);
            return DrawOrder.FOREGROUND_CONTENT_BOTTOM + (positionRatio * (DrawOrder.FOREGROUND_CONTENT_TOP - DrawOrder.FOREGROUND_CONTENT_BOTTOM));
        }
    }
}
