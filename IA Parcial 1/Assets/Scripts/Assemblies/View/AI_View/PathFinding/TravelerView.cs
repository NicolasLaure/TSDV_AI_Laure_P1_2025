using System.Collections;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI_View.Pathfinding
{
    public class TravelerView : MonoBehaviour
    {
        [SerializeField] private GameObject destinationObject;
        [SerializeField] private float sleepDuration = 0.1f;
        public GridView gridView;

        //private DepthFirstPathfinder<Node<Vec2Int>> _pathfinder;
        //private BreadthFirstPathfinder<Node<Vec2Int>> Pathfinder;
        //private DijstraPathfinder<Node<Vec2Int>> Pathfinder;
        private AStarPathfinder<Node<Vec2Int>> Pathfinder;

        private Node<Vec2Int> startNode;
        private Node<Vec2Int> destinationNode;

        private void Start()
        {
            //_pathfinder = new DepthFirstPathfinder<Node<Vec2Int>>(gridView.grid);
            Pathfinder = new AStarPathfinder<Node<Vec2Int>>(gridView.grid);
        }

        public IEnumerator Move(List<Node<Vec2Int>> path)
        {
            gridView.PaintPath(path, true);
            yield return new WaitForSeconds(sleepDuration);
            foreach (Node<Vec2Int> node in path)
            {
                Vector3 travelerPos = gridView.ToGridAligned(node.GetCoordinate());
                travelerPos.z = -1;
                transform.position = travelerPos;
                yield return new WaitForSeconds(sleepDuration);
            }

            gridView.PaintPath(path, false);
        }

        [ContextMenu("FindPath")]
        private void FindPath()
        {
            do
            {
                startNode = gridView.grid.nodes[Random.Range(0, gridView.grid.nodes.Count)];
            } while (startNode.IsBlocked());

            destinationNode = gridView.voronoi.GetClosestLandMark(startNode);
            Vector3 destPos = gridView.ToGridAligned(destinationNode.GetCoordinate());
            destPos.z = -1;
            destinationObject.transform.position = destPos;

            Vector3 travelerPos = gridView.ToGridAligned(startNode.GetCoordinate());
            travelerPos.z = -1;
            transform.position = travelerPos;

            List<Node<Vec2Int>> path = Pathfinder.FindPath(startNode, destinationNode);
            StartCoroutine(Move(path));
        }

        [ContextMenu("Repeat")]
        public void RepeatPath()
        {
            Vector3 travelerPos = gridView.ToGridAligned(startNode.GetCoordinate());
            travelerPos.z = -1;
            transform.position = travelerPos;

            List<Node<Vec2Int>> path = Pathfinder.FindPath(startNode, destinationNode);
            StartCoroutine(Move(path));
        }
    }
}