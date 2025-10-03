using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AI_View.Pathfinding;
using RTS.Model;
using RTS.View;
using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private RTS_MapView gridView;
    [SerializeField] private VoronoiView voronoiView;

    [Header("Input Fields")] [SerializeField]
    private TMP_InputField width;

    [SerializeField] private TMP_InputField height;
    [SerializeField] private TMP_InputField minesQty;
    [SerializeField] private Vector3 cubeSize;
    [SerializeField] private float gridSpacing;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject hud;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private GameObject villagerPrefab;
    [SerializeField] private GameObject convoyPrefab;
    [SerializeField] public Mesh villagerMesh;
    [SerializeField] public Mesh convoyMesh;
    [SerializeField] public Material villagerMaterial;
    [SerializeField] public Material convoyMaterial;
    private const int MAX_OBJS_PER_DRAWCALL = 1000;

    [SerializeField] private float tickDelay;

    private Dictionary<WorkerAgent, TravelerView> agentToTravelerView = new Dictionary<WorkerAgent, TravelerView>();
    private Game game;
    private Coroutine gameCoroutine;

    public Action<Map> onMapBuild;


    private List<TravelerView> villagerViews = new List<TravelerView>();
    private List<TravelerView> convoyViews = new List<TravelerView>();

    public void InitGame()
    {
        game = new Game(int.Parse(width.text), int.Parse(height.text), cubeSize.x, gridSpacing,
            int.Parse(minesQty.text));
        gridView.Init(game.map, typeof(VillagerAgent), VillagerAgent.typeToCost, cubeSize, gridSpacing);
        voronoiView.Init(game.map);
        panel.SetActive(false);
        hud.SetActive(true);
        onMapBuild?.Invoke(game.map);

        foreach (VillagerAgent agent in game.villagers)
        {
            GameObject villager = Instantiate(villagerPrefab,
                gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            villager.GetComponent<VillagerView>().villagerAgent = agent;
            voronoiView.AddMiner(agent);
            agentToTravelerView.Add(agent, villager.GetComponent<TravelerView>());
            villagerViews.Add(villager.GetComponent<TravelerView>());
        }

        foreach (Convoy agent in game.convoys)
        {
            GameObject convoy = Instantiate(convoyPrefab,
                gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            agentToTravelerView.Add(agent, convoy.GetComponent<TravelerView>());
            convoyViews.Add(convoy.GetComponent<TravelerView>());
        }

        if (gameCoroutine != null)
            StopCoroutine(gameCoroutine);

        gameCoroutine = StartCoroutine(GameUpdate());
    }

    private void Update()
    {
        if (game == null)
            return;

        Draw();
    }

    private IEnumerator GameUpdate()
    {
        while (game != null)
        {
            game.Tick(Time.deltaTime);
            voronoiView.UpdateView(game.villagers);
            goldText.text = $"Gold: {game.map.headquarters.heldResources}";

            foreach (WorkerAgent agent in agentToTravelerView.Keys)
            {
                if ((agent as VillagerAgent) != null)
                {
                    Debug.Log(
                        $"Worker Food: {((VillagerAgent)agent).CurrentFood}, heldResources: {agent.inventory.heldResources}");
                    Debug.Log(
                        $"AStar NodesQty:{agent.CurrentPath.Count}, ThetaStar Nodes Qty: {agent.CurrentThetaPath.Count}");
                }

                agentToTravelerView[agent].SetPosition(new Vector3(agent.transform.Position.x, agent.transform.Position.y, 0));
            }

            foreach (Mine mine in game.map.GetMines())
            {
                Debug.Log($"Mine has: {mine.workingVillagers} workers");
            }

            yield return new WaitForSeconds(tickDelay);
        }
    }

    private void Draw()
    {
        gridView.Draw();

        Draw(villagerViews, villagerMesh, villagerMaterial);
        Draw(convoyViews, convoyMesh, convoyMaterial);
    }

    private void Draw(List<TravelerView> agents, Mesh agentMesh, Material agentMaterial)
    {
        List<Matrix4x4[]> drawMatrix = new List<Matrix4x4[]>();
        int meshes = agents.Count;
        for (int i = 0; i < agents.Count; i += MAX_OBJS_PER_DRAWCALL)
        {
            drawMatrix.Add(new Matrix4x4[meshes > MAX_OBJS_PER_DRAWCALL ? MAX_OBJS_PER_DRAWCALL : meshes]);
            meshes -= MAX_OBJS_PER_DRAWCALL;
        }

        Parallel.For(0, agents.Count, i =>
        {
            drawMatrix[(i / MAX_OBJS_PER_DRAWCALL)][(i % MAX_OBJS_PER_DRAWCALL)]
                .SetTRS(agents[i].position, Quaternion.identity, agents[i].scale);
        });
        for (int i = 0; i < drawMatrix.Count; i++)
        {
            Graphics.DrawMeshInstanced(agentMesh, 0, agentMaterial, drawMatrix[i]);
        }
    }

    public void AddVillager()
    {
        if (game.TryBuyVillager(out VillagerAgent agent))
        {
            GameObject villager = Instantiate(villagerPrefab,
                gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            villager.GetComponent<VillagerView>().villagerAgent = agent;
            agentToTravelerView.Add(agent, villager.GetComponent<TravelerView>());
            villagerViews.Add(villager.GetComponent<TravelerView>());
        }
    }

    public void AddConvoy()
    {
        if (game.TryBuyConvoy(out Convoy agent))
        {
            GameObject convoy = Instantiate(convoyPrefab,
                gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            agentToTravelerView.Add(agent, convoy.GetComponent<TravelerView>());
            convoyViews.Add(convoy.GetComponent<TravelerView>());
        }
    }

    public void Alert()
    {
        game.Alert();
    }
}