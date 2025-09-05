using System.Collections;
using System.Collections.Generic;
using AIP1_Laure.AI.Pathfinding;
using Pathfinder;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AIP1_Laure.View.Pathfinding
{
    public class TravelerView : MonoBehaviour
    {
        [SerializeField] private GameObject destinationObject;

        public GridView gridView;

        private DepthFirstPathfinder<Node<Vector2Int>> _pathfinder;
        //private BreadthFirstPathfinder<Node<Vector2Int>> Pathfinder;
        //private DijstraPathfinder<Node<Vector2Int>> Pathfinder;
        //private AStarPathfinder<Node<Vector2Int>> Pathfinder;

        private Node<Vector2Int> startNode;
        private Node<Vector2Int> destinationNode;

        private void Start()
        {
            _pathfinder = new DepthFirstPathfinder<Node<Vector2Int>>(gridView.grid);
        }

        public IEnumerator Move(List<Node<Vector2Int>> path)
        {
            foreach (Node<Vector2Int> node in path)
            {
                transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
                yield return new WaitForSeconds(1.0f);
            }
        }

        [ContextMenu("FindPath")]
        private void FindPath()
        {
            do
            {
                startNode = gridView.grid.nodes[Random.Range(0, gridView.grid.nodes.Count)];
            } while (startNode.IsBlocked());

            do
            {
                destinationNode = gridView.grid.nodes[Random.Range(0, gridView.grid.nodes.Count)];
            } while (destinationNode.IsBlocked() || destinationNode == startNode);

            Vector3 destPos = gridView.ToGridAligned(destinationNode.GetCoordinate());
            destPos.z = -1;
            destinationObject.transform.position = destPos;

            Vector3 travelerPos = gridView.ToGridAligned(startNode.GetCoordinate());
            travelerPos.z = -1;
            transform.position = travelerPos;

            List<Node<Vector2Int>> path = _pathfinder.FindPath(startNode, destinationNode);
            StartCoroutine(Move(path));
        }
    }
}