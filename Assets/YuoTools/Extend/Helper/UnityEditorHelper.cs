namespace YuoTools.Extend.Helper
{
    public class UnityEditorHelper
    {
        public static void ClearConsole()
        {
#if UNITY_EDITOR
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            if (logEntries != null)
            {
                var clearMethod = logEntries.GetMethod("Clear",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                clearMethod?.Invoke(null, null);
            }
#endif
        }
    }
}