using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace YuoTools.Editor
{
    public class ShowCodeLineData : SerializedScriptableObject
    {
        public string path;
        public List<string> excludes = new();
        public ShowCodeLine.RankType rankType = ShowCodeLine.RankType.有效行数;
        public ShowCodeLine.RankAsc ranks = ShowCodeLine.RankAsc.从大到小;
    }
}