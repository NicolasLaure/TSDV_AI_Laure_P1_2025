using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    public class GridView : MonoBehaviour
    {
        public Grid<Node<Vector2Int>> grid;
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridHeight;
        [SerializeField] private Vector3 cubeSize;
        [SerializeField] private float GridSpacing;
        [SerializeField] private GameObject nodePrefab;

        private Dictionary<Node<Vector2Int>, NodeView> nodeToView = new Dictionary<Node<Vector2Int>, NodeView>();

        void Awake()
        {
            grid = new Grid<Node<Vector2Int>>(gridWidth, gridHeight);
            foreach (Node<Vector2Int> node in grid.nodes)
            {
                GameObject nodeObject = Instantiate(nodePrefab, ToGridAligned(node.GetCoordinate()), Quaternion.identity);
                NodeView nodeView = nodeObject.GetComponent<NodeView>();
                nodeView.Init(node);
                nodeToView.Add(node, nodeView);
                nodeObject.transform.localScale = cubeSize;
            }
        }

        public Vector3 ToGridAligned(Vector2Int nodePosition)
        {
            return new Vector3(nodePosition.x * (cubeSize.x + GridSpacing / 2), nodePosition.y * (cubeSize.y + GridSpacing / 2));
        }

        public void PaintPath(List<Node<Vector2Int>> path, bool shouldDraw)
        {
            foreach (Node<Vector2Int> node in path)
            {
                nodeToView[node].SetPath(shouldDraw);
            }
        }
    }
}