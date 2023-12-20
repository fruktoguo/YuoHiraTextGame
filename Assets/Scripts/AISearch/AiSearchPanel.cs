using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.Utilities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using YuoTools;
using YuoTools.Extend.Helper;
using YuoTools.Extend.MathFunction;

public class AiSearchPanel : MonoBehaviour
{
    public Button BtnInitMap;
    public Button BtnStartSearch;

    public Slider WidthSlider;
    public Slider HeightSlider;

    public YuoInt2 MapSize;

    private ScrollRect scrollRect;

    private void Start()
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
            var s = scrollRect.content.localScale + new Vector3(scrollWheel, scrollWheel, 0);
            if (s.x < 0.1f || s.y < 0.1f)
            {
                s = new Vector3(0.1f, 0.1f, scrollRect.content.localScale.z);
            }

            scrollRect.content.localScale = s;
        }
    }

    private int[,] map;

    public Vector2Int Pointer1;
    public Vector2Int Pointer2;

    private Image[,] mapImages;

    public YuoDrawTool DrawTool;

    public void InitMap()
    {
        // var texture = DrawTool.LayerManager.CurrentLayer.Texture;
        // map = new int[texture.width, texture.height];
        // for (int x = 0; x < texture.width; x++)
        // {
        //     for (int y = 0; y < texture.height; y++)
        //     {
        //         var pixel = texture.GetPixel(x, y);
        //         if (pixel.r < 0.5f)
        //         {
        //             map[x, y] = 1;
        //         }
        //         else
        //         {
        //             map[x, y] = 0;
        //         }
        //     }
        // }
        //
        // MapSize = new(texture.width, texture.height);
        // Pointer1 = new(1, 1);
        // Pointer2 = new(MapSize.x - 2, MapSize.y - 2);
        // return;

        map = MazeGenerator.GenerateMap(MapSize.x, MapSize.y, new(0, 0), new(MapSize.x - 1, MapSize.y - 1));

        for (int i = 0; i < mapImages.GetLength(0); i++)
        {
            for (int j = 0; j < mapImages.GetLength(1); j++)
            {
                Destroy(mapImages[i, j].gameObject);
            }
        }

        mapImages = new Image[map.GetLength(0), map.GetLength(1)];

        Pointer1 = new(1, 1);
        Pointer2 = new(map.GetLength(0) - 2, map.GetLength(1) - 2);

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
    }

    public Image ImagePrefab;
    private YuoBStarSearch searchB;

    public int Num = 100;
    public int JobNum = 100;

    public void StartSearch()
    {
        if (map == null)
        {
            Debug.LogError("请先初始化地图");
            return;
        }

        if (Pointer1 == Pointer2) return;

        var searchA = new YuoAStarSearch(map);
        StopwatchHelper.Start();
        for (int i = 0; i < Num; i++)
        {
            var pathA = searchA.Search(Pointer1, Pointer2);
        }

        var ms = StopwatchHelper.Stop();

        Debug.Log($"单线程总共耗时{ms}毫秒");
        // searchB = new YuoBStarSearch(map);
        // var pathB = searchB.Search(Pointer1, Pointer2);

        // Vector2Int[] 
        // var data = YuoAStarSearchJobs.GetInitData(map, new[] { Pointer1 }, new[] { Pointer2 });

        NativeList<JobHandle> jobs = new NativeList<JobHandle>(Allocator.TempJob);
        // NativeList<int2> result = new NativeList<int2>(Allocator.TempJob);
        List<IDisposable> disposables = new List<IDisposable>();

        Vector2Int[] startPos = new Vector2Int[JobNum];
        for (int i = 0; i < JobNum; i++)
        {
            startPos[i] = new Vector2Int(1, 1);
        }

        Vector2Int[] endPos = new Vector2Int[JobNum];
        for (int i = 0; i < JobNum; i++)
        {
            endPos[i] = new Vector2Int(map.GetLength(0) - 2, map.GetLength(1) - 2);
        }

        StopwatchHelper.Start();
        for (int i = 0; i < Num; i++)
        {
            var data = YuoAStarSearchJobs.InitJob(map, startPos, endPos);
            jobs.Add(data.job.Schedule());
            disposables.AddRange(data.disposables);
        }

        JobHandle.CompleteAll(jobs);
        ms = StopwatchHelper.Stop();
        Debug.Log($"Jobs总共耗时{ms}毫秒");
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
        // foreach (var grid in data.result)
        // {
        //     mapImages[grid.x, grid.y].color = Color.green;
        // }
        //
        // // data.disposables.DisposeAll();
        // data.nodeMap.Dispose();
        // data.openQueue.Dispose();
        // data.result.Dispose();
        // data.startPosArray.Dispose();
        // data.endPosArray.Dispose();
        jobs.Dispose();
        disposables.DisposableAll();
    }
}