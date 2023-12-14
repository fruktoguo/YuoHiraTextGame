using NF.Mathematics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public int Width = 9;
    public int Height = 5;

    void Start()
    {
        this.goBoard.Init(Width, Height);
        var walls = GetWalls(new string[]
        {
            "..X...X..",
            "......X..",
            ".XX...XX.",
            "..X......",
            "..X...X..",
        });

        this.goBoard.SetWalls(walls);
        this.goBoard.StartPos = new Int2(0, 4);
        this.goBoard.GoalPos = new Int2(7, 0);
    }

    bool[,] GetWalls(string[] strs)
    {
        var height = strs.Length;
        var width = strs[0].Length;
        var walls = new bool[height, width];
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                switch (strs[y][x])
                {
                    case 'X':
                        walls[y, x] = true;
                        break;
                }
            }
        }

        return walls;
    }

    public GoBoard goBoard;
}