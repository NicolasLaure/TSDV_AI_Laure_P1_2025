using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using AI_Model.Voronoi;
using RTS.Model;
using UnityEngine;

namespace AI_View.Pathfinding
{
    public class GridView : MonoBehaviour
    {
        public Grid<MapNode> grid;
        [SerializeField] private Vector3 cubeSize;
        [SerializeField] private float GridSpacing;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject minePrefab;

        [Header("Voronoi")] [SerializeField] private int landmarksQty;

        private Dictionary<INode, NodeView> nodeToView = new Dictionary<INode, NodeView>();
        private List<GameObject> landMarkObjects = new List<GameObject>();

        private Dictionary<MapNode, GameObject> nodeToHeldGameObject = new Dictionary<MapNode, GameObject>();

        public void Init(Grid<MapNode> grid)
        {
            this.grid = grid;
            Camera.main.transform.position =
                new Vector3(grid.Width / 2 * cubeSize.x, grid.Height / 2 * cubeSize.y, -grid.Width);
            foreach (Node<Vec2Int> node in grid.nodes)
            {
                GameObject nodeObject =
                    Instantiate(nodePrefab, ToGridAligned(node.GetCoordinate()), Quaternion.identity);
                NodeView nodeView = nodeObject.GetComponent<NodeView>();
                nodeView.Init(node);
                nodeToView.Add(node, nodeView);
                nodeObject.transform.localScale = cubeSize;
            }
        }

        public Vector3 ToGridAligned(Vec2Int nodePosition)
        {
            return new Vector3(nodePosition.X * (cubeSize.x + GridSpacing / 2),
                nodePosition.Y * (cubeSize.y + GridSpacing / 2));
        }

        public void PaintPath(List<MapNode> path, bool shouldDraw)
        {
            foreach (MapNode node in path)
            {
                nodeToView[node].SetPath(shouldDraw);
            }
        }

        public void AddMines(List<MapNode> mines)
        {
            foreach (GameObject mine in landMarkObjects)
            {
                Destroy(mine);
            }

            landMarkObjects.Clear();

            foreach (MapNode mine in mines)
            {
                AddMine(mine);
            }
        }

        public void AddMine(MapNode mineLocation)
        {
            GameObject mineObject = Instantiate(minePrefab, ToGridAligned(mineLocation.GetCoordinate()),
                Quaternion.identity);
            landMarkObjects.Add(mineObject);
            nodeToHeldGameObject.Add(mineLocation, mineObject);
        }

        private void PaintNodes(List<MapNode> nodes)
        {
            Color randomColor = Random.ColorHSV();
            randomColor.a = 1;
            foreach (MapNode node in nodes)
            {
                nodeToView[node].SetAreaColor(randomColor);
            }
        }
    }
}