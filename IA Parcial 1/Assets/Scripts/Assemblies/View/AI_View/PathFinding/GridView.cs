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
        [SerializeField] private Vector3 cubeSize;
        [SerializeField] private float GridSpacing;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject minePrefab;
        [SerializeField] private GameObject hqPrefab;

        [Header("Voronoi")] [SerializeField] private int landmarksQty;

        private Map map;
        private Dictionary<INode, NodeView> nodeToView = new Dictionary<INode, NodeView>();
        private List<GameObject> landMarkObjects = new List<GameObject>();

        private Dictionary<MapNode, GameObject> nodeToHeldGameObject = new Dictionary<MapNode, GameObject>();

        public void Init(Map map)
        {
            this.map = map;
            Camera.main.transform.position =
                new Vector3(map.grid.Width / 2 * cubeSize.x, map.grid.Height / 2 * cubeSize.y, -map.grid.Width);
            foreach (Node<Vec2Int> node in map.grid.nodes)
            {
                GameObject nodeObject =
                    Instantiate(nodePrefab, ToGridAligned(node.GetCoordinate()), Quaternion.identity);
                NodeView nodeView = nodeObject.GetComponent<NodeView>();
                nodeView.Init(node);
                nodeToView.Add(node, nodeView);
                nodeObject.transform.localScale = cubeSize;
            }

            AddHeadQuarters(map.hqNode);
            AddMines(map.GetMineLocations());
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
            landMarkObjects.Clear();

            foreach (MapNode mine in mines)
            {
                AddMine(mine);
            }
        }

        public void AddMine(MapNode mineLocation)
        {
            if (nodeToHeldGameObject.ContainsKey(mineLocation))
                return;

            Vector3 minePos = ToGridAligned(mineLocation.GetCoordinate());
            minePos.z = -1;
            GameObject mineObject = Instantiate(minePrefab, minePos, Quaternion.identity);

            landMarkObjects.Add(mineObject);
            nodeToHeldGameObject.Add(mineLocation, mineObject);
        }

        public void AddHeadQuarters(MapNode hqLocation)
        {
            Vector3 hqPos = ToGridAligned(hqLocation.GetCoordinate());
            hqPos.z = -1;
            GameObject hqObject = Instantiate(hqPrefab, hqPos, Quaternion.identity);
            landMarkObjects.Add(hqObject);
            nodeToHeldGameObject.Add(hqLocation, hqObject);
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