using Microsoft.Xna.Framework;

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
    }
}
