using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.PhysicalCallback
{
    public class PhysicalCallbackTrigger2D : MonoBehaviour
    {
        [ShowInInspector] public PhysicalCallbackTrigger2DComponent callbackComponent;
        public bool OpenOnTriggerEnter2D = true;
        public bool OpenOnTriggerExit2D = true;
        public bool OpenOnTriggerStay2D = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!OpenOnTriggerEnter2D) return;
            // if (callbackComponent.IsNull())
            // {
            //     "callbackComponent is null".LogError();
            // }
            if (callbackComponent.IsNull()) return;

            callbackComponent.eventData = other;
            callbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);
            callbackComponent.RunSystem<IOnTriggerEnter2D>();

            callbackComponent.eventDataEntity = null;
            callbackComponent.eventData = null;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!OpenOnTriggerExit2D) return;
            if (callbackComponent.IsNull()) return;

            callbackComponent.eventData = other;
            callbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            callbackComponent.RunSystem<IOnTriggerExit2D>();

            callbackComponent.eventDataEntity = null;
            callbackComponent.eventData = null;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!OpenOnTriggerStay2D) return;
            if (callbackComponent.IsNull()) return;

            callbackComponent.eventData = other;
            callbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            callbackComponent.RunSystem<IOnTriggerStay2D>();

            callbackComponent.eventDataEntity = null;
            callbackComponent.eventData = null;
        }
    }

    public class PhysicalCallbackTrigger2DComponent : YuoComponent
    {
        public Transform transform;
        public Collider2D eventData;
        public YuoEntity eventDataEntity;
        public PhysicalCallbackTrigger2D callback;
    }

    public class
        PhysicalCallbackTrigger2DComponentStartSystem :
            YuoSystem<PhysicalCallbackTrigger2DComponent, TransformComponent>, IAwake, IDestroy
    {
        public override string Group => SystemGroupConst.CallBack;

        protected override void Run(PhysicalCallbackTrigger2DComponent component, TransformComponent tran)
        {
            if (RunType == SystemTagType.Awake)
            {
                component.transform = tran.Transform;
                //检查是否有物理回调组件
                var callback = component.transform.GetOrAddComponent<PhysicalCallbackTrigger2D>();
                callback.callbackComponent = component;
                component.callback = callback;
                // "生成物理回调组件".Log();
            }

            if (RunType == SystemTagType.Destroy)
            {
                component.callback.callbackComponent = null;
                Object.Destroy(component.callback);
            }
        }
    }

    public interface IOnTriggerEnter2D : ISystemTag
    {
    }

    public interface IOnTriggerExit2D : ISystemTag
    {
    }

    public interface IOnTriggerStay2D : ISystemTag
    {
    }
}