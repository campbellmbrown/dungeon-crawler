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

        private Entity _entity;
        public Entity entity
        {
            get
            {
                return _entity;
            }
            set
            {
                _entity = value;
                if (_entity != null)
                {
                    _entity.ChangeGridSquare(this);
                }
            }
        }
        public bool hasEntity { get { return entity != null; } }

        public GridSquare(int xIdx, int yIdx)
        {
            this.xIdx = xIdx;
            this.yIdx = yIdx;
        }

        public void FrameTick(GameTime gameTime)
        {
            if (hasEntity)
            {
                entity.FrameTick(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE), Color.Red);
            if (hasEntity)
            {
                entity.Draw(spriteBatch);
            }
        }
    }
}
