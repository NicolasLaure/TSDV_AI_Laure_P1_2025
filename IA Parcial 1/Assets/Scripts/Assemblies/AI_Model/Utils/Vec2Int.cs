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
    }
}