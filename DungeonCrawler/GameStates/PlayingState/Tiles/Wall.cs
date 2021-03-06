using System.Collections.Generic;
using DungeonCrawler.Visual;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameStates.PlayingState
{
    public interface IWall : IGridSquare, IMyDrawable, IHasSpriteSheet
    {
    }

    public class Wall : GridSquare, IWall
    {
        public const int WALL_HEIGHT = 23;
        private readonly SpriteSheet _sprite;

        public static Dictionary<int, Rectangle> TextureRectangleLookup { get; } = new Dictionary<int, Rectangle>()
        {
            { 0, new Rectangle(0 * GRID_SQUARE_SIZE, 0 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 1, new Rectangle(1 * GRID_SQUARE_SIZE, 0 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 2, new Rectangle(2 * GRID_SQUARE_SIZE, 0 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 3, new Rectangle(3 * GRID_SQUARE_SIZE, 0 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 4, new Rectangle(0 * GRID_SQUARE_SIZE, 1 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 5, new Rectangle(1 * GRID_SQUARE_SIZE, 1 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 6, new Rectangle(2 * GRID_SQUARE_SIZE, 1 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 7, new Rectangle(3 * GRID_SQUARE_SIZE, 1 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 8, new Rectangle(0 * GRID_SQUARE_SIZE, 2 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 9, new Rectangle(1 * GRID_SQUARE_SIZE, 2 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 10, new Rectangle(2 * GRID_SQUARE_SIZE, 2 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 11, new Rectangle(3 * GRID_SQUARE_SIZE, 2 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 12, new Rectangle(0 * GRID_SQUARE_SIZE, 3 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 13, new Rectangle(1 * GRID_SQUARE_SIZE, 3 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 14, new Rectangle(2 * GRID_SQUARE_SIZE, 3 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) },
            { 15, new Rectangle(3 * GRID_SQUARE_SIZE, 3 * WALL_HEIGHT, GRID_SQUARE_SIZE, WALL_HEIGHT) }
        };


        public Wall(ILogManager logManager, IGridManager gridManager, int xIdx, int yIdx) :
            base(gridManager, xIdx, yIdx)
        {
            _sprite = new SpriteSheet(logManager, Game1.Textures["gray_brick_walls"], TextureRectangleLookup, 0);
        }

        public void Draw(ISpriteBatchWrapper spriteBatch)
        {
            Vector2 offset = new Vector2(0, GRID_SQUARE_SIZE - WALL_HEIGHT);
            _sprite.Draw(spriteBatch, Position + offset, FindLayerDepth());
        }

        public void UpdateID(int id)
        {
            _sprite.ChangeTextureRectangle(id);
        }
    }
}
