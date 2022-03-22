using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Utility
{
    public static class Conversion
    {
        public static Vector2 PointToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Point Vector2ToPoint(Vector2 vector2)
        {
            return new Point((int)vector2.X, (int)vector2.Y);
        }
    }

    public static class RNG
    {
        public static T ChooseRandom<T>(List<T> list)
        {
            int idx = Game1.Random.Next(list.Count);
            return list[idx];
        }

        public class Weight
        {
            public int Value { get; set; }
        }

        public static T ChooseWeighted<T>(List<T> list, Dictionary<T, Weight> weights)
        {
            int totalWeights = list.Sum(item => weights[item].Value);
            int rand = Game1.Random.Next(totalWeights);
            int check = 0;
            T ret = default;
            for (int idx = 0; idx < list.Count; idx++)
            {
                check += weights[list[idx]].Value;
                if (rand < check)
                {
                    ret = list[idx];
                    break;
                }
            }
            return ret;
        }

        public static T RandomEnum<T>()
        {
            Type type = typeof(T);
            Array values = Enum.GetValues(type);
            object value = values.GetValue(Game1.Random.Next(values.Length));
            return (T)value;
        }

        public static bool PercentChance(int percent)
        {
            return Game1.Random.Next(100) < percent;
        }

        public static int Gaussian(int mean, int stdDev, int lowLimit = -999, int upLimit = 999)
        {
            // Using the Irwin-Hall method:
            double val = 0;
            for (int idx = 0; idx < 12; idx++)
            {
                val += Game1.Random.NextDouble(); // [0, 0)
            }
            val -= 6;
            val = mean + (val * stdDev);
            int ret = (int)Math.Round(val);
            return Math.Clamp(ret, lowLimit, upLimit);
        }
    }
}
