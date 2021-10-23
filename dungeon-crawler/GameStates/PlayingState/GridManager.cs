using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class GridManager
    {
        private List<GridSquare> _gridSquares;

        public GridManager()
        {
            _gridSquares = new List<GridSquare>();
            _gridSquares.Add(new GridSquare(0, 0));
            _gridSquares.Add(new GridSquare(1, 0));
            _gridSquares.Add(new GridSquare(0, 1));
            _gridSquares.Add(new GridSquare(1, 1));
        }

        public void FrameTick(GameTime gameTime)
        {
            foreach (var gridSquare in _gridSquares)
            {
                gridSquare.FrameTick(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var gridSquare in _gridSquares)
            {
                gridSquare.Draw(spriteBatch);
            }
        }
    }
}
