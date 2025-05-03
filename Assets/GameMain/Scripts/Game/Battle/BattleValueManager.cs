using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = System.Random;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, Entity>  BattleValueEntities = new ();
        private int curValueEntityIdx = 0;
        private int showValueEntityIdx = 0;

        
        // public void ShowDisplayValue(int unitIdx)
        // {
        //     UnShowDisplayValues();
        //     ShowDisplayValues(unitIdx);
        // }
        
        public void ShowHurtDisplayValue(int effectUnitIdx, int actionUnitIdx)
        {
            //UnShowDisplayValues();
            ShowHurtDisplayValues(effectUnitIdx, actionUnitIdx);
        }
        
        public async void ShowHurtDisplayValues(int effectUnitIdx, int actionUnitIdx)
        {
            //isShowDisplayValue = true;
            
            //BattleValueEntities.Clear();
            
            var entityIdx = curValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdx),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdx));

            foreach (var kv in triggerDataDict)
            {
                curValueEntityIdx += kv.Value.Count;
            }
            
            //curValueEntityIdx += triggerDataDict.Count;

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
                // var _entityIdx = entityIdx;
                // var values = kv.Value;
                //
                // GameUtility.DelayExcute(0.25f * idx, () =>
                // {
                //     ShowValues(values, _entityIdx);
                // });
                
                
                ShowValues(kv.Value, entityIdx);
                idx++;
                entityIdx += kv.Value.Count;
            }

        }

        public async void ShowDisplayValue(int unitIdx)
        {
            //BattleValueEntities.Clear();
            
            var entityIdx = curValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unitIdx),
                BattleFightManager.Instance.GetInDirectAttackDatas(unitIdx));
            //curValueEntityIdx += triggerDataDict.Count;

            foreach (var kv in triggerDataDict)
            {
                curValueEntityIdx += kv.Value.Count;
            }
            
  
            //Log.Debug("triggerDataDict.Count:" + triggerDataDict.Count);
            
            
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                var _entityIdx = entityIdx;
                var values = kv.Value;
                
                ShowValues(values, _entityIdx);
                idx++;
                entityIdx += kv.Value.Count;
            }

        }

        public async void ShowActionSort(int sort)
        {
            var entityIdx = curValueEntityIdx;
            curValueEntityIdx += 1;
            
            var effectUnitPos = Root.position;


            effectUnitPos.y += 1f;
            effectUnitPos.z -= 0.5f;

            var entity = await GameEntry.Entity.ShowBattleValueEntityAsync(
                effectUnitPos, sort, entityIdx);

            if ((entity as BattleValueEntity).BattleValueEntityData.EntityIdx <
                showValueEntityIdx)
            {

                GameEntry.Entity.HideEntity(entity);
            }
            else
            {
                BattleValueEntities.Add(entity.Entity.Id, entity);
            }
        }
        
        

        private async void InternalShowValue(BattleUnitEntity effectUnit, int value, int entityIdx)
        {
            if (effectUnit == null)
            {
                return;
            }
            
            if (effectUnit is BattleMonsterEntity)
            {
                var effectUnitPos = effectUnit.Root.position;


                effectUnitPos.y += 1f;
                effectUnitPos.z -= 0.5f;

                var entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(
                    effectUnitPos, value, entityIdx);

                if ((entity as BattleDisplayValueEntity).BattleDisplayValueEntityData.EntityIdx <
                    showValueEntityIdx)
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

                var uiCorePos = AreaController.Instance.UICore.transform.position;
                uiCorePos.y -= 0.4f;

                var pos = RectTransformUtility.WorldToScreenPoint(AreaController.Instance.UICamera,
                    uiCorePos);

                Vector3 position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
                Vector3 uiCoreWorldPos = Camera.main.ScreenToWorldPoint(position);

                var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(effectUnitPos,
                    uiCoreWorldPos,
                    value, entityIdx, true, effectUnit is BattleCoreEntity ? false : true);



                //entity.transform.parent = effectUnit.Root;

                if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showValueEntityIdx)
                {

                    GameEntry.Entity.HideEntity(entity);
                }
                else
                {
                    if (GameEntry.Entity.HasEntity(effectUnit.Entity.Id))
                    {
                        GameEntry.Entity.AttachEntity(entity.Entity.Id, effectUnit.Entity.Id);
                    }

                    BattleValueEntities.Add(entity.Entity.Id, entity);
                }

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

            var idx = 0;
            foreach (var triggerData in triggerDatas)
            {
                var value = (int)triggerData.ActualValue;
                // if(value == 0)
                //     continue;

                GameUtility.DelayExcute(idx *0.25f, () =>
                {
                    InternalShowValue(effectUnit, value, entityIdx++);
                });
                //InternalShowValue(effectUnit, value, entityIdx++);

                idx++;



            }


            // var value = 0;
            // foreach (var triggerData in triggerDatas)
            // {
            //     value += (int)triggerData.ActualValue;
            // }
            //
            // if(value == 0)
            //     return;
            //
            //
            //
            // if (effectUnit is BattleMonsterEntity)
            // {
            //     var effectUnitPos = effectUnit.Root.position;
            //     
            //     
            //     effectUnitPos.y += 1f;
            //     effectUnitPos.z -= 0.3f;
            //     
            //     var entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(
            //         effectUnitPos, value, entityIdx);
            //     
            //     if ((entity as BattleDisplayValueEntity).BattleDisplayValueEntityData.EntityIdx < showValueEntityIdx)
            //     {
            //
            //         GameEntry.Entity.HideEntity(entity);
            //     }
            //     else
            //     {
            //         BattleValueEntities.Add(entity.Entity.Id, entity);
            //     }
            //     
            // }
            // else
            // {
            //     var effectUnitPos = effectUnit.Root.position;
            //     
            //     effectUnitPos.y += 1f;
            //
            //     var uiCorePos = AreaController.Instance.UICore.transform.position;
            //     uiCorePos.y -= 0.4f;
            //
            //     var pos = RectTransformUtility.WorldToScreenPoint(AreaController.Instance.UICamera,
            //         uiCorePos);
            //     
            //     Vector3 position = new Vector3(pos.x, pos.y,  Camera.main.transform.position.z);
            //     Vector3 uiCoreWorldPos = Camera.main.ScreenToWorldPoint(position);
            //
            //     var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(effectUnitPos, uiCoreWorldPos,
            //         value, entityIdx, true, effectUnit is BattleCoreEntity ? false : true);
            //     
            //     
            //     
            //     //entity.transform.parent = effectUnit.Root;
            //     
            //     if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showValueEntityIdx)
            //     {
            //
            //         GameEntry.Entity.HideEntity(entity);
            //     }
            //     else
            //     {
            //         if (GameEntry.Entity.HasEntity(effectUnit.Entity.Id))
            //         {
            //             GameEntry.Entity.AttachEntity(entity.Entity.Id, effectUnit.Entity.Id);
            //         }
            //         
            //         BattleValueEntities.Add(entity.Entity.Id, entity);
            //     }
            //    
            // }
            //
            // //entityIdx++;
        }
        
        public void UnShowDisplayValues()
        {
            showValueEntityIdx = curValueEntityIdx;
            
            foreach (var kv in BattleValueEntities)
            {
                if (GameEntry.Entity.HasEntity(kv.Value.Entity.Id))
                {
                    //Log.Debug("ID:" + kv.Value.Entity.Id + kv.Value.Entity.EntityAssetName);
                    GameEntry.Entity.HideEntity(kv.Value);
                }
              
            }
            BattleValueEntities.Clear();
            
        }
    }

}