using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleCoreEntity : BattleUnitEntity
    {
        public BattleCoreEntityData BattleCoreEntityData { get; protected set; }
        
        public virtual Vector3 Position
        {
            get => transform.position; 
            set => transform.position = value;
        }

        public virtual int GridPosIdx
        {
            get => BattleCoreEntityData.BattleCoreData.GridPosIdx; 
            set => BattleCoreEntityData.BattleCoreData.GridPosIdx = value;
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleCoreEntityData = userData as BattleCoreEntityData;
            if (BattleCoreEntityData == null)
            {
                Log.Error("Error BattleCoreEntityData");
                return;
            }
            
            BattleUnitData = BattleCoreEntityData.BattleCoreData;
            
            var unitDescFormData = GetComponent<UnitDescTriggerItem>().UnitDescFormData;
            unitDescFormData.UnitCamp = BattleCoreEntityData.BattleCoreData.UnitCamp;
            unitDescFormData.UnitRole = EUnitRole.Hero;

            unitDescFormData.Idx = BattleCoreEntityData.BattleCoreData.Idx;
        }
        
        public void UpdateCacheHPDelta()
        {
            ChangeCurHP(HeroManager.Instance.BattleHeroData.CacheHPDelta, false, false, true, false);
            
            if (CurHP == 0)
            {
                CurHP = -1;
                GameUtility.DelayExcute(1.5f, () =>
                {
                    Dead();
                });
               
            }
            // if (BattleHeroEntityData.BattleHeroData.CacheHPDelta < 0)
            // {
            //     Hurt();
            // }
            HeroManager.Instance.BattleHeroData.CacheHPDelta = 0;
            //cacheHPText.text = BattleHeroEntityData.BattleHeroData.CacheHPDelta.ToString();
        }
        
        public override void RefreshHP()
        {
            hpAndDamageNode.SetActive(false);
            damageNode.SetActive(false);

            

        }
        
        public override void OnPointerEnter(BaseEventData baseEventData)
        {
            base.OnPointerEnter(baseEventData);
            
            // if(CurHP <= 0)
            //     return;
            //
            // if(IsMove)
            //     return;
            //
            // ShowHurtTags(BattleCoreEntityData.BattleCoreData.Idx);
            
            
        }

        

        public override void OnPointerExit(BaseEventData baseEventData)
        {
            base.OnPointerExit(baseEventData);

            //UnShowTags();
        }

    }
}