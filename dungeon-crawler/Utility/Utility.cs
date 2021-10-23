using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace dungeoncrawler.Utility
{
    public static class Conversion
    {
        public static Vector2 PointToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
