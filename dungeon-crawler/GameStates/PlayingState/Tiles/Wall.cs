using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Wall : GridSquare
    {
        public Wall(GridManager gridManager, int xIdx, int yIdx) :
            base(gridManager, xIdx, yIdx)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE), Color.Green * opacity);
            base.Draw(spriteBatch);
        }
    }
}
