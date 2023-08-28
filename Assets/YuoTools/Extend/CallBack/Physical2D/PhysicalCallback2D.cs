using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.PhysicalCallback
{
    public class PhysicalCallback2D : MonoBehaviour
    {
        public PhysicalCallback2DComponent CallbackComponent;
        public bool OpenOnCollisionEnter2D = true;
        public bool OpenOnCollisionExit2D = true;
        public bool OpenOnCollisionStay2D = true;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!OpenOnCollisionEnter2D) return;
            if (CallbackComponent.IsNull()) return;
            CallbackComponent.eventData = other;
            CallbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            YuoWorld.RunSystem<IOnCollisionEnter2D>(CallbackComponent);

            CallbackComponent.eventDataEntity = null;
            CallbackComponent.eventData = null;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!OpenOnCollisionExit2D) return;
            if (CallbackComponent.IsNull()) return;
            CallbackComponent.eventData = other;
            CallbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            YuoWorld.RunSystem<IOnCollisionExit2D>(CallbackComponent);

            CallbackComponent.eventDataEntity = null;
            CallbackComponent.eventData = null;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!OpenOnCollisionStay2D) return;
            if (CallbackComponent.IsNull()) return;
            CallbackComponent.eventData = other;
            CallbackComponent.eventDataEntity = GameObjectMapManager.Get.GetMap(other.gameObject);

            YuoWorld.RunSystem<IOnCollisionStay2D>(CallbackComponent);

            CallbackComponent.eventDataEntity = null;
            CallbackComponent.eventData = null;
        }
    }

    public class PhysicalCallback2DComponent : YuoComponent
    {
        public Transform transform;
        public Collision2D eventData;
        public YuoEntity eventDataEntity;
    }

    public class PhysicalCallback2DComponentStartSystem : YuoSystem<PhysicalCallback2DComponent>, IStart
    {
        protected override void Run(PhysicalCallback2DComponent component)
        {
        }
    }

    public interface IOnCollisionEnter2D : ISystemTag
    {
    }

    public interface IOnCollisionExit2D : ISystemTag
    {
    }

    public interface IOnCollisionStay2D : ISystemTag
    {
    }
}