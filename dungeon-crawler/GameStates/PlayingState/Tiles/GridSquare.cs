using System;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameStates.PlayingState
{
    public class GridSquare
    {
        protected readonly GridManager _gridManager;

        public const int GRID_SQUARE_SIZE = 16;

        public Vector2 Position { get { return GRID_SQUARE_SIZE * new Vector2(XIdx, YIdx); } }
        public int XIdx { get; }
        public int YIdx { get; }
        public bool Visible { get; set; } = false;

        public GridSquare(GridManager gridManager, int xIdx, int yIdx)
        {
            this._gridManager = gridManager;
            this.XIdx = xIdx;
            this.YIdx = yIdx;
        }

        public virtual void FrameTick(GameTime gameTime)
        {
            // Do nothing.
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Do nothing.
        }

        /// <summary>
        /// Performs the action tick for the GridSquare state machine.
        /// </summary>
        /// <remarks>
        /// The xIdxOfFocus, yIdxOfFocus, and range could go into a Focus structure if desired.
        /// </remarks>
        /// <param name="xIdx">The X index of where the focus is.</param>
        /// <param name="yIdx">The Y index of where the focus is.</param>
        /// <param name="range">The range away from the focus to initiate a state change.</param>
        public virtual void ActionTick(int xIdxOfFocus, int yIdxOfFocus, int range)
        {
            bool withinRange = (Math.Abs(xIdxOfFocus - XIdx) < range) && (Math.Abs(yIdxOfFocus - YIdx) < range);

            if (!Visible && withinRange)
            {
                Visible = true;
                _gridManager.AddToDrawables(this);
            }
            else if (Visible && !withinRange)
            {
                Visible = false;
                _gridManager.RemoveFromDrawables(this);
            }
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
