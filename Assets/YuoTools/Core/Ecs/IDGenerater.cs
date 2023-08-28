namespace YuoTools.Main.Ecs
{
    public class IDGenerate
    {
        private static IDGenerate instance;

        public static IDGenerate Instance => instance ??= new IDGenerate();

        public enum IDType
        {
            Scene,
        }

        public static long GetID(IDType type, long id)
        {
            switch (type)
            {
                case IDType.Scene:
                    return id + 10000;
                default:
                    return id;
            }
        }

        public static long GetID(YuoEntity entity)
        {
            return entity.GetHashCode();
        }

        public static long GetID(string name)
        {
            return name.GetHashCode() + 10000000000;
        }
    }
}