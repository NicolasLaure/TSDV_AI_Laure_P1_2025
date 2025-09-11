using System.Collections;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI_View.Pathfinding
{
    public class TravelerView : MonoBehaviour
    {
        [SerializeField] private GameObject destinationObject;
        [SerializeField] private float sleepDuration = 0.1f;
        public GridView gridView;

        //private DepthFirstPathfinder<Node<Vector2Int>> _pathfinder;
        //private BreadthFirstPathfinder<Node<Vector2Int>> Pathfinder;
        //private DijstraPathfinder<Node<Vector2Int>> Pathfinder;
        private AStarPathfinder<Node<Vector2Int>> Pathfinder;

        private Node<Vector2Int> startNode;
        private Node<Vector2Int> destinationNode;

        private void Start()
        {
            //_pathfinder = new DepthFirstPathfinder<Node<Vector2Int>>(gridView.grid);
            Pathfinder = new AStarPathfinder<Node<Vector2Int>>(gridView.grid);
        }

        public IEnumerator Move(List<Node<Vector2Int>> path)
        {
            gridView.PaintPath(path, true);
            yield return new WaitForSeconds(sleepDuration);
            foreach (Node<Vector2Int> node in path)
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

            List<Node<Vector2Int>> path = Pathfinder.FindPath(startNode, destinationNode);
            StartCoroutine(Move(path));
        }

        [ContextMenu("Repeat")]
        public void RepeatPath()
        {
            Vector3 travelerPos = gridView.ToGridAligned(startNode.GetCoordinate());
            travelerPos.z = -1;
            transform.position = travelerPos;

            List<Node<Vector2Int>> path = Pathfinder.FindPath(startNode, destinationNode);
            StartCoroutine(Move(path));
        }
    }
}