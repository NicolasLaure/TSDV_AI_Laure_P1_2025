using System.Collections.Generic;
using AI_View.Pathfinding;
using RTS.Model;
using RTS.View;
using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private RTS_MapView gridView;

    [Header("Input Fields")] [SerializeField]
    private TMP_InputField width;

    [SerializeField] private TMP_InputField height;
    [SerializeField] private TMP_InputField minesQty;
    [SerializeField] private GameObject panel;

    [SerializeField] private GameObject villagerPrefab;
    [SerializeField] private GameObject convoyPrefab;

    private Dictionary<TravelerAgent, TravelerView> agentToTravelerView = new Dictionary<TravelerAgent, TravelerView>();
    private Game game;

    public void InitGame()
    {
        game = new Game(int.Parse(width.text), int.Parse(height.text), int.Parse(minesQty.text));
        gridView.Init(game.map);
        panel.SetActive(false);
        
        foreach (TravelerAgent agent in game.villagers)
        {
            GameObject villager = Instantiate(villagerPrefab,
                gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            agentToTravelerView.Add(agent, villager.GetComponent<TravelerView>());
        }

        foreach (TravelerAgent agent in game.convoys)
        {
            GameObject convoy = Instantiate(convoyPrefab,
                gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()), Quaternion.identity);
            agentToTravelerView.Add(agent, convoy.GetComponent<TravelerView>());
        }
    }

    public void Update()
    {
        if (game == null)
            return;

        game.Tick(Time.deltaTime);
        foreach (TravelerAgent agent in agentToTravelerView.Keys)
        {
            Debug.Log($"Villager Closest Mine:{agent.closestMineNode}");
            agentToTravelerView[agent].SetPosition(gridView.ToEntityGridAligned(agent.agentPosition.GetCoordinate()));
        }
    }
}