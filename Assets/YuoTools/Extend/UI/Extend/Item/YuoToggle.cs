using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class YuoToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
{
    public UnityEvent<bool> onValueChanged = new UnityEvent<bool>();

    [OnValueChanged("OnChanged")] [SerializeField]
    bool _isOn;

    public bool isOn
    {
        get => _isOn;
        set
        {
            if (value != _isOn)
            {
                if (group != null)
                {
                    if (_isOn != group.Check(this, value))
                    {
                        _isOn = value;
                        OnChanged();
                    }
                }
                else
                {
                    _isOn = value;
                    onValueChanged.Invoke(value);
                }
            }
        }
    }

    void OnChanged()
    {
        onValueChanged.Invoke(_isOn);
        if (_isOn && group != null)
        {
            group.SelectToggle(this);
        }
    }

    [OnValueChanged("OnGroupChanged")] [SerializeField]
    private YuoToggleGroup group;

    /// <summary>
    /// Group the toggle belongs to.
    /// </summary>
    public YuoToggleGroup Group
    {
        get { return group; }
        set
        {
            if (value != group)
            {
                if (group != null)
                    group.UnregisterToggle(this);
                group = value;
                OnGroupChanged();
            }
        }
    }

    void OnGroupChanged()
    {
        Debug.Log("OnGroupChanged");
        if (group != null)
        {
            group.RegisterToggle(this);
        }
    }

    public void Rebuild(CanvasUpdate executing)
    {
        throw new System.NotImplementedException();
    }

    public void LayoutComplete()
    {
        throw new System.NotImplementedException();
    }

    public void GraphicUpdateComplete()
    {
        throw new System.NotImplementedException();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }

#endif

    protected override void Awake()
    {
        base.Awake();
        if (Group != null)
        {
            Group.RegisterToggle(this);
        }
    }

    protected override void Start()
    {
        PlayEffect(true);
    }

    /// <summary>
    /// Transition mode for the toggle.
    /// </summary>
    public Toggle.ToggleTransition toggleTransition = Toggle.ToggleTransition.Fade;

    /// <summary>
    /// Graphic the toggle should be working with.
    /// </summary>
    public Graphic graphic;

    private void PlayEffect(bool instant)
    {
        if (graphic == null)
            return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            graphic.canvasRenderer.SetAlpha(_isOn ? 1f : 0f);
        else
#endif
            graphic.CrossFadeAlpha(_isOn ? 1f : 0f, instant ? 0f : 0.1f, true);
    }

    private void InternalToggle()
    {
        if (!IsActive() || !IsInteractable())
            return;
        isOn = !isOn;
    }

    /// <summary>
    /// React to clicks.
    /// </summary>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        InternalToggle();
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        InternalToggle();
    }
}