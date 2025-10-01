using System;
using System.Collections.Generic;
using AI_Model.Utilities;

namespace AI_Model.Pathfinding
{
    public class Grid<NodeType> : Graph<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        private int width;
        private int height;

        public int Width => width;
        public int Height => height;

        public Grid(int x, int y)
        {
            width = x;
            height = y;
            PopulateGraph();
        }

        public override void PopulateGraph()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    NodeType node = new NodeType();
                    node.SetCoordinate(new Vec2Int(i, j));
                    node.SetLatitude(GetLatitude(node));
                    node.SetLongitude(GetLongitude(node));
                    nodes.Add(node);
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                List<NodeType> neighbours = new List<NodeType>();
                //!LeftLimit
                if (i % width != 0)
                    neighbours.Add(nodes[i - 1]);
                else
                    neighbours.Add(nodes[i + width - 1]);

                //!RightLimit
                if ((i + 1) % width != 0)
                    neighbours.Add(nodes[i + 1]);
                else
                    neighbours.Add(nodes[i - width + 1]);

                //!TopLimit
                if (MathF.Floor(i / width) > 0)
                    neighbours.Add(nodes[i - width]);
                else
                {
                    int neighbourI = width - 1 - i;
                    if (neighbourI != i)
                        neighbours.Add(nodes[neighbourI]);
                }

                //!BottomLimit
                if (MathF.Floor(i / width) + 1 < height)
                    neighbours.Add(nodes[i + width]);
                else
                {
                    int neighbourI = width * height - 1 - (i - width * (height - 1));
                    if (neighbourI != i)
                        neighbours.Add(nodes[neighbourI]);
                }

                nodeToNeighbours.Add(nodes[i], neighbours);
            }
        }

        public override int Distance(NodeType A, NodeType B)
        {
            Vec2Int aPos = A.GetCoordinate();
            Vec2Int bPos = B.GetCoordinate();

            return (int)MathF.Abs(aPos.X - bPos.X) + (int)MathF.Abs(aPos.Y - bPos.Y);
        }

        public float GetLongitude(NodeType node)
        {
            return 360 / width * node.GetCoordinate().X;
        }

        public float GetLatitude(NodeType node)
        {
            return 180 / (height + 1) * (node.GetCoordinate().Y + 1);
        }
    }
}