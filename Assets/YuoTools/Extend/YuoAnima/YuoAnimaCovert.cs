using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools;
using YuoTools.Main.Ecs;

namespace YuoAnima
{
    [RequireComponent(typeof(Animator))]
    public class YuoAnimaCovert : MonoBehaviour
    {
#if UNITY_EDITOR
        [Button("同步HashCode", ButtonHeight = 60)]
        private void CreateAnimaScriptable()
        {
            Debug.Log($"同步HashCode__{gameObject}");
            var eac = GetComponent<Animator>().runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            UnityEditor.Animations.AnimatorStateMachine stateMachine;
            if (eac != null)
            {
                stateMachine = eac.layers[0].stateMachine;
                if (GetComponent<Animator>().GetBehaviour<YuoStateMachineBehaviour>() == null)
                {
                    stateMachine.AddStateMachineBehaviour<YuoStateMachineBehaviour>();
                }
            }
            else
            {
                var eave = GetComponent<Animator>().runtimeAnimatorController as AnimatorOverrideController;
                if (eave == null) return;
                stateMachine = (eave.runtimeAnimatorController as UnityEditor.Animations.AnimatorController)?.layers[0].stateMachine;
            }

            if (stateMachine == null)
            {
                Debug.LogError("Error");
                return;
            }

            if (GetComponent<Animator>().GetBehaviour<YuoStateMachineBehaviour>() == null)
            {
                stateMachine.AddStateMachineBehaviour<YuoStateMachineBehaviour>();
            }

            var aaa = GetComponent<Animator>().runtimeAnimatorController;
            foreach (var item in stateMachine.states)
            {
                // item.state.motion.averageDuration.Log();
                HashCodeHelper.Add(item.state.name, item.state.nameHash);
            }
        }
#endif
        public YuoAnimaComponent Convert(YuoEntity entity)
        {
            var anima = entity.AddComponent<YuoAnimaComponent>();
            anima.Init(GetComponent<Animator>());
            Destroy(this);
            return anima;
        }
    }
}