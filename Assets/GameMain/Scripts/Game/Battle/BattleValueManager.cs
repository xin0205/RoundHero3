using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class BattleValueManager: Singleton<BattleValueManager>
    {
        public Dictionary<int, Entity>  BattleValueEntities = new ();
        //private bool isShowDisplayValue = false;
        private int curEntityIdx = 0;
        private int showEntityIdx = 0;
        
        public Random Random;
        private int randomSeed;

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            curEntityIdx = 0;
            showEntityIdx = 0;

        }
        
        public void ShowDisplayValue(int unitID)
        {
            UnShowDisplayValues();
            ShowDisplayValues(unitID);
        }

        public async void ShowDisplayValues(int unitID)
        {
            //isShowDisplayValue = true;
            
            BattleValueEntities.Clear();
            
            var entityIdx = curEntityIdx;
            var triggerDatas = BattleFightManager.Instance.GetAttackData(unitID);
            foreach (var triggerData in triggerDatas)
            {
                var unit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);

                if (unit != null)
                {
                    curEntityIdx++;
                }
                    
            }

            var idx = 0;
            foreach (var triggerData in triggerDatas)
            {
                GameUtility.DelayExcute(0.25f * idx, () =>
                {
                    ShowValues(triggerData, entityIdx);
                });
                idx++;

            }

        }

        private async void ShowValues(TriggerData triggerData, int entityIdx)
        {
            var actionUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.ActionUnitID);
            var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            var value = (int)(triggerData.Value + triggerData.DeltaValue);
            
            Entity entity;
            if (effectUnit is BattleSoliderEntity solider)
            {
                var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnit.UnitCamp);
                        
                entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(effectUnit.Position, heroEntity.Position,
                    value, entityIdx, true);
                        
                if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showEntityIdx)
                {
                    GameEntry.Entity.HideEntity(entity);
                }
                else
                {
                    BattleValueEntities.Add(entity.Entity.Id, entity);
                }
            }
            else
            {
                entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(actionUnit.Position,
                    effectUnit.Position, value, entityIdx);
                        
                if ((entity as BattleDisplayValueEntity).BattleDisplayValueEntityData.EntityIdx < showEntityIdx)
                {
                    GameEntry.Entity.HideEntity(entity);
                }
                else
                {
                    BattleValueEntities.Add(entity.Entity.Id, entity);
                }
            }

            entityIdx++;
        }
        
        public void UnShowDisplayValues()
        {

            showEntityIdx = curEntityIdx;
            
            foreach (var kv in BattleValueEntities)
            {
                if (GameEntry.Entity.HasEntity(kv.Value.Entity.Id))
                {
                    GameEntry.Entity.HideEntity(kv.Value.Entity);
                }
              
            }
            BattleValueEntities.Clear();
            
        }
    }
}