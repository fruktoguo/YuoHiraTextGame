namespace YuoTools.Main.Ecs
{
    /// <summary>
    /// 不会存,只是简化访问
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YuoComponentGet<T> : YuoComponent where T : YuoComponent, new()
    {
        private static YuoEntity instanceEntity;

        public static T Get
        {
            get
            {
                if (!(instanceEntity is { IsDisposed: false }))
                {
                    instanceEntity = YuoWorld.Main;
                }

                return instanceEntity.GetOrAddComponent<T>();
            }
        }

        public static void SetInstance(YuoEntity entity)
        {
            instanceEntity = entity;
            entity.AddComponent<T>();
        }
    }

    /// <summary>
    /// 会存的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YuoComponentInstance<T> : YuoComponent where T : YuoComponent, new()
    {
        private static YuoEntity instanceEntity;

        private static T instance;

        public static T Get
        {
            get
            {
                if (instance is { IsDisposed: false }) return instance;

                if (instanceEntity == null || instanceEntity.IsDisposed)
                {
                    instanceEntity = YuoWorld.Main;
                }

                instance = instanceEntity.GetOrAddComponent<T>();
                
                return instance;
            }
        }

        public static void SetInstance(YuoEntity entity)
        {
            instanceEntity = entity;
            entity.AddComponent<T>();
        }
    }
}