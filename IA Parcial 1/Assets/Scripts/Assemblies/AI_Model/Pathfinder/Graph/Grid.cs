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
                    neighbours.Add(nodes[i + width * (height - 1)]);

                //!BottomLimit
                if (MathF.Floor(i / width) + 1 < height)
                    neighbours.Add(nodes[i + width]);
                else
                    neighbours.Add(nodes[i - width * (height - 1)]);

                nodeToNeighbours.Add(nodes[i], neighbours);
            }
        }

        public override int Distance(NodeType A, NodeType B)
        {
            Vec2Int aPos = A.GetCoordinate();
            Vec2Int bPos = B.GetCoordinate();

            return (int)MathF.Abs(aPos.X - bPos.X) + (int)MathF.Abs(aPos.Y - bPos.Y);
        }
    }
}