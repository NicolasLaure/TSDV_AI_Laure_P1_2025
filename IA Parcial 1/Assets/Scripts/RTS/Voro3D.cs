using System;
using System.Collections.Generic;
using AI_Model.Voronoi;
using CustomMath;
using RTS.Model;
using Unity.Mathematics;
using UnityEngine;

public class Voro3D : MonoBehaviour
{
    [SerializeField] private GameView game;

    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private GameObject quaternionPointPrefab;

    private List<GameObject> mines = new List<GameObject>();
    private List<GameObject> planes = new List<GameObject>();

    private void Awake()
    {
        game.onMapBuild += Init;
    }

    private void OnDisable()
    {
        game.onMapBuild -= Init;
    }

    public void Init(Map map)
    {
        foreach (MapNode mineLocation in map.GetMineLocations())
        {
            float latitude = map.grid.GetLatitude(mineLocation);
            float longitude = map.grid.GetLongitude(mineLocation);
            Debug.Log($"Latitude: {latitude} Longitude: {longitude}");

            GameObject mine = Instantiate(minePrefab, transform);
            Quaternion mineRotation = Quaternion.Euler(latitude, longitude, 0f).normalized;
            mine.transform.rotation = mineRotation;
            mines.Add(mine);

            foreach (VoronoiPoint<MapNode> voroPoint in map.agentTypeToVoronoi[typeof(VillagerAgent)].voronoiObjects)
            {
                foreach (Self_Plane plane in voroPoint.planes)
                {
                    GameObject planeObject = Instantiate(planePrefab, transform);
                    planeObject.transform.up = new Vector3(plane.Normal.x, plane.Normal.y, plane.Normal.z);
                }
            }
        }
    }
}