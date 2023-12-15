using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;
using YuoTools.Extend.YuoMathf;

public class AiSearchPanel : MonoBehaviour
{
    public Button BtnInitMap;
    public Button BtnStartSearch;

    public Slider WidthSlider;
    public Slider HeightSlider;

    public YuoVector2Int MapSize;

    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        BtnInitMap.onClick.AddListener(InitMap);
        BtnStartSearch.onClick.AddListener(StartSearch);

        WidthSlider.onValueChanged.AddListener(v => MapSize.x = (int)v);
        HeightSlider.onValueChanged.AddListener(v => MapSize.y = (int)v);

        MapSize.x = (int)WidthSlider.value;
        MapSize.y = (int)HeightSlider.value;

        mapImages = new Image[0, 0];

        InitMap();
    }

    void Update()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {
            scrollRect.content.localScale += new Vector3(scrollWheel, scrollWheel, 0);
        }
    }

    private int[,] map;

    public YuoVector2Int Pointer1;
    public YuoVector2Int Pointer2;

    private Image[,] mapImages ;

    public void InitMap()
    {
        Pointer1 = new(1, 1);
        Pointer2 = new(MapSize.x - 2, MapSize.y - 2);
        map = MazeGenerator.GenerateMap(MapSize.x, MapSize.y, new(0, 0), new(MapSize.x - 1, MapSize.y - 1));

        for (int i = 0; i < mapImages.GetLength(0); i++)
        {
            for (int j = 0; j < mapImages.GetLength(1); j++)
            {
                Destroy(mapImages[i, j].gameObject);
            }
        }

        mapImages = new Image[map.GetLength(0), map.GetLength(1)];


        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                mapImages[i, j] = Instantiate(ImagePrefab, scrollRect.content);
                mapImages[i, j].Show();
                mapImages[i, j].rectTransform.anchoredPosition = new Vector2(i * 100, -j * 100);
                mapImages[i, j].color = map[i, j] == 1 ? Color.red : Color.white;
            }
        }

        mapImages[Pointer1.x, Pointer1.y].color = Color.blue;
        mapImages[Pointer2.x, Pointer2.y].color = Color.black;

        scrollRect.content.sizeDelta = new Vector2(map.GetLength(0) * 100, map.GetLength(1) * 100);

        YuoBStarSearch._Map = map;
    }

    public YuoBStarSearch YuoBStarSearch;
    public Image ImagePrefab;

    public void StartSearch()
    {
        if (map == null)
        {
            Debug.LogError("请先初始化地图");
            return;
        }

        if (Pointer1 == Pointer2) return;

        // YuoAStarSearch search = new YuoAStarSearch(map);
        // var path = search.Search(Pointer1, Pointer2);
        // YuoAStarSearch2 search2 = new YuoAStarSearch2(map);
        // var path = search2.GetAllPathPoints(Pointer1, Pointer2);

        // YuoBStarSearch searchB1 = new YuoBStarSearch(map);
        // var path = searchB1.Search(Pointer1, Pointer2);
        //
        // if (path == null || path.Count == 0)
        // {
        //     string mapString = "";
        //     for (int i = 0; i < map.GetLength(0); i++)
        //     {
        //         for (int j = 0; j < map.GetLength(1); j++)
        //         {
        //             mapString += map[i, j].ToString();
        //         }
        //         mapString += "\n";
        //     }
        //     Debug.Log(mapString);
        //     return;
        // }
        //
        // foreach (var grid in path)
        // {
        //     mapImages[grid.x, grid.y].color = Color.green;
        // }
    }
}