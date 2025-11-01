using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, Entity>  BattleValueEntities = new ();
        public int CurValueEntityIdx = 0;
        private int _curValueEntityIdx = 0;
        public int ShowValueEntityIdx = 0;
        
        public void ShowHurtDisplayValue(int effectUnitIdx, int actionUnitIdx)
        {
            //UnShowDisplayValues();
            ShowHurtDisplayValues(effectUnitIdx, actionUnitIdx);
        }

        private void InternalShowHurtDisplayValue(int effectUnitIdx, Dictionary<int, List<TriggerData>> triggerDataDict)
        {
            _curValueEntityIdx = CurValueEntityIdx;
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            
            if (effectUnit is BattleSoliderEntity)
            {
                foreach (var kv in triggerDataDict)
                {
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                        {
                            continue;
                        }
                        //CurValueEntityIdx += 1;
                        
                    }

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
                    ShowValues(kv.Value);
                    idx++;
                    //entityIdx += kv.Value.Count;
                }
            }
            else
            {
                var startValue = 0;
                var endValue = 0;
                var isShow = false;
                
                var idx = 0;
                foreach (var kv in triggerDataDict)
                {
                    ShowValues(kv.Value);
                    idx++;
                    
                    // foreach (var triggerData in kv.Value)
                    // {
                    //     if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                    //     {
                    //         continue;
                    //     }
                    //     isShow = true;
                    //     startValue += (int)triggerData.ActualValue;
                    //     endValue += BlessManager.Instance.AddCurHPByAttackDamage()
                    //         ? (int)(triggerData.Value + triggerData.DeltaValue)
                    //         : (int)triggerData.ActualValue;
                    // }
                }

                // if (isShow)
                // {
                //     InternalShowValue(effectUnit, startValue, endValue, _curValueEntityIdx);
                // }
               
            }
        }
        
        public async void ShowTacticHurtDisplayValues(int effectUnitIdx)
        {

            var triggerDataDict = BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);

            InternalShowHurtDisplayValue(effectUnitIdx, triggerDataDict);

            
        }
        
        public async Task ShowTacticHurtAttackTag(int effectUnitIdx, int actionUnitIdx, List<int> exceptUnitIdxs = null)
        {

            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }

            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
        
            if (effectUnit == null)
                return;
        
            var triggerDataDict =
                BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);
        
            if (triggerDataDict.Values.Count <= 0)
            {
                return;
            }
        
            var values = triggerDataDict.Values.ToList();
            if (values[0].Count <= 0)
            {
                return;
            }
            
            var entityIdx = curAttackTagEntityIdx;
            
            
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        continue;
                    
                    if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
                        continue;
                    
                    if(exceptUnitIdxs != null && exceptUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    // if(actionUnit == null)
                    //     continue;
        
                    curAttackTagEntityIdx++;
                }
        
            }
            
            var effectGridPosIdxs = new List<int>();
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        continue;
                    
                    if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
                        continue;
                    
                    if(exceptUnitIdxs != null && exceptUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    // if(actionUnit == null)
                    //     continue;
                    
                    if(triggerData.ActionUnitGridPosIdx == -1)
                        continue;
                    
                    var _entityIdx = entityIdx;
                    var battleAttackTagEntity = await BattleStaticAttackTagManager.Instance.ShowTag(
                        triggerData.ActionUnitGridPosIdx, effectUnit.GridPosIdx, triggerData, _entityIdx,
                        triggerData.ActionUnitIdx == Constant.Battle.UnUnitTriggerIdx,
                        !effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx),
                        triggerData.TriggerDataSubType == ETriggerDataSubType.Collision, false);
                    effectGridPosIdxs.Add(triggerData.EffectUnitGridPosIdx);
                    entityIdx++;
                    
                    if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
                    {
                        //Log.Debug("Tag hide");
                        GameEntry.Entity.HideEntity(battleAttackTagEntity);
                    }
                    else
                    {
                
                        BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
                        //Log.Debug("Tag add:" + BattleUnitData.Idx + "-" + BattleAttackTagEntities.Count); 
                    }
                    
                    //InternalShowTag(actionUnit.BattleUnitData, triggerData.BuffValue);
                }
        
            }
        
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
            
            _curValueEntityIdx = CurValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(actionUnitIdx),
                BattleFightManager.Instance.GetInDirectAttackDatas(actionUnitIdx));
            //curValueEntityIdx += triggerDataDict.Count;

            // foreach (var kv in triggerDataDict)
            // {
            //     curValueEntityIdx += 1;
            // }
            
            
            foreach (var kv in triggerDataDict)
            {

                var effectUnit =  BattleUnitManager.Instance.GetUnitByIdx(kv.Key);

                if (kv.Key == PlayerManager.Instance.PlayerData.BattleHero.Idx)
                {
                    // foreach (var triggerData in kv.Value)
                    // {
                    //     if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                    //         triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                    //     {
                    //         continue;
                    //     }
                    //     //CurValueEntityIdx += 1;
                    //     
                    // }
                    
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                            triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                        {
                            continue;
                        }
                        ShowHeroValue(triggerData.ActionUnitGridPosIdx, (int)triggerData.ActualValue, _curValueEntityIdx);
                    }

                }
                else if (effectUnit is BattleSoliderEntity)
                {
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                            triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                        {
                            continue;
                        }
                        //CurValueEntityIdx += 1;
                        
                    }
                    ShowValues(kv.Value);
                    //entityIdx += kv.Value.Count;
                }
                else
                {
                    //CurValueEntityIdx += 1;
                    
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                            triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                        {
                            continue;
                        }
                        //CurValueEntityIdx += 1;
                        
                    }
                    ShowValues(kv.Value);
                    
                    // var startValue = 0;
                    // var endValue = 0;
                    // var isShow = false;
                    // foreach (var triggerData in kv.Value)
                    // {
                    //     if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                    //         triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                    //     {
                    //         continue;
                    //     }
                    //     isShow = true;
                    //     startValue += (int)triggerData.ActualValue;
                    //     endValue += BlessManager.Instance.AddCurHPByAttackDamage()
                    //         ? (int)(triggerData.Value + triggerData.DeltaValue)
                    //         : (int)triggerData.ActualValue;
                    // }
                    //
                    //
                    // if (isShow)
                    // {
                    //     InternalShowValue(effectUnit, startValue, endValue, _curValueEntityIdx);
                    // }
                    
                }

            }
   
            
        }

        public async void ShowActionSort(int sort)
        {
            _curValueEntityIdx = CurValueEntityIdx;
            //CurValueEntityIdx += 1;
            
            var effectUnitPos = Root.position;
            
            // effectUnitPos.y += 1f;
            // effectUnitPos.z -= 0.5f;
            
            var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);

            uiLocalPoint.y += 50f;

            var entity = await GameEntry.Entity.ShowBattleValueEntityAsync(
                uiLocalPoint, sort, _curValueEntityIdx++);

            if ((entity as BattleValueEntity).BattleValueEntityData.EntityIdx <
                ShowValueEntityIdx)
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

            var gridEntity = BattleAreaManager.Instance.GetGridEntityByGridPosIdx(gridPosIdx);
            var moveParams = new MoveParams()
            {
                FollowGO = gridEntity.gameObject,
                DeltaPos = new Vector2(0, 1f),
                IsUIGO = false,
            };
            
            var targetMoveParams = new MoveParams()
            {
                FollowGO = AreaController.Instance.UICore,
                DeltaPos = new Vector2(0, -25f),
                IsUIGO = true,
            };
            
            AddMoveValue(value, value, CurValueEntityIdx++, true,
                false, moveParams, targetMoveParams);

            // var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(value, value, showValueIdx++, _curValueEntityIdx++, true, false,
            //     moveParams,
            //     targetMoveParams);

            
            //Log.Debug("2ShowDisplayValues:" + (entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx + "-" + showValueEntityIdx);
            // if (GameEntry.Entity.HasEntity(entity.Id))
            // {
            //     if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < ShowValueEntityIdx)
            //     {
            //
            //         GameEntry.Entity.HideEntity(entity);
            //     }
            //     else
            //     {
            //
            //         BattleValueEntities.Add(entity.Entity.Id, entity);
            //     }
            // }
        }

        private async void InternalShowValue(BattleUnitEntity effectUnit, int startValue, int endValue, int entityIdx, bool isAdd)
        {
            
            if (effectUnit == null)
            {
                return;
            }

            //effectUnit is BattleMonsterEntity
            if (isAdd || effectUnit is BattleCoreEntity)
            {
 
                var moveParams = new MoveParams()
                {
                    FollowGO = effectUnit.gameObject,
                    DeltaPos = new Vector2(0, 25f),
                    IsUIGO = false,
                };
            
                var targetMoveParams = new MoveParams()
                {
                    FollowGO = startValue < 0 ? AreaController.Instance.UICore :  effectUnit.gameObject,
                    DeltaPos = new Vector2(0, startValue < 0 ? -25f : 100f),
                    IsUIGO = startValue < 0,
                };
                
                AddMoveValue(startValue, endValue, CurValueEntityIdx++, true,
                    isAdd,
                    moveParams,
                    targetMoveParams);
                
                

                // var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(startValue, endValue, _curValueEntityIdx++,
                //     true, effectUnit is BattleSoliderEntity,
                //     moveParams,
                //     targetMoveParams);
                //
                // if (GameEntry.Entity.HasEntity(entity.Id))
                // {
                //     if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < ShowValueEntityIdx)
                //     {
                //
                //         GameEntry.Entity.HideEntity(entity);
                //     }
                //     else
                //     {
                //
                //         BattleValueEntities.Add(entity.Entity.Id, entity);
                //     }
                // }

            }
            else
            {
                var moveParams = new MoveParams()
                {
                    FollowGO = effectUnit.gameObject,
                    DeltaPos = new Vector2(0, 25f),
                    IsUIGO = false,
                };
            
                var targetMoveParams = new MoveParams()
                {
                    FollowGO = effectUnit.gameObject,
                    DeltaPos = new Vector2(0, 100f),
                    IsUIGO = false,
                };
                
                AddMoveValue(startValue, endValue, CurValueEntityIdx++,
                    true, isAdd,
                    moveParams,
                    targetMoveParams);
                

                // var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(startValue, endValue, _curValueEntityIdx++, true,
                //     effectUnit is BattleSoliderEntity && startValue < 0,
                //     moveParams,
                //     targetMoveParams);
                //
                // if (GameEntry.Entity.HasEntity(entity.Id))
                // {
                //     if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < ShowValueEntityIdx)
                //     {
                //
                //         GameEntry.Entity.HideEntity(entity);
                //     }
                //     else
                //     {
                //
                //         BattleValueEntities.Add(entity.Entity.Id, entity);
                //     }
                // }

            }
        }

        //, _curValueEntityIdx
        private async void ShowValues(List<TriggerData> triggerDatas)
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
                if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                {
                    continue;
                }
                
                var startvalue = (int)triggerData.ActualValue;
                var endValue = BlessManager.Instance.AddCurHPByAttackDamage()
                    ? (int)(triggerData.Value + triggerData.DeltaValue)
                    : (int)triggerData.ActualValue;

                // if (triggerData.HeroHPDelta)
                // {
                //     endValue = Mathf.Abs(endValue);
                // }
                // if(value == 0)
                //     continue;

                // GameUtility.DelayExcute(idx * 0.25f, () =>
                // {
                //     
                // });
                InternalShowValue(effectUnit, startvalue, endValue, _curValueEntityIdx, triggerData.HeroHPDelta);
                //InternalShowValue(effectUnit, value, entityIdx++);

                idx++;



            }

        }
        
        public void  UnShowDisplayValues()
        {
            
            ShowValueEntityIdx = CurValueEntityIdx;
            //Log.Debug("UnShowDisplayValues:" + showValueEntityIdx);
            foreach (var kv in BattleValueEntities)
            {
                if (GameEntry.Entity.HasEntity(kv.Value.Entity.Id))
                {
                    if (kv.Value is BattleMoveValueEntity battleMoveValueEntity)
                    {
                        battleMoveValueEntity.HideEntity();
                    }
                    else
                    {
                        GameEntry.Entity.HideEntity(kv.Value);
                    }

                }
              
            }
            BattleValueEntities.Clear();
            
        }
    }

}