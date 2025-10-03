using System;
using System.Collections.Generic;
using System.Numerics;
using AI_Model.Utilities;
using CustomMath;

namespace AI_Model.Pathfinding
{
    public class Grid<NodeType> : Graph<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        private int width;
        private int height;
        public float nodeSize;
        private float nodeSpacing;

        public int Width => width;
        public int Height => height;

        public Grid(int x, int y, float nodeSize, float nodeSpacing)
        {
            width = x;
            height = y;
            this.nodeSize = nodeSize;
            this.nodeSpacing = nodeSpacing;
            PopulateGraph();
        }

        public override void PopulateGraph()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    NodeType node = new NodeType();
                    node.SetCoordinate(new Vec2Int(j, i));
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

        public Vec3 ToGridAligned(Vec2Int nodePosition)
        {
            float offset = nodeSize + nodeSpacing / 2;
            return new Vec3(nodePosition.X * offset,
                nodePosition.Y * offset);
        }

        public Vec3 ToEntityGridAligned(Vec2Int nodePosition)
        {
            float offset = nodeSize + nodeSpacing / 2;

            return new Vec3(nodePosition.X * offset,
                nodePosition.Y * offset, -1.0f);
        }
    }
}