using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject hud;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private GameObject villagerPrefab;
    [SerializeField] private GameObject convoyPrefab;
    [SerializeField] private float tickDelay;

    private Dictionary<WorkerAgent, TravelerView> agentToTravelerView = new Dictionary<WorkerAgent, TravelerView>();
    private Game game;
    private Coroutine gameCoroutine;

    public Action<Map> onMapBuild;

    public void InitGame()
    {
        game = new Game(int.Parse(width.text), int.Parse(height.text), int.Parse(minesQty.text));
        gridView.Init(game.map, typeof(VillagerAgent), VillagerAgent.typeToCost);
        voronoiView.Init(game.map);
        panel.SetActive(false);
        hud.SetActive(true);
        onMapBuild?.Invoke(game.map);

        foreach (VillagerAgent agent in game.villagers)
        {
            GameObject villager = Instantiate(villagerPrefab, gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            villager.GetComponent<VillagerView>().villagerAgent = agent;
            voronoiView.AddMiner(agent);
            agentToTravelerView.Add(agent, villager.GetComponent<TravelerView>());
        }

        foreach (Convoy agent in game.convoys)
        {
            GameObject convoy = Instantiate(convoyPrefab, gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            agentToTravelerView.Add(agent, convoy.GetComponent<TravelerView>());
        }

        if (gameCoroutine != null)
            StopCoroutine(gameCoroutine);

        gameCoroutine = StartCoroutine(GameUpdate());
    }

    private void Update()
    {
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
                    Debug.Log($"Worker Food: {((VillagerAgent)agent).CurrentFood}, heldResources: {agent.inventory.heldResources}");

                agentToTravelerView[agent].SetPosition(gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()));
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
    }
    public void AddVillager()
    {
        if (game.TryBuyVillager(out VillagerAgent agent))
        {
            GameObject villager = Instantiate(villagerPrefab, gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            villager.GetComponent<VillagerView>().villagerAgent = agent;
            agentToTravelerView.Add(agent, villager.GetComponent<TravelerView>());
        }
    }

    public void AddConvoy()
    {
        if (game.TryBuyConvoy(out Convoy agent))
        {
            GameObject convoy = Instantiate(convoyPrefab, gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            agentToTravelerView.Add(agent, convoy.GetComponent<TravelerView>());
        }
    }

    public void Alert()
    {
        game.Alert();
    }
}