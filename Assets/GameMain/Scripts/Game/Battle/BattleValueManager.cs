using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, Entity>  BattleValueEntities = new ();
        private int curValueEntityIdx = 0;
        private int showValueEntityIdx = 0;

        
        public void ShowDisplayValue(int unitIdx)
        {
            UnShowDisplayValues();
            ShowDisplayValues(unitIdx);
        }
        
        public void ShowHurtDisplayValue(int effectUnitIdx, int actionUnitIdx)
        {
            UnShowDisplayValues();
            ShowHurtDisplayValues(effectUnitIdx, actionUnitIdx);
        }
        
        public async void ShowHurtDisplayValues(int effectUnitIdx, int actionUnitIdx)
        {
            //isShowDisplayValue = true;
            
            BattleValueEntities.Clear();
            
            var entityIdx = curValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdx),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdx));
            curValueEntityIdx += triggerDataDict.Count;

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

        public async void ShowDisplayValues(int unitIdx)
        {
            BattleValueEntities.Clear();
            
            var entityIdx = curValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unitIdx),
                BattleFightManager.Instance.GetInDirectAttackDatas(unitIdx));
            curValueEntityIdx += triggerDataDict.Count;
  

            
            
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                var _entityIdx = entityIdx;
                var values = kv.Value;
                
                ShowValues(values, _entityIdx);
                idx++;
                entityIdx++;
            }

        }

        private async void ShowValues(List<TriggerData> triggerDatas, int entityIdx)
        {
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].ActionUnitIdx);
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].EffectUnitIdx);
            
            if (effectUnit == null)
            {
                return;
            }

            var value = 0;
            foreach (var triggerData in triggerDatas)
            {
                value += (int)triggerData.ActualValue;
            }
            
            if(value == 0)
                return;
            
            
            Entity entity;
            if (effectUnit is BattleSoliderEntity solider)
            {

                var effectUnitPos = effectUnit.Root.position;
                
                effectUnitPos.y += 1f;

                var pos = RectTransformUtility.WorldToScreenPoint(AreaController.Instance.UICamera,
                    AreaController.Instance.UICore.transform.position);
                
                Vector3 position = new Vector3(pos.x, pos.y,  Camera.main.transform.position.z);
                Vector3 uiCorePos = Camera.main.ScreenToWorldPoint(position);
                
                entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(effectUnitPos, uiCorePos,
                    value, entityIdx, true);
                
                entity.transform.parent = effectUnit.Root;
                
                if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showValueEntityIdx)
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
                
                
                effectUnitPos.y += 1f;
                effectUnitPos.z -= 0.3f;
                
                entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(
                    effectUnitPos, value, entityIdx);
                
                if ((entity as BattleDisplayValueEntity).BattleDisplayValueEntityData.EntityIdx < showValueEntityIdx)
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
            showValueEntityIdx = curValueEntityIdx;
            
            foreach (var kv in BattleValueEntities)
            {
                if (GameEntry.Entity.HasEntity(kv.Value.Entity.Id))
                {
                    GameEntry.Entity.HideEntity(kv.Value);
                }
              
            }
            BattleValueEntities.Clear();
            
        }
    }

}