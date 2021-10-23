using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.GameStates
{
    public interface IGameState
    {
        public void FrameTick(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch);
    }
}
