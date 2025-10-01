using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using AI_View.Pathfinding;
using RTS.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RTS.View
{
    public class RTS_MapView : MonoBehaviour
    {
        [SerializeField] private Vector3 cubeSize;
        [SerializeField] private float GridSpacing;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject minePrefab;
        [SerializeField] private GameObject hqPrefab;
        [SerializeField] private Mesh nodeMesh;
        [SerializeField] private Material baseMaterial;

        [Header("Entities")]
        [SerializeField] private Mesh mineMesh;
        [SerializeField] private Vector3 mineSize;
        [SerializeField] private Material mineMaterial;
        [SerializeField] private Mesh hqMesh;
        [SerializeField] private Vector3 hqSize;
        [SerializeField] private Material hqMaterial;
        private const int MAX_OBJS_PER_DRAWCALL = 1000;

        [Header("Voronoi")]
        [SerializeField] private int landmarksQty;

        private Map map;
        private Dictionary<INode, NodeView> nodeToView = new Dictionary<INode, NodeView>();
        private List<GameObject> landMarkObjects = new List<GameObject>();

        private Dictionary<MapNode, GameObject> nodeToHeldGameObject = new Dictionary<MapNode, GameObject>();

        private Type shownVoronoi;
        private Dictionary<int, Transitability> typeToWeight;

        private Dictionary<MapNode, Material> landMarkToAreaMaterial = new Dictionary<MapNode, Material>();

        public void Init(Map map, Type shownVoronoi, Dictionary<int, Transitability> typeToWeight)
        {
            this.map = map;
            this.shownVoronoi = shownVoronoi;
            this.typeToWeight = typeToWeight;

            Camera.main.transform.position =
                new Vector3(map.grid.Width / 2 * cubeSize.x, map.grid.Height / 2 * cubeSize.y, -map.grid.Width);
            foreach (MapNode node in map.grid.nodes)
            {
                GameObject nodeObject = Instantiate(nodePrefab, ToGridAligned(node.GetCoordinate()), Quaternion.identity);
                NodeView nodeView = nodeObject.GetComponent<NodeView>();
                nodeView.Init(node, typeToWeight);
                nodeView.position = nodeObject.transform.position;
                nodeToView.Add(node, nodeView);
                nodeObject.transform.localScale = cubeSize;
            }

            AddHeadQuarters(map.hqNode);
            AddMines(map.GetMineLocations());

            PaintVoronoiAreas();
            map.onMineRemove += RemoveMine;
            map.onBake += PaintVoronoiAreas;
        }

        public void Draw()
        {
            List<NodeView> entities = new List<NodeView>(nodeToView.Values);
            for (int i = 0; i < entities.Count; i++)
                Graphics.DrawMesh(nodeMesh, Matrix4x4.TRS(entities[i].position, Quaternion.identity, cubeSize), entities[i].CurrentMaterial, 0);

            Graphics.DrawMesh(hqMesh, Matrix4x4.TRS(ToEntityGridAligned(map.hqNode.GetCoordinate()), Quaternion.identity, hqSize), hqMaterial, 0);
            DrawMines();
        }

        private void DrawMines()
        {
            List<MapNode> mines = map.GetMineLocations();
            List<Matrix4x4[]> drawMatrix = new List<Matrix4x4[]>();
            int meshes = mines.Count;
            for (int i = 0; i < mines.Count; i += MAX_OBJS_PER_DRAWCALL)
            {
                drawMatrix.Add(new Matrix4x4[meshes > MAX_OBJS_PER_DRAWCALL ? MAX_OBJS_PER_DRAWCALL : meshes]);
                meshes -= MAX_OBJS_PER_DRAWCALL;
            }

            Parallel.For(0, mines.Count, i =>
            {
                drawMatrix[(i / MAX_OBJS_PER_DRAWCALL)][(i % MAX_OBJS_PER_DRAWCALL)]
                    .SetTRS(ToEntityGridAligned(mines[i].GetCoordinate()), Quaternion.identity, mineSize);
            });
            for (int i = 0; i < drawMatrix.Count; i++)
            {
                Graphics.DrawMeshInstanced(mineMesh, 0, mineMaterial, drawMatrix[i]);
            }
        }

        public Vector3 ToGridAligned(Vec2Int nodePosition)
        {
            return new Vector3(nodePosition.X * (cubeSize.x + GridSpacing / 2),
            nodePosition.Y * (cubeSize.y + GridSpacing / 2));
        }

        public Vector3 ToEntityGridAligned(Vec2Int nodePosition)
        {
            return new Vector3(nodePosition.X * (cubeSize.x + GridSpacing / 2),
            nodePosition.Y * (cubeSize.y + GridSpacing / 2), -1.0f);
        }

        public void AddMines(List<MapNode> mines)
        {
            Debug.Log($"MinesQty: {mines.Count}");
            foreach (MapNode mine in mines)
            {
                AddMine(mine);
                Material mat = Material.Instantiate(baseMaterial);
                Color color = Random.ColorHSV();
                color.a = 0.2f;
                mat.color = color;
                mat.enableInstancing = true;
                landMarkToAreaMaterial.Add(mine, mat);
            }

            foreach (Material material in landMarkToAreaMaterial.Values)
            {
                Debug.Log($"Mat Color: {material.color}");
            }
        }

        public void AddMine(MapNode mineLocation)
        {
            if (nodeToHeldGameObject.ContainsKey(mineLocation))
                return;

            Vector3 minePos = ToEntityGridAligned(mineLocation.GetCoordinate());
            GameObject mineObject = Instantiate(minePrefab, minePos, Quaternion.identity);

            landMarkObjects.Add(mineObject);
            nodeToHeldGameObject.Add(mineLocation, mineObject);
        }

        public void RemoveMine(MapNode mine)
        {
            if (!nodeToHeldGameObject.ContainsKey(mine))
                return;

            Destroy(nodeToHeldGameObject[mine]);
            nodeToHeldGameObject.Remove(mine);

            foreach (MapNode landmark in map.agentTypeToVoronoi[shownVoronoi].Landmarks)
                PaintNodes(map.agentTypeToVoronoi[shownVoronoi].GetLandmarkNodes(landmark), landmark);
        }

        public void AddHeadQuarters(MapNode hqLocation)
        {
            Vector3 hqPos = ToEntityGridAligned(hqLocation.GetCoordinate());
            GameObject hqObject = Instantiate(hqPrefab, hqPos, Quaternion.identity);
            landMarkObjects.Add(hqObject);
            nodeToHeldGameObject.Add(hqLocation, hqObject);
        }

        private void PaintNodes(List<MapNode> nodes, MapNode landMark)
        {
            foreach (MapNode node in nodes)
            {
                if (landMark != null && landMarkToAreaMaterial.ContainsKey(landMark))
                    nodeToView[node].SetAreaMaterial(landMarkToAreaMaterial[landMark]);
                else
                    nodeToView[node].SetAreaMaterial(null);
            }
        }

        private void PaintVoronoiAreas()
        {
            map.agentTypeToVoronoi[shownVoronoi].SetLandmarkNodes();

            foreach (MapNode landmark in map.agentTypeToVoronoi[shownVoronoi].Landmarks)
                PaintNodes(map.agentTypeToVoronoi[shownVoronoi].GetLandmarkNodes(landmark), landmark);
        }

        public void ShowVoronoi(bool shouldShow)
        {
            if (shouldShow)
                PaintVoronoiAreas();
            else
                PaintNodes(map.grid.nodes, null);
        }

        public void SetShownVoronoi(Type newType, Dictionary<int, Transitability> typeToWeight)
        {
            shownVoronoi = newType;
            this.typeToWeight = typeToWeight;
            PaintVoronoiAreas();
        }
    }
}