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
        public const int GRID_SQUARE_SIZE = 16;
        public Vector2 position { get { return GRID_SQUARE_SIZE * new Vector2(xIdx, yIdx); } }
        public int xIdx { get; }
        public int yIdx { get; }

        public GridSquare(int xIdx, int yIdx)
        {
            this.xIdx = xIdx;
            this.yIdx = yIdx;
        }

        public void FrameTick(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE), Color.Red);
        }
    }
}
