using AI_View.Pathfinding;
using RTS.Model;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private GridView gridView;

    [Header("Input Fields")] [SerializeField]
    private InputField width;

    [SerializeField] private InputField height;
    [SerializeField] private InputField minesQty;

    private Game game;

    public void InitGame()
    {
        game = new Game(int.Parse(width.text), int.Parse(height.text), int.Parse(minesQty.text));
        gridView.Init(game.map.grid);
    }
}