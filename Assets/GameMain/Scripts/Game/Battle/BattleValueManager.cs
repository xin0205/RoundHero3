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
        
        public void ShowDisplayValue(int unitIdx)
        {
            UnShowDisplayValues();
            ShowDisplayValues(unitIdx);
        }

        public async void ShowDisplayValues(int unitIdx)
        {
            //isShowDisplayValue = true;
            
            BattleValueEntities.Clear();
            
            var entityIdx = curEntityIdx;
            var triggerDatas = BattleFightManager.Instance.GetAttackDatas(unitIdx);
            foreach (var triggerData in triggerDatas)
            {
                var unit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);

                if (unit != null)
                {
                    curEntityIdx++;
                }
                    
            }

            var idx = 0;
            foreach (var triggerData in triggerDatas)
            {
                ShowValues(triggerData, entityIdx);
                // GameUtility.DelayExcute(0.25f * idx, () =>
                // {
                //     ShowValues(triggerData, entityIdx);
                // });
                idx++;

            }

        }

        private async void ShowValues(TriggerData triggerData, int entityIdx)
        {
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            var value = (int)(triggerData.Value + triggerData.DeltaValue);
            
            Entity entity;
            if (effectUnit is BattleSoliderEntity solider)
            {
                var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnit.UnitCamp);
                
                var effectUnitPos = effectUnit.Position;
                
                var effectUnitFlyGridPosIdx =
                    BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);

                if (effectUnitFlyGridPosIdx != -1)
                {
                    effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyGridPosIdx);
                }
                        
                entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(effectUnitPos, heroEntity.Position,
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
                var effectUnitPos = effectUnit.Position;
                var effectUnitFlyGridPosIdx =
                    BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);

                if (effectUnitFlyGridPosIdx != -1)
                {
                    effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyGridPosIdx);
                }
                
                effectUnitPos.y += 1f;
                
                entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(actionUnit.Position,
                    effectUnitPos, value, entityIdx);
                        
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