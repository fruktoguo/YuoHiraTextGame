using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YuoTools.Extend;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.Editor.Ecs
{
    public class YuoWorldWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/World &w")]
        private static async void OpenWindow()
        {
            var window = GetWindow<YuoWorldWindow>();
            window.Show();
            await Task.Delay(100);
            window.ForceMenuTreeRebuild();
        }

        float _time;
        string _searchEntity = "";

        string SearchEntity
        {
            get => _searchEntity;
            set
            {
                if (value != _searchEntity)
                {
                    _searchEntity = value;
                    ForceMenuTreeRebuild();
                }
            }
        }

        string _searchSystem = "";

        string SearchSystem
        {
            get => _searchSystem;
            set
            {
                if (value != _searchSystem)
                {
                    _searchSystem = value;
                    ForceMenuTreeRebuild();
                }
            }
        }


        protected override void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_isEntities, "Entities", EditorStyles.toolbarButton) != _isEntities)
            {
                _isEntities = !_isEntities;
                _isSystems = !_isEntities;
                ForceMenuTreeRebuild();
            }

            if (GUILayout.Toggle(_isSystems, "Systems", EditorStyles.toolbarButton) != _isSystems)
            {
                _isSystems = !_isSystems;
                _isEntities = !_isSystems;
                ForceMenuTreeRebuild();
            }

            if (GUILayout.Button("刷新", GUILayout.Width(50)))
            {
                ForceMenuTreeRebuild();
            }

            GUILayout.EndHorizontal();
            if (_isEntities)
            {
                SearchEntity = GUILayout.TextField(SearchEntity, (GUIStyle)"SearchTextField");
            }
            else
            {
                SearchSystem = GUILayout.TextField(SearchSystem, (GUIStyle)"SearchTextField");
                GUILayout.BeginHorizontal();
                GUILayout.Label("排序方式");
                RankDirection = GUILayout.Toggle(RankDirection, "↑↓", EditorStyles.toolbarButton,
                    new[] { GUILayout.Width(30) });
                SystemRank =
                    (ESystemRank)GUILayout.Toolbar((int)SystemRank, new[] { "无", "单次耗时", "总耗时", "平均耗时", "执行次数" });
                GUILayout.EndHorizontal();
            }

            foreach (var systemView in _systemViews)
            {
                systemView.Update();
            }

            foreach (var entityView in _componentViews)
            {
                entityView.Update();
            }

            base.OnGUI();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public void ForceRebuild()
        {
            ForceMenuTreeRebuild();
        }

        private void Update()
        {
            if (!Application.isPlaying) return;
            if (_isSystems) return;
            if (YuoWorld.Instance == null) return;
            if (_lastCount != YuoWorld.Instance.Entities.Count)
            {
                ForceMenuTreeRebuild();
                _lastCount = YuoWorld.Instance.Entities.Count;
            }
        }

        int _lastCount = 0;

        bool _isEntities = true;
        bool _isSystems = false;


        readonly List<ComponentView> _componentViews = new List<ComponentView>();
        readonly List<SystemView> _systemViews = new List<SystemView>();

        private ESystemRank systemRank = ESystemRank.无;

        private ESystemRank SystemRank
        {
            get => systemRank;
            set
            {
                if (value != systemRank)
                {
                    systemRank = value;
                    ForceMenuTreeRebuild();
                }
            }
        }

        private bool _rankDirection = false;

        private bool RankDirection
        {
            get => _rankDirection;
            set
            {
                if (value != _rankDirection)
                {
                    _rankDirection = value;
                    ForceMenuTreeRebuild();
                }
            }
        }

        public enum ESystemRank
        {
            无 = 0,
            单次耗时 = 1,
            总耗时 = 2,
            平均耗时 = 3,
            执行次数 = 4,
        }


        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                DefaultMenuStyle =
                {
                    AlignTriangleLeft = false,
                }
            };

            if (!Application.isPlaying)
            {
                return tree;
            }

            if (YuoWorld.Instance == null) return tree;

            _componentViews.Clear();
            _systemViews.Clear();

            #region Entites

            if (_isEntities)
            {
                AddEntity(YuoWorld.Main, YuoWorld.Main.EntityName);
                foreach (var scene in YuoWorld.Instance.AllScenes)
                {
                    AddEntity(scene, scene.EntityName);
                }
            }

            #endregion

            #region Systems

            if (_isSystems)
            {
                foreach (var system in YuoWorld.Instance.GetAllSystem)
                {
                    AddSystem(system);
                }

                switch (SystemRank)
                {
                    case ESystemRank.单次耗时:
                        _systemViews.Sort((a, b) =>
                            RankDirection
                                ? a.TimeConsuming.CompareTo(b.TimeConsuming)
                                : b.TimeConsuming.CompareTo(a.TimeConsuming));
                        break;
                    case ESystemRank.总耗时:
                        _systemViews.Sort((a, b) =>
                            RankDirection
                                ? a.System.TotalTimeConsuming.CompareTo(b.System.TotalTimeConsuming)
                                : b.System.TotalTimeConsuming.CompareTo(a.System.TotalTimeConsuming));
                        break;
                    case ESystemRank.平均耗时:
                        _systemViews.Sort((a, b) =>
                            RankDirection
                                ? a.AverageTimeConsuming.CompareTo(b.AverageTimeConsuming)
                                : b.AverageTimeConsuming.CompareTo(a.AverageTimeConsuming));
                        break;
                    case ESystemRank.执行次数:
                        _systemViews.Sort((a, b) =>
                            RankDirection
                                ? a.System.TotalRunCount.CompareTo(b.System.TotalRunCount)
                                : b.System.TotalRunCount.CompareTo(a.System.TotalRunCount));
                        break;
                }

                if (SystemRank == ESystemRank.无)
                {
                    foreach (var systemView in _systemViews)
                    {
                        tree.Add(
                            (systemView.System.Group == "" ? "" : $"{systemView.System.Group}/") +
                            systemView.System.Name,
                            systemView);
                    }
                }
                else
                {
                    foreach (var systemView in _systemViews)
                    {
                        tree.Add(systemView.System.Name, systemView);
                    }
                }
            }

            #endregion

            return tree;

            void AddSystem(SystemBase system)
            {
                if (!string.IsNullOrEmpty(SearchSystem) &&
                    //检索名称,不区分大小写
                    !system.GetType().Name.ToLower().Contains(SearchSystem.ToLower()))
                {
                    return;
                }

                SystemView view = new SystemView();
                view.SystemOrder = system.GetType().GetCustomAttribute<SystemOrderAttribute>()?.Order ?? 0;
                view.TimeConsuming = system.TimeConsuming;
                view.System = system;
                view.InfluenceType.AddRange(system.InfluenceTypes().Select(type => type.Name));
                view.InterfaceType.AddRange(system.Type.GetInterfaces().Select(type => type.Name));
                view.InterfaceType.Remove("ISystemTag");
                _systemViews.Add(view);
            }

            void AddEntity(YuoEntity entity, string path)
            {
                if (string.IsNullOrEmpty(SearchEntity) ||
                    //检索名称,不区分大小写
                    entity.EntityName.ToLower().Contains(SearchEntity.ToLower()) ||
                    //检索ID
                    entity.ID.ToString().Contains(SearchEntity))
                {
                    ComponentView view = new()
                    {
                        Entity = entity
                    };
                    view.EntityID = entity.ID;
                    foreach (var component in entity.Components.Values)
                    {
                        if (!view.Components.Contains(component))
                        {
                            view.Components.Add(component);
                        }
                    }

                    _componentViews.Add(view);

                    tree.Add(path, view);

                    foreach (var child in entity.Children)
                    {
                        AddEntity(child, path + "/" + child.EntityName);
                    }
                }
                else
                {
                    foreach (var child in entity.Children)
                    {
                        AddEntity(child, child.EntityName);
                    }
                }
            }
        }

        public class ComponentView
        {
            [ReadOnly] [HorizontalGroup("info", 300, LabelWidth = 100)] [GUIColor(0.8f, 0.5f, 0.3f)]
            public long EntityID;

            [ShowInInspector]
            [HorizontalGroup("info", 200, LabelWidth = 100)]
            [GUIColor(0.8f, 0.5f, 0.3f)]
            public int ChildCount => Entity.Children.Count;

            [ShowInInspector]
            [HorizontalGroup("info", 200, LabelWidth = 150)]
            [GUIColor(0.8f, 0.5f, 0.3f)]
            public int ComponentCount => Components.Count;

            [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true,
                DraggableItems = false,
                NumberOfItemsPerPage = 99,
                ShowItemCount = false, ListElementLabelName = "@Name",
                ElementColor = "ElementColor")]
            [ShowInInspector]
            [HideIf("_count", 0)]
            public List<YuoComponent> Components = new();

            [HideInInspector] public YuoEntity Entity;

            [OnInspectorGUI]
            public void OnSelect()
            {
                if (Selection.activeGameObject != _gameObject)
                {
                    if (Entity.TryGetComponent<EntitySelectComponent>(out var select))
                    {
                        Selection.activeGameObject = select.SelectGameObject;
                        _gameObject = select.SelectGameObject;
                    }
                }
            }

            private GameObject _gameObject;

            private Color ElementColor(int index)
            {
                return HashCodeToColor(Components[index].Type.Name.GetHashCode());
            }

            private Color HashCodeToColor(int hashCode)
            {
                float h = Math.Abs(hashCode / 1.3f % 1f);
                return Color.HSVToRGB(h, 0.6f, 0.55f);
            }

            int _count;

            public void Update()
            {
                _count = Components.Count;
                if (Entity == null) return;
                Components.Clear();
                if (Entity.TryGetComponent<EntitySelectComponent>(out var select))
                {
                    _gameObject = select.SelectGameObject;
                }

                foreach (var component in Entity.Components.Values)
                {
                    if (!Components.Contains(component))
                    {
                        Components.Add(component);
                    }
                }
            }
        }

        public class SystemView
        {
            [HorizontalGroup()]
            [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true,
                DraggableItems = false,
                ShowItemCount = false)]
            [ShowInInspector]
            [ReadOnly]
            [LabelText("支持的类型")]
            public List<string> InfluenceType = new();

            [HorizontalGroup()]
            [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true,
                DraggableItems = false,
                ShowItemCount = false)]
            [ShowInInspector]
            [ReadOnly]
            [LabelText("触发的类型")]
            public List<string> InterfaceType = new();

            [ReadOnly] public short SystemOrder;

            [BoxGroup("Time")] [LabelWidth(100)] [LabelText("执行耗时")] [SuffixLabel("毫秒")] [ReadOnly]
            public double TimeConsuming;

            [BoxGroup("Time")] [LabelWidth(100)] [LabelText("总耗时")] [SuffixLabel("毫秒")] [ReadOnly]
            public double TotalTimeConsuming;

            [LabelWidth(100)] [BoxGroup("Time")] [LabelText("执行")] [SuffixLabel("次")] [ReadOnly]
            public long RunCount;

            [BoxGroup("Time")]
            [LabelText("平均耗时")]
            [SuffixLabel("毫秒")]
            [LabelWidth(100)]
            [ReadOnly]
            [ShowInInspector]
            public double AverageTimeConsuming => RunCount == 0 ? 0 : TotalTimeConsuming / RunCount;

            [BoxGroup("Time")]
            [LabelText("总耗时占比")]
            [LabelWidth(100)]
            [SuffixLabel("万分之一")]
            [ReadOnly]
            [ShowInInspector]
            public string RatioForMain => (TotalTimeConsuming / Time.unscaledTime * 10).ToString("F4");

            public SystemBase System;
            public List<YuoEntity> Entities = new();

            [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true,
                DraggableItems = false,
                ShowItemCount = false)]
            [ShowInInspector]
            [ReadOnly]
            [LabelText("Entities")]
            public List<string> Entity = new();

            public void Update()
            {
                TimeConsuming = System.TimeConsuming;
                TotalTimeConsuming = System.TotalTimeConsuming;
                RunCount = System.TotalRunCount;
                Entities = System.Entities;
                Entity.Clear();
                Entity.AddRange(Entities.Select(e => e.EntityName));
            }
        }
    }
}