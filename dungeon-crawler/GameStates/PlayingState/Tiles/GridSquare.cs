using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class GridSquare
    {
        public enum VisibilityState
        {
            Visible,
            FadingIn,
            FadingOut,
            NotVisible,
        }

        protected readonly GridManager gridManager;

        public const int GRID_SQUARE_SIZE = 16;
        protected const float OPACITY_RATE = 1.0f; // 0 -> 1 opacity in 1 second.

        public Vector2 position { get { return GRID_SQUARE_SIZE * new Vector2(xIdx, yIdx); } }
        public int xIdx { get; }
        public int yIdx { get; }
        public VisibilityState visibilityState { get; set; }
        protected float opacity = 0;

        public GridSquare(GridManager gridManager, int xIdx, int yIdx)
        {
            this.gridManager = gridManager;
            this.xIdx = xIdx;
            this.yIdx = yIdx;
            visibilityState = VisibilityState.NotVisible;
        }

        public virtual void FrameTick(GameTime gameTime)
        {
            switch (visibilityState)
            {
                case VisibilityState.FadingOut:
                    // Fade out the opacity until it reaches 0.
                    opacity -= OPACITY_RATE * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                    {
                        opacity = 0;
                        visibilityState = VisibilityState.NotVisible;
                        gridManager.RemoveFromDrawables(this);
                    }
                    break;
                case VisibilityState.FadingIn:
                    // Fade in the opacity until it reaches 1.
                    opacity += OPACITY_RATE * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity >= 1)
                    {
                        opacity = 1;
                        visibilityState = VisibilityState.Visible;
                    }
                    break;
            }
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
            bool withinRange = ((Math.Abs(xIdxOfFocus - xIdx) < range) && (Math.Abs(yIdxOfFocus - yIdx) < range));

            switch (visibilityState)
            {
                case VisibilityState.Visible:
                    if (!withinRange)
                    {
                        visibilityState = VisibilityState.FadingOut;
                    }
                    break;
                case VisibilityState.FadingIn:
                    if (!withinRange)
                    {
                        visibilityState = VisibilityState.FadingOut;
                    }
                    break;
                case VisibilityState.FadingOut:
                    if (withinRange)
                    {
                        visibilityState = VisibilityState.FadingIn;
                    }
                    break;
                case VisibilityState.NotVisible:
                    if (withinRange)
                    {
                        visibilityState = VisibilityState.FadingIn;
                        gridManager.AddToDrawables(this);
                    }
                    break;
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
            float positionRatio = (position.Y - gridManager.minY) / (gridManager.maxY - gridManager.minY);
            return DrawOrder.FOREGROUND_CONTENT_BOTTOM + (positionRatio * (DrawOrder.FOREGROUND_CONTENT_TOP - DrawOrder.FOREGROUND_CONTENT_BOTTOM));
        }
    }
}
