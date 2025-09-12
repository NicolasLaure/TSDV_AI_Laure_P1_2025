using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using AI_Model.Voronoi;
using UnityEngine;

namespace AI_View.Pathfinding
{
    public class GridView : MonoBehaviour
    {
        public Grid<Node<Vec2Int>> grid;
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridHeight;
        [SerializeField] private Vector3 cubeSize;
        [SerializeField] private float GridSpacing;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject minePrefab;

        [Header("Voronoi")] [SerializeField] private int landmarksQty;

        private Dictionary<INode, NodeView> nodeToView = new Dictionary<INode, NodeView>();

        public Voronoi<Node<Vec2Int>> voronoi;
        private List<Node<Vec2Int>> landMarks = new List<Node<Vec2Int>>();

        private List<GameObject> landMarkObjects = new List<GameObject>();

        void Awake()
        {
            Camera.main.transform.position = new Vector3(gridWidth / 2 * cubeSize.x, gridHeight / 2* cubeSize.y, -gridWidth);
            grid = new Grid<Node<Vec2Int>>(gridWidth, gridHeight);
            foreach (Node<Vec2Int> node in grid.nodes)
            {
                GameObject nodeObject =
                    Instantiate(nodePrefab, ToGridAligned(node.GetCoordinate()), Quaternion.identity);
                NodeView nodeView = nodeObject.GetComponent<NodeView>();
                nodeView.Init(node);
                nodeToView.Add(node, nodeView);
                nodeObject.transform.localScale = cubeSize;
            }

            voronoi = new Voronoi<Node<Vec2Int>>(grid);
        }

        public Vector3 ToGridAligned(Vec2Int nodePosition)
        {
            return new Vector3(nodePosition.X * (cubeSize.x + GridSpacing / 2),
                nodePosition.Y * (cubeSize.y + GridSpacing / 2));
        }

        public void PaintPath(List<Node<Vec2Int>> path, bool shouldDraw)
        {
            foreach (Node<Vec2Int> node in path)
            {
                nodeToView[node].SetPath(shouldDraw);
            }
        }

        [ContextMenu("Bake")]
        public void BakeVoronoi()
        {
            landMarks.Clear();
            for (int i = 0; i < landmarksQty; i++)
            {
                Node<Vec2Int> newLandmark;
                do
                {
                    newLandmark = grid.nodes[Random.Range(0, grid.nodes.Count)];
                } while (landMarks.Contains(newLandmark));

                landMarks.Add(newLandmark);
            }

            voronoi.Bake(landMarks);

            foreach (GameObject landMarkObject in landMarkObjects)
            {
                Destroy(landMarkObject);
            }

            landMarkObjects.Clear();

            foreach (Node<Vec2Int> landmark in landMarks)
            {
                Vector3 minePos = ToGridAligned(landmark.GetCoordinate());
                minePos.z = -1;
                landMarkObjects.Add(Instantiate(minePrefab, minePos, Quaternion.identity));
                PaintNodes(voronoi.GetLandmarkNodes(landmark));
            }
        }

        private void PaintNodes(List<Node<Vec2Int>> nodes)
        {
            Color randomColor = Random.ColorHSV();
            randomColor.a = 1;
            foreach (Node<Vec2Int> node in nodes)
            {
                nodeToView[node].SetAreaColor(randomColor);
            }
        }
    }
}