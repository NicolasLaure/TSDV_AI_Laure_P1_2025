using System;

namespace AI_Model.Utilities
{
    public class Vec2Int
    {
        private int x = 0;
        private int y = 0;

        public int X
        {
            get => x;
            set => x = value;
        }

        public int Y
        {
            get => y;
            set => y = value;
        }

        public Vec2Int()
        {
            x = 0;
            y = 0;
        }

        public Vec2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec2Int operator +(Vec2Int a, Vec2Int b)
        {
            return new Vec2Int(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2Int operator -(Vec2Int a, Vec2Int b)
        {
            return new Vec2Int(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2Int operator *(Vec2Int a, float scalar)
        {
            return new Vec2Int((int)(a.X * scalar), (int)(a.Y * scalar));
        }

        public static Vec2Int operator /(Vec2Int a, float scalar)
        {
            return new Vec2Int((int)(a.X / scalar), (int)(a.Y / scalar));
        }

        public float Distance(Vec2Int other)
        {
            return (int)MathF.Abs(X - other.X) + (int)MathF.Abs(Y - other.Y);
        }

        public static float Distance(Vec2Int lhs, Vec2Int rhs)
        {
            return (int)MathF.Abs(lhs.X - rhs.X) + (int)MathF.Abs(lhs.Y - rhs.Y);
        }
    }
}