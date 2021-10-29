using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Entity
    {
        protected GridSquare gridSquare { get; set; }

        public Entity()
        {
        }

        public void ChangeGridSquare(GridSquare gridSquare)
        {
            this.gridSquare = gridSquare;
        }

        public virtual void FrameTick(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(gridSquare.position.X, gridSquare.position.Y, 4, 4), Color.Yellow);
        }

        public virtual void ActionTick()
        {

        }
    }
}
