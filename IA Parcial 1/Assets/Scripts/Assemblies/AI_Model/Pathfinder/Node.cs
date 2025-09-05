using Pathfinder;
using UnityEngine;

public class Node<Coordinate> : INode, INode<Coordinate>
{
    private Coordinate coordinate;
    private bool isBlocked = false;

    public void SetCoordinate(Coordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public bool IsBlocked()
    {
        return isBlocked;
    }

    public void ToggleBlock()
    {
        Debug.Log($"Is Blocked: {isBlocked}");
        isBlocked = !isBlocked;
    }
}