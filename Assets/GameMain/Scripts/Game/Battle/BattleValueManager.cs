﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private void InternalShowHurtDisplayValue(int effectUnitIdx, Dictionary<int, List<TriggerData>> triggerDataDict)
        {
            var entityIdx = curValueEntityIdx;
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            
            if (effectUnit is BattleSoliderEntity)
            {
                foreach (var kv in triggerDataDict)
                {
                    curValueEntityIdx += kv.Value.Count;
                }
                
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
            else
            {
                var value = 0;

                foreach (var kv in triggerDataDict)
                {
                    foreach (var triggerData in kv.Value)
                    {
                        value += (int)triggerData.ActualValue;
                    }
                }

                if (value != 0)
                {
                    curValueEntityIdx += 1;
                    InternalShowValue(effectUnit, value, entityIdx++);
                }
               
            }
        }
        
        public async void ShowTacticHurtDisplayValues(int effectUnitIdx)
        {

            var triggerDataDict = BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);

            InternalShowHurtDisplayValue(effectUnitIdx, triggerDataDict);

            
        }
        
        public async void ShowHurtDisplayValues(int effectUnitIdx, int actionUnitIdx)
        {

            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdx),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdx));


            InternalShowHurtDisplayValue(effectUnitIdx, triggerDataDict);
            

            

        }

        public async void ShowDisplayValue(int actionUnitIdx)
        {
            //BattleValueEntities.Clear();
            var actionUnit =  BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);
            
            var entityIdx = curValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(actionUnitIdx),
                BattleFightManager.Instance.GetInDirectAttackDatas(actionUnitIdx));
            //curValueEntityIdx += triggerDataDict.Count;

            foreach (var kv in triggerDataDict)
            {
                curValueEntityIdx += 1;
            }
            
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                var effectUnit =  BattleUnitManager.Instance.GetUnitByIdx(kv.Key);

                if (kv.Key == PlayerManager.Instance.PlayerData.BattleHero.Idx)
                {
                    var value = 0;
                    foreach (var triggerData in kv.Value)
                    {
                        ShowHeroValue(triggerData.ActionUnitGridPosIdx, (int)triggerData.ActualValue, entityIdx);
                        idx++;
                        entityIdx += kv.Value.Count;
                    }

                }
                else if (effectUnit is BattleSoliderEntity)
                {
                    ShowValues(kv.Value, entityIdx);
                    idx++;
                    entityIdx += kv.Value.Count;
                }
                else
                {
                    var value = 0;
                    foreach (var triggerData in kv.Value)
                    {
                        value += (int)triggerData.ActualValue;
                    }

                    if (value != 0)
                    {
                        InternalShowValue(effectUnit, value, entityIdx++);
                    }
               
                }

            }
   
            //Log.Debug("triggerDataDict.Count:" + triggerDataDict.Count);
            
            
            // var idx = 0;
            // foreach (var kv in triggerDataDict)
            // {
            //     var _entityIdx = entityIdx;
            //     var values = kv.Value;
            //     
            //     ShowValues(values, _entityIdx);
            //     idx++;
            //     entityIdx += kv.Value.Count;
            // }

        }

        public async void ShowActionSort(int sort)
        {
            var entityIdx = curValueEntityIdx;
            curValueEntityIdx += 1;
            
            var effectUnitPos = Root.position;
            
            // effectUnitPos.y += 1f;
            // effectUnitPos.z -= 0.5f;
            
            var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);

            uiLocalPoint.y += 50f;

            var entity = await GameEntry.Entity.ShowBattleValueEntityAsync(
                uiLocalPoint, sort, entityIdx);

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
        
        private async Task ShowHeroValue(int gridPosIdx, int value, int entityIdx)
        {
            var gridPos = GameUtility.GridPosIdxToPos(gridPosIdx);
            gridPos.y += 1f;
                
            var uiCorePos = AreaController.Instance.UICore.transform.localPosition;
            uiCorePos.y -= 25f;
            var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), gridPos);
            //var uiLocalPoint2 = uiLocalPoint;
            uiLocalPoint.y += 25f;
            //uiLocalPoint2.y += 75f;
                
            var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(uiLocalPoint,
                uiCorePos,
                value, entityIdx, true, false);

                
            //Log.Debug("2ShowDisplayValues:" + (entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx + "-" + showValueEntityIdx);
            if (GameEntry.Entity.HasEntity(entity.Id))
            {
                if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showValueEntityIdx)
                {

                    GameEntry.Entity.HideEntity(entity);
                }
                else
                {

                    BattleValueEntities.Add(entity.Entity.Id, entity);
                }
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
                // var effectUnitPos = effectUnit.Root.position;
                //
                // var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                //     AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);
                //
                // uiLocalPoint.y += 50f;
                //
                // var entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(
                //     uiLocalPoint, value, entityIdx);
                //
                // if (GameEntry.Entity.HasEntity(entity.Id))
                // {
                //     if ((entity as BattleDisplayValueEntity).BattleDisplayValueEntityData.EntityIdx <
                //         showValueEntityIdx)
                //     {
                //
                //         GameEntry.Entity.HideEntity(entity);
                //     }
                //     else
                //     {
                //     
                //         BattleValueEntities.Add(entity.Id, entity);
                //     }
                // }
                var effectUnitPos = effectUnit.Root.position;

                

                var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);
                var uiLocalPoint2 = uiLocalPoint;
                
                uiLocalPoint.y += 25f;
                uiLocalPoint2.y += 75f;
                
                var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(uiLocalPoint,
                    uiLocalPoint2,
                    value, entityIdx, true, effectUnit is BattleSoliderEntity);

                
                //Log.Debug("2ShowDisplayValues:" + (entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx + "-" + showValueEntityIdx);
                if (GameEntry.Entity.HasEntity(entity.Id))
                {
                    if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showValueEntityIdx)
                    {

                        GameEntry.Entity.HideEntity(entity);
                    }
                    else
                    {

                        BattleValueEntities.Add(entity.Entity.Id, entity);
                    }
                }

            }
            else
            {
                var effectUnitPos = effectUnit.Root.position;

                
                var uiCorePos = AreaController.Instance.UICore.transform.localPosition;
                uiCorePos.y -= 25f;
                var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);
                var uiLocalPoint2 = uiLocalPoint;
                uiLocalPoint.y += 25f;
                uiLocalPoint2.y += 75f;
                
                var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(uiLocalPoint,
                    value < 0 ? uiCorePos : uiLocalPoint2,
                    value, entityIdx, true, effectUnit is BattleSoliderEntity && value < 0);

                
                //Log.Debug("2ShowDisplayValues:" + (entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx + "-" + showValueEntityIdx);
                if (GameEntry.Entity.HasEntity(entity.Id))
                {
                    if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showValueEntityIdx)
                    {

                        GameEntry.Entity.HideEntity(entity);
                    }
                    else
                    {

                        BattleValueEntities.Add(entity.Entity.Id, entity);
                    }
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
            //Log.Debug("UnShowDisplayValues:" + showValueEntityIdx);
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