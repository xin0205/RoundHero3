using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unitIdx),
                BattleFightManager.Instance.GetInDirectAttackDatas(unitIdx));
            curEntityIdx += triggerDataDict.Count;
  
            
            // foreach (var triggerData in triggerDatas)
            // {
            //     var unit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            //
            //     if (unit != null)
            //     {
            //         curEntityIdx++;
            //     }
            //         
            // }

            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                var _entityIdx = entityIdx;
                var values = kv.Value;
                //ShowValues(kv.Value, entityIdx);
                GameUtility.DelayExcute(0.25f * idx, () =>
                {
                    ShowValues(values, _entityIdx);
                });
                idx++;
                entityIdx++;
            }

        }

        private async void ShowValues(List<TriggerData> triggerDatas, int entityIdx)
        {
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].ActionUnitIdx);
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].EffectUnitIdx);

            var value = 0;
            foreach (var triggerData in triggerDatas)
            {
                value += (int)(triggerData.Value + triggerData.DeltaValue);
            }
            
            
            Entity entity;
            if (effectUnit is BattleSoliderEntity solider)
            {
                var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnit.UnitCamp);
                
                var effectUnitPos = effectUnit.Root.position;
                
                // var effectUnitFlyEndGridPosIdx =
                //     BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);
                //
                // if (effectUnitFlyEndGridPosIdx != -1)
                // {
                //     effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyEndGridPosIdx);
                // }

                var targetPos = heroEntity.Root.position;
                
                // var heroMovePaths = BattleFightManager.Instance.GetMovePaths(heroEntity.UnitIdx, actionUnit.UnitIdx);
                // if (heroMovePaths != null && heroMovePaths.Count > 0)
                // {
                //     targetPos = GameUtility.GridPosIdxToPos(heroMovePaths[heroMovePaths.Count - 1]);
                // }

                effectUnitPos.y += 1f;
                targetPos.y += 1f;
                
                entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(effectUnitPos, targetPos,
                    value, entityIdx, true);
                
                entity.transform.parent = heroEntity.Root;
                
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
                var effectUnitPos = effectUnit.Root.position;
                // var effectUnitFlyGridPosIdx =
                //     BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);
                //
                // if (effectUnitFlyGridPosIdx != -1)
                // {
                //     effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyGridPosIdx);
                // }
                
                effectUnitPos.y += 1f;
                
                entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(actionUnit.Position,
                    effectUnitPos, value, entityIdx);

                //entity.transform.parent = effectUnit.Root;
                        
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