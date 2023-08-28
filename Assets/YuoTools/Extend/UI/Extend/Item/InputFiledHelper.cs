using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFiledHelper : MonoBehaviour, IPointerClickHandler, IInputFiledHelper
{
    public int Group;

    [HideInInspector]
    public InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<InputField>();
    }

    private InputFiledManager inputFiledManager;

    private void OnEnable()
    {
        inputFiledManager = InputFiledManager.Instance;
        inputFiledManager.Register(this);
    }

    private void OnDisable()
    {
        inputFiledManager?.Remove(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inputFiledManager?.Select(this);
        inputField.Select();
    }

    int IInputFiledHelper.Index()
    {
        return Group * 10 + transform.GetSiblingIndex();
    }

    public void Select()
    {
        inputField.Select();
    }
}