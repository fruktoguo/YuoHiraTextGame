using UnityEngine.UI;
using YuoTools.UI;

namespace YuoTools.Extend.UI
{
    public partial class SpawnUICodeConfig
    {
        public void Init1()
        {
            SpawnType.Add(typeof(YuoUIDrag));
            SpawnType.Add(typeof(YuoDropDown));
            SpawnType.Add(typeof(YuoToggle));
            SpawnType.Add(typeof(YuoToggleGroup));
            
            RemoveType.Add(typeof(YuoDropDown), typeof(Button));
        }
    }
}