using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameStates
{
    public interface IGameState : IMyDrawable, IFrameTickable
    {
    }
}
