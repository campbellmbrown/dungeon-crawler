using dungeoncrawler.Management;
using dungeoncrawler.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.GameStates.PlayingState
{

    public class Floor : GridSquare, HasSpriteSheet
    {
        private readonly ClickManager _clickManager;

        private SpriteSheet _sprite;
        public Entity entity { get; set; }
        public bool hasEntity { get { return entity != null; } }

        public static Dictionary<int, Rectangle> textureRectangleLookup = new Dictionary<int, Rectangle>()
        {
            { 0, new Rectangle(0 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 1, new Rectangle(1 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 2, new Rectangle(2 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 3, new Rectangle(3 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 4, new Rectangle(4 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 5, new Rectangle(5 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 6, new Rectangle(6 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 7, new Rectangle(7 * GRID_SQUARE_SIZE, 0 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 8, new Rectangle(0 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 9, new Rectangle(1 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 10, new Rectangle(2 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 11, new Rectangle(3 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 12, new Rectangle(4 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 13, new Rectangle(5 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 14, new Rectangle(6 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 15, new Rectangle(7 * GRID_SQUARE_SIZE, 1 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 16, new Rectangle(0 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 17, new Rectangle(1 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 18, new Rectangle(2 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 19, new Rectangle(3 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 20, new Rectangle(4 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 21, new Rectangle(5 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 22, new Rectangle(6 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 23, new Rectangle(7 * GRID_SQUARE_SIZE, 2 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 24, new Rectangle(0 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 25, new Rectangle(1 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 26, new Rectangle(2 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 27, new Rectangle(3 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 28, new Rectangle(4 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 29, new Rectangle(5 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 30, new Rectangle(6 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 31, new Rectangle(7 * GRID_SQUARE_SIZE, 3 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 32, new Rectangle(0 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 33, new Rectangle(1 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 34, new Rectangle(2 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 35, new Rectangle(3 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 36, new Rectangle(4 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 37, new Rectangle(5 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 38, new Rectangle(6 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 39, new Rectangle(7 * GRID_SQUARE_SIZE, 4 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 40, new Rectangle(0 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 41, new Rectangle(1 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 42, new Rectangle(2 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 43, new Rectangle(3 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 44, new Rectangle(4 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 45, new Rectangle(5 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 46, new Rectangle(6 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
            { 47, new Rectangle(7 * GRID_SQUARE_SIZE, 5 * GRID_SQUARE_SIZE, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE) },
        };

        public Floor(GridManager gridManager, ClickManager clickManager, int xIdx, int yIdx)
            : base(gridManager, xIdx, yIdx)
        {
            _clickManager = clickManager;
            _clickManager.AddLeftClick(new RectangleF(position.X, position.Y, GRID_SQUARE_SIZE, GRID_SQUARE_SIZE), SetPlayerDestination);
            _sprite = new SpriteSheet(Game1.textures["gray_brick_floors"], textureRectangleLookup, 0);
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
            _sprite.Draw(spriteBatch, position, opacity);
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

        public void UpdateID(int id)
        {
            _sprite.ChangeTextureRectangle(id);
        }
    }
}
