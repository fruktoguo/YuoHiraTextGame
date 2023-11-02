using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YuoTools;

public class YuoDropDown : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, ICancelHandler
{
    public Text LableText;
    public Image Arrow;

    [SerializeField] private GameObject dropPre;

    public ScrollRect DropList;

    [SerializeField] private List<DropItem> dropItems = new List<DropItem>();

    private Dictionary<string, DropItem> dropKeys = new Dictionary<string, DropItem>();

    public int DropCount
    {
        get => dropItems.Count;
    }

    [HorizontalGroup("背景")] [SerializeField] [LabelWidth(25)]
    private Image _背景;

    [HorizontalGroup("背景")] [SerializeField] [LabelWidth(50)]
    private Sprite _背景图片;

    int defSortOrder;

    private void Awake()
    {
        _defScale = Arrow.transform.localScale;
    }

    private void Update()
    {
        if (!DropList.gameObject.activeSelf) return;
        if (Input.GetMouseButtonUp(0))
        {
            var go = new PointerEventData(EventSystem.current).selectedObject;
            if (go != null)
            {
                if (go.GetComponentInParent<YuoDropDown>() == this) return;
            }

            HideDropDown();
        }
    }

    public DropItem AddItem(string key)
    {
        return AddItem(key, key);
    }

    public DropItem AddItem(string key, string text)
    {
        if (dropKeys.TryGetValue(key, out var item)) return item;

        DropItem dropItem = Instantiate(dropPre, dropPre.transform.parent).AddComponent<DropItem>();
        dropItem.go = dropItem.gameObject;
        dropItem.name = key;
        dropItem.go.SetActive(true);
        dropItem.button = dropItem.go.GetComponent<Button>();
        dropItem.text = dropItem.go.transform.Find("Label").GetComponent<Text>();
        dropItem.arrow = dropItem.go.transform.Find("Arrow").gameObject;
        dropItem.button.onClick.RemoveAllListeners();
        dropItem.button.onClick.AddListener(HideDropDown);
        dropItem.button.onClick.AddListener(() => SetItem(dropItem));
        dropItem.text.text = text;
        dropItem.index = dropItems.Count;
        dropItems.Add(dropItem);
        dropKeys.Add(key, dropItem);
        return dropItem;
    }

    public bool ContainsItem(string key)
    {
        return dropKeys.ContainsKey(key);
    }

    private Vector3 _defScale;

    public void Clear()
    {
        foreach (var item in dropItems)
        {
            Destroy(item.gameObject);
        }

        dropItems.Clear();
        dropKeys.Clear();
    }

    public void HideDropDown()
    {
        DropList.gameObject.SetActive(false);
        Arrow.transform.localScale = _defScale;
        transform.SetSiblingIndex(defSortOrder);
    }

    public void ShowDropDown()
    {
        DropList.gameObject.SetActive(true);

        Arrow.transform.localScale = _defScale * -1;
        if (NowItem != null)
            DropList.verticalNormalizedPosition = 1f - NowItem.index / (dropItems.Count - 1f);
        defSortOrder = transform.GetSiblingIndex();
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    [SerializeField] private DropItem nowItem;

    public DropItem NowItem
    {
        get => nowItem;
        set
        {
            OnValueChanged?.Invoke(value);
            nowItem = value;
            LableText.text = value.name;
        }
    }

    public UnityAction<DropItem> OnValueChanged;

    public void SetItem(DropItem itemName)
    {
        NowItem = itemName;
        foreach (var item in dropItems)
        {
            if (item.arrow.activeSelf)
            {
                item.arrow.SetActive(false);
            }
        }

        itemName.arrow.SetActive(true);
    }

    public void SetItem(string key)
    {
        if (dropKeys.ContainsKey(key))
        {
            foreach (var item in dropItems)
            {
                if (item.arrow.activeSelf)
                {
                    item.arrow.SetActive(false);
                }
            }

            NowItem = dropKeys[key];
            dropKeys[key].arrow.SetActive(true);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (DropList.gameObject.activeSelf)
        {
            if (eventData.selectedObject == gameObject)
            {
                HideDropDown();
            }
        }
        else
        {
            ShowDropDown();
        }
        //EventSystem.current.SetSelectedGameObject(base.gameObject);
    }

    public virtual void OnCancel(BaseEventData eventData)
    {
        //print(eventData.selectedObject.name);
    }

    [System.Serializable]
    public class DropItem : MonoBehaviour
    {
        public new string name;
        public int index;
        public GameObject go;
        public Button button;
        public Text text;
        public GameObject arrow;
        public object Action;
    }
}