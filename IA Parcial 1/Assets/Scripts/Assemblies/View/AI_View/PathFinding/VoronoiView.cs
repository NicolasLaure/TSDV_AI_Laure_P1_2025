using System;
using System.Collections.Generic;
using AI_Model.Voronoi;
using CustomMath;
using RTS.Model;
using UnityEngine;

public class VoronoiView : MonoBehaviour
{
    [SerializeField] private GameView game;

    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private GameObject minerPrefab;
    [SerializeField] private GameObject halfRotationPoint;

    private List<GameObject> mines = new List<GameObject>();
    private Dictionary<VillagerAgent, GameObject> miners = new Dictionary<VillagerAgent, GameObject>();
    private List<GameObject> voroPlanes = new List<GameObject>();

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
        }

        foreach (VoronoiPoint<MapNode> voroPoint in map.agentTypeToVoronoi[typeof(VillagerAgent)].voronoiObjects)
        {
            foreach (Self_Plane plane in voroPoint.planes)
            {
                GameObject planeObject = Instantiate(planePrefab, transform);
                planeObject.transform.up = new Vector3(plane.Normal.x, plane.Normal.y, plane.Normal.z);
                voroPlanes.Add(planeObject);
            }
        }

        Debug.Log($"PLANES QTY: {voroPlanes.Count}");
    }

    public void UpdateView(List<VillagerAgent> villagers)
    {
        foreach (VillagerAgent miner in miners.Keys)
        {
            miners[miner].transform.rotation = Quaternion.Euler(miner.agentPosition.GetLatitude(), miner.agentPosition.GetLongitude(), 0.0f);
        }
    }

    public void AddMiner(VillagerAgent minerAgent)
    {
        GameObject miner = Instantiate(minerPrefab, transform);
        miner.transform.rotation = Quaternion.Euler(minerAgent.agentPosition.GetLatitude(), minerAgent.agentPosition.GetLongitude(), 0.0f);
        miners.Add(minerAgent, miner);
    }
}