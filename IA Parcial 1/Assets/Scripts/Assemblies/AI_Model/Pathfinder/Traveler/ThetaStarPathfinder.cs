using System;
using System.Collections.Generic;
using AI_Model.Utilities;

namespace AI_Model.Pathfinding
{
    public class ThetaStarPathfinder<NodeType> : Pathfinder<NodeType>
        where NodeType : INode<Vec2Int>, INode, new()
    {
        private AStarPathfinder<NodeType> aStar;

        public ThetaStarPathfinder(AStarPathfinder<NodeType> aStarPathfinder)
        {
            aStar = aStarPathfinder;
        }

        public override List<NodeType> FindPath(NodeType startNode, NodeType destinationNode)
        {
            List<NodeType> path = aStar.FindPath(startNode, destinationNode);

            int startIndex = 0;
            List<NodeType> itemsToRemove = new List<NodeType>();
            while (startIndex + 1 < path.Count)
            {
                int index = startIndex + 1;
                if (index >= path.Count)
                    break;

                while (index < path.Count)
                {
                    if (path[startIndex].GetTileType() != path[index].GetTileType() ||
                        (path[startIndex].GetCoordinate().X != path[index].GetCoordinate().X &&
                         path[startIndex].GetCoordinate().Y != path[index].GetCoordinate().Y))
                        break;

                    itemsToRemove.Add(path[index]);
                    index++;
                }

                startIndex = index;
            }

            foreach (NodeType node in itemsToRemove)
            {
                path.Remove(node);
            }

            return path;
        }

        protected override List<NodeType> GetValidNeighbours(NodeType node)
        {
            throw new System.NotImplementedException();
        }

        protected override int Distance(NodeType A, NodeType B)
        {
            Vec2Int aPos = A.GetCoordinate();
            Vec2Int bPos = B.GetCoordinate();

            return (int)MathF.Abs(aPos.X - bPos.X) + (int)MathF.Abs(aPos.Y - bPos.Y);
        }

        protected override bool NodesEquals(NodeType A, NodeType B)
        {
            return A.GetCoordinate().X == B.GetCoordinate().X && A.GetCoordinate().Y == B.GetCoordinate().Y;
        }

        protected override int MoveToNeighborCost(NodeType A, NodeType b)
        {
            throw new System.NotImplementedException();
        }
    }
}