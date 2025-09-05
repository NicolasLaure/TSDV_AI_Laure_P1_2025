using System.Collections.Generic;
using Pathfinder;

public class BreadthFirstPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode<UnityEngine.Vector2Int>, INode, new()
{
    protected override int Distance(NodeType A, NodeType B)
    {
        throw new System.NotImplementedException();
    }

    public override List<NodeType> FindPath(NodeType startNode, NodeType destinationNode)
    {
        throw new System.NotImplementedException();
    }

    protected override List<NodeType> GetValidNeighbours(NodeType node)
    {
        throw new System.NotImplementedException();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b)
    {
        throw new System.NotImplementedException();
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        throw new System.NotImplementedException();
    }
}