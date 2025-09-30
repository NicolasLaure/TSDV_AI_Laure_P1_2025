using AI_Model.Voronoi;
using CustomMath;
using RTS.Model;
using UnityEngine;

public class VoronoiView : MonoBehaviour
{
    [SerializeField] private GameObject planePrefab;

    public void Init(Voronoi<MapNode> voronoi)
    {
        foreach (VoronoiPoint<MapNode> point in voronoi.voronoiObjects)
        {
            foreach (Self_Plane plane in point.planes)
            {
                Vec3 planePos = plane.Normal * plane.Distance;
                Vector3 unityPos = new Vector3(planePos.x, planePos.y, planePos.z);
                GameObject planeObject = Instantiate(planePrefab, unityPos, Quaternion.identity);
                planeObject.transform.up = new Vector3(plane.Normal.x, plane.Normal.y, plane.Normal.z);
            }
        }
    }
}