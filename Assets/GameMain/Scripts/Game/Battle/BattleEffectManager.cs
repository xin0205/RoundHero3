using System.Threading.Tasks;
using UnityEngine;

namespace RoundHero
{
    public class BattleEffectManager : Singleton<BattleEffectManager>
    {
        public async Task<EffectEntity> ShowHurtRoundStartEffect(Vector3 effectPos, Transform parent = null)
        {
             return await ShowEffectEntity("EffectHurtRoundStartEntity", effectPos, Vector3.zero, parent);
        }
        
        public async Task<EffectEntity> ShowCollideEffect(Vector3 effectPos, Transform parent = null)
        {
            return await ShowEffectEntity("EffectCollideEntity", effectPos, Vector3.zero, parent);
        }
        
        private async Task<EffectEntity> ShowEffectEntity(string effectName, Vector3 effectPos, Vector3 lookAtPos, Transform parent = null)
        {
            var effectAttackEntity = await GameEntry.Entity.ShowEffectEntityAsync(effectName, effectPos);
            if (parent != null)
            {
                effectAttackEntity.transform.SetParent(parent);
                effectAttackEntity.transform.position = effectPos;
            }

            if (lookAtPos != Vector3.zero)
            {
                effectAttackEntity.transform.LookAt(new Vector3(lookAtPos.x, effectAttackEntity.transform.position.y, lookAtPos.z));
            }
            
            if (!effectAttackEntity.AutoHide)
            {
                GameUtility.DelayExcute(1f, () =>
                {
                    GameEntry.Entity.HideEntity(effectAttackEntity);
                });
            }
            
            return effectAttackEntity;
        }
    }
}