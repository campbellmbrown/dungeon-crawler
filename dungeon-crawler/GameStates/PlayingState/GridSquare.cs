using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

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

        private readonly GridManager _gridManager;
        private readonly ClickManager _clickManager;

        public const int GRID_SQUARE_SIZE = 16;
        private const float OPACITY_RATE = 1.0f; // 0 -> 1 opacity in 1 second.

        public Vector2 position { get { return GRID_SQUARE_SIZE * new Vector2(xIdx, yIdx); } }
        public int xIdx { get; }
        public int yIdx { get; }
        public VisibilityState visibilityState { get; set; }
        private float _opacity = 0;

        public Entity entity { get; set; }
        public bool hasEntity { get { return entity != null; } }

        public GridSquare(GridManager gridManager, ClickManager clickManager, int xIdx, int yIdx)
        {
            _gridManager = gridManager;
            _clickManager = clickManager;
            this.xIdx = xIdx;
            this.yIdx = yIdx;
            visibilityState = VisibilityState.NotVisible;
            // TODO: move to the child class for floor gridsquares.
            _clickManager.AddLeftClick(new RectangleF(position.X, position.Y, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE), SetPlayerDestination);
        }

        public void FrameTick(GameTime gameTime)
        {
            if (hasEntity)
            {
                entity.FrameTick(gameTime);
            }
            switch (visibilityState)
            {
                case VisibilityState.FadingOut:
                    // Fade out the opacity until it reaches 0.
                    _opacity -= OPACITY_RATE * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_opacity <= 0)
                    {
                        _opacity = 0;
                        visibilityState = VisibilityState.NotVisible;
                        _gridManager.RemoveFromDrawables(this);
                    }
                    break;
                case VisibilityState.FadingIn:
                    // Fade in the opacity until it reaches 1.
                    _opacity += OPACITY_RATE * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_opacity >= 1)
                    {
                        _opacity = 1;
                        visibilityState = VisibilityState.Visible;
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE), Color.Red * _opacity);
            if (hasEntity)
            {
                entity.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Performs the action tick for the GridSquare state machine.
        /// </summary>
        /// <remarks>
        /// The xIdxOfFocus, yIdxOfFocus, and range could go into a Focus structure if desired.
        /// </remarks>
        /// <param name="xIdx">The X index of where the focus is. </param>
        /// <param name="yIdx">The Y index of where the focus is. </param>
        /// <param name="range">The range away from the focus to initiate a state change.</param>
        public void ActionTick(int xIdxOfFocus, int yIdxOfFocus, int range)
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
                        _gridManager.AddToDrawables(this);
                    }
                    break;
            }
        }

        public void SetPlayerDestination()
        {
            _gridManager.SetPlayerDestination(this);
        }

        public bool Busy()
        {
            // TODO: Move to an entity manager
            if (entity != null)
            {
                return entity.Busy();
            }
            else
            {
                return false;
            }
        }
    }
}
