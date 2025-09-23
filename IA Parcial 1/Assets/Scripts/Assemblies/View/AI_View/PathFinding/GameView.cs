using AI_View.Pathfinding;
using RTS.Model;
using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private GridView gridView;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField width;
    [SerializeField] private TMP_InputField height;
    [SerializeField] private TMP_InputField minesQty;
    [SerializeField] private GameObject panel;

    private Game game;

    public void InitGame()
    {
        game = new Game(int.Parse(width.text), int.Parse(height.text), int.Parse(minesQty.text));
        gridView.Init(game.map.grid);
        panel.SetActive(false);
    }
}