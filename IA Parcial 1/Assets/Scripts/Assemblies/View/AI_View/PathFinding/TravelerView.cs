using System.Collections;
using System.Collections.Generic;
using RTS.Model;
using UnityEngine;

namespace AI_View.Pathfinding
{
    public class TravelerView : MonoBehaviour
    {
        [SerializeField] private GameObject destinationObject;
        [SerializeField] private float sleepDuration = 0.1f;
        public GridView gridView;

        public Map map;

        //private DepthFirstPathfinder<Node<Vec2Int>> _pathfinder;
        //private BreadthFirstPathfinder<Node<Vec2Int>> Pathfinder;
        //private DijstraPathfinder<Node<Vec2Int>> Pathfinder;

        public IEnumerator Move(List<MapNode> path)
        {
            gridView.PaintPath(path, true);
            yield return new WaitForSeconds(sleepDuration);
            foreach (MapNode node in path)
            {
                Vector3 travelerPos = gridView.ToGridAligned(node.GetCoordinate());
                travelerPos.z = -1;
                transform.position = travelerPos;
                yield return new WaitForSeconds(sleepDuration);
            }

            gridView.PaintPath(path, false);
        }

        // [ContextMenu("FindPath")]
        // private void FindPath()
        // {
        //     do
        //     {
        //         startNode = gridView.grid.nodes[Random.Range(0, gridView.grid.nodes.Count)];
        //     } while (startNode.IsBlocked());
        //
        //     destinationNode = map.voronoi.GetClosestLandMark(startNode);
        //     Vector3 destPos = gridView.ToGridAligned(destinationNode.GetCoordinate());
        //     destPos.z = -1;
        //     destinationObject.transform.position = destPos;
        //
        //     Vector3 travelerPos = gridView.ToGridAligned(startNode.GetCoordinate());
        //     travelerPos.z = -1;
        //     transform.position = travelerPos;
        //
        //     List<MapNode> path = Pathfinder.FindPath(startNode, destinationNode);
        //     StartCoroutine(Move(path));
        // }
        //
        // [ContextMenu("Repeat")]
        // public void RepeatPath()
        // {
        //     Vector3 travelerPos = gridView.ToGridAligned(startNode.GetCoordinate());
        //     travelerPos.z = -1;
        //     transform.position = travelerPos;
        //
        //     List<MapNode> path = Pathfinder.FindPath(startNode, destinationNode);
        //     StartCoroutine(Move(path));
        // }
    }
}