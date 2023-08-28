using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UI;

public class InputFiledManager : MonoBehaviour
{
    private static InputFiledManager instance;

    public static InputFiledManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("InputFiledManager").AddComponent<InputFiledManager>();
            }
            return instance;
        }
    }

    public ColorInput colorInput;

    private List<IInputFiledHelper> InputFiledHelpers = new List<IInputFiledHelper>();
    public int Index;
    private bool IsSelect;

    //private void Awake()
    //{
    //    instance = this;
    //}

    private void Update()
    {
        if (IsSelect && Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNext();
            print(InputFiledHelpers.Count);
        }
    }

    private void SelectNext()
    {
        Index++;
        if (Index < 0)
            Index += InputFiledHelpers.Count;
        Index %= InputFiledHelpers.Count;
        //Index = Mathf.Abs(Index);
        var input = InputFiledHelpers[Index];
        input.Select();
    }

    public void Register(IInputFiledHelper IInputFiledHelper)
    {
        if (!InputFiledHelpers.Contains(IInputFiledHelper))
            InputFiledHelpers.Add(IInputFiledHelper);
        InputFiledHelpers.Sort((x, y) => x.Index() - y.Index());
    }

    public void Remove(IInputFiledHelper IInputFiledHelper)
    {
        if (InputFiledHelpers.Contains(IInputFiledHelper))
            InputFiledHelpers.Remove(IInputFiledHelper);
    }

    public void Select(IInputFiledHelper IInputFiledHelper)
    {
        if (!InputFiledHelpers.Contains(IInputFiledHelper))
            InputFiledHelpers.Add(IInputFiledHelper);
        IsSelect = true;
        Index = InputFiledHelpers.IndexOf(IInputFiledHelper);
    }
}

public interface IInputFiledHelper
{
    public int Index();

    public void Select();
}