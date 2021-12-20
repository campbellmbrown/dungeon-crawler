using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.GameStates.PlayingState
{

    public class Floor : GridSquare
    {
        private readonly ClickManager _clickManager;
        public Entity entity { get; set; }
        public bool hasEntity { get { return entity != null; } }

        public Floor(GridManager gridManager, ClickManager clickManager, int xIdx, int yIdx)
            : base(gridManager, xIdx, yIdx)
        {
            _clickManager = clickManager;
            _clickManager.AddLeftClick(new RectangleF(position.X, position.Y, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE), SetPlayerDestination);
        }

        public override void FrameTick(GameTime gameTime)
        {
            // TODO: Move to an entity manager
            if (hasEntity)
            {
                entity.FrameTick(gameTime);
            }
            base.FrameTick(gameTime);
        }

        public override void ActionTick(int xIdxOfFocus, int yIdxOfFocus, int range)
        {
            // TODO: Move to an entity manager
            if (hasEntity)
            {
                entity.ActionTick();
            }

            base.ActionTick(xIdxOfFocus, yIdxOfFocus, range);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // TODO: Move to an entity manager
            if (hasEntity)
            {
                entity.Draw(spriteBatch);
            }
        }

        public void SetPlayerDestination()
        {
            gridManager.SetPlayerDestination(this);
        }

        public bool Busy()
        {
            // TODO: Move to an entity manager
            if (hasEntity)
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
