using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

            Hide();
        }
    }

    public DropItem AddItem(string key)
    {
        return AddItem(key, key);
    }

    public DropItem AddItem(string key, string text)
    {
        if (dropKeys.ContainsKey(key)) return dropKeys[key];

        DropItem dropItem = Instantiate(dropPre, dropPre.transform.parent).AddComponent<DropItem>();
        dropItem.go = dropItem.gameObject;
        dropItem.name = key;
        dropItem.go.SetActive(true);
        dropItem.button = dropItem.go.GetComponent<Button>();
        dropItem.text = dropItem.go.transform.Find("Label").GetComponent<Text>();
        dropItem.Arrow = dropItem.go.transform.Find("Arrow").gameObject;
        dropItem.button.onClick.RemoveAllListeners();
        dropItem.button.onClick.AddListener(Hide);
        dropItem.button.onClick.AddListener(() => SetItem(dropItem));
        dropItem.text.text = text;
        dropItem.Index = dropItems.Count;
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

    public void Hide()
    {
        DropList.gameObject.SetActive(false);
        Arrow.transform.localScale = _defScale;
        transform.SetSiblingIndex(defSortOrder);
    }

    public void Show()
    {
        DropList.gameObject.SetActive(true);

        Arrow.transform.localScale = _defScale * -1;
        if (nowItem != null)
            DropList.verticalNormalizedPosition = 1f - nowItem.Index / (dropItems.Count - 1f);
        defSortOrder = transform.GetSiblingIndex();
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    [SerializeField] private DropItem _nowItem;

    public DropItem nowItem
    {
        get => _nowItem;
        set
        {
            OnValueChanged?.Invoke(value);
            _nowItem = value;
            LableText.text = value.name;
        }
    }

    public UnityAction<DropItem> OnValueChanged;

    public void SetItem(DropItem item)
    {
        nowItem = item;
        foreach (var _item in dropItems)
        {
            if (_item.Arrow.activeSelf)
            {
                _item.Arrow.SetActive(false);
            }
        }

        item.Arrow.SetActive(true);
    }

    public void SetItem(string item)
    {
        if (dropKeys.ContainsKey(item))
        {
            foreach (var _item in dropItems)
            {
                if (_item.Arrow.activeSelf)
                {
                    _item.Arrow.SetActive(false);
                }
            }

            nowItem = dropKeys[item];
            dropKeys[item].Arrow.SetActive(true);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (DropList.gameObject.activeSelf)
        {
            if (eventData.selectedObject == gameObject)
            {
                Hide();
            }
        }
        else
        {
            Show();
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
        public int Index;
        public GameObject go;
        public Button button;
        public Text text;
        public GameObject Arrow;
    }
}