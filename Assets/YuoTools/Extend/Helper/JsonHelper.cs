using System.Text;
using Newtonsoft.Json.Linq;

namespace YuoTools.Extend.Helper
{
    public class JsonHelper
    {
        //Json转C#实体类
        public static string JsonToCSharp(string json)
        {
            var sb = new StringBuilder();
            var jObject = JObject.Parse(json);
            foreach (var item in jObject)
            {
                var name = item.Key;
                var value = item.Value;
                var type = value.Type.ToString();
                switch (type)
                {
                    case "String":
                        sb.AppendLine($"public string {name} {{ get; set; }}");
                        break;
                    case "Integer":
                        sb.AppendLine($"public int {name} {{ get; set; }}");
                        break;
                    case "Float":
                        sb.AppendLine($"public float {name} {{ get; set; }}");
                        break;
                    case "Double":
                        sb.AppendLine($"public double {name} {{ get; set; }}");
                        break;
                    case "Boolean":
                        sb.AppendLine($"public bool {name} {{ get; set; }}");
                        break;
                    case "Array":
                        sb.AppendLine($"public List<{value[0].Type}> {name} {{ get; set; }}");
                        break;
                    case "Object":
                        sb.AppendLine($"public {name} {name} {{ get; set; }}");
                        break;
                }
            }

            return sb.ToString();
        }
    }
}