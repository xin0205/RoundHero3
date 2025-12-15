using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework;
using JetBrains.Annotations;
using UnityEngine;

namespace RoundHero
{
    public class BattleValueManager : Singleton<BattleValueManager>
    {
        public Dictionary<int, Entity>  BattleValueEntities = new ();
        public int CurValueEntityIdx = 0;
        //private int _curValueEntityIdx = 0;
        public int ShowValueEntityIdx = 0;

        public void ShowDisplayValue(int actionUnitIdx)
        {
            //BattleValueEntities.Clear();
            var actionUnit =  BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);

            var triggerDataDict = BattleFightManager.Instance.GetAttackDatas(actionUnitIdx);

            
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
                    
                    foreach (var triggerData in kv.Value.TriggerDatas)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                            triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                        {
                            continue;
                        }
                        ShowHeroValue(triggerData.ActionUnitGridPosIdx, (int)triggerData.ActualValue, triggerData);
                    }

                }
                else if (effectUnit is BattleSoliderEntity)
                {
                    // foreach (var triggerData in kv.Value.TriggerDatas)
                    // {
                    //     if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                    //         triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                    //     {
                    //         continue;
                    //     }
                    //     //CurValueEntityIdx += 1;
                    //     
                    // }
                    ShowValues(kv.Value.TriggerDatas);
                    //entityIdx += kv.Value.Count;
                }
                else
                {
                    //CurValueEntityIdx += 1;
                    
                    // foreach (var triggerData in kv.Value.TriggerDatas)
                    // {
                    //     if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                    //         triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
                    //     {
                    //         continue;
                    //     }
                    //     //CurValueEntityIdx += 1;
                    //     
                    // }
                    ShowValues(kv.Value.TriggerDatas);
                    
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

        public async void ShowActionSort(int unitIdx, int sort)
        {
            //_curValueEntityIdx = CurValueEntityIdx;
            //CurValueEntityIdx += 1;
            var unit = BattleUnitManager.Instance.GetUnitByIdx(unitIdx);
            if(unit == null)
                return;
            
            var unitPos = unit.Root.position;
            
            // effectUnitPos.y += 1f;
            // effectUnitPos.z -= 0.5f;
            
            var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), unitPos);
        
            uiLocalPoint.y += 50f;
        
            var entity = await GameEntry.Entity.ShowBattleValueEntityAsync(
                uiLocalPoint, sort, CurValueEntityIdx++);
        
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
        
        private async Task ShowHeroValue(int gridPosIdx, int value, TriggerData triggerData)
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
                false, moveParams, targetMoveParams, triggerData.Idx);
        

        }
        //
        private async void InternalShowValue(BattleUnitEntity unit, int startValue, int endValue, bool isAdd, TriggerData triggerData)
        {
            
            if (unit == null)
            {
                return;
            }
        
            //effectUnit is BattleMonsterEntity
            if (isAdd || unit is BattleCoreEntity)
            {
        
                var moveParams = new MoveParams()
                {
                    FollowGO = unit.gameObject,
                    DeltaPos = new Vector2(0, 25f),
                    IsUIGO = false,
                };
            
                var targetMoveParams = new MoveParams()
                {
                    FollowGO = startValue < 0 ? AreaController.Instance.UICore :  unit.gameObject,
                    DeltaPos = new Vector2(0, startValue < 0 ? -25f : 100f),
                    IsUIGO = startValue < 0,
                };
                
                AddMoveValue(startValue, endValue, CurValueEntityIdx++, true,
                    isAdd,
                    moveParams,
                    targetMoveParams, triggerData.Idx);
        
            }
            else
            {
                var moveParams = new MoveParams()
                {
                    FollowGO = unit.gameObject,
                    DeltaPos = new Vector2(0, 25f),
                    IsUIGO = false,
                };
            
                var targetMoveParams = new MoveParams()
                {
                    FollowGO = unit.gameObject,
                    DeltaPos = new Vector2(0, 100f),
                    IsUIGO = false,
                };
                
                AddMoveValue(startValue, endValue, CurValueEntityIdx++,
                    true, isAdd,
                    moveParams,
                    targetMoveParams, triggerData.Idx);
                
        
            }
        }

        public async void ShowValues(List<TriggerData> triggerDatas)
        {
            if(triggerDatas.Count <= 0)
                return;
            
            // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].ActionUnitIdx);
            // var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].EffectUnitIdx);
            //
            // if (effectUnit == null)
            // {
            //     return;
            // }
        
            var idx = 0;
            foreach (var triggerData in triggerDatas)
            {
                if (triggerData.TriggerDataType != ETriggerDataType.Atrb &&
                    triggerData.TriggerDataType != ETriggerDataType.HeroAtrb)
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
                
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                
                if (triggerData.EffectUnitIdx == BattlePlayerManager.Instance.PlayerData.BattleHero.Idx)
                {
                    var attackUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    var moveParams = new MoveParams()
                    {
                        FollowGO = triggerData.ActionUnitIdx == Constant.Battle.UnUnitTriggerIdx ? BattleController.Instance.HandCardPos.gameObject :  attackUnit?.gameObject,
                        DeltaPos = new Vector2(0, 25f),
                        IsUIGO = triggerData.ActionUnitIdx == Constant.Battle.UnUnitTriggerIdx,
                    };
            
                    var targetMoveParams = new MoveParams()
                    {
                        FollowGO = AreaController.Instance.UICore,
                        DeltaPos = new Vector2(0, -25f),
                        IsUIGO = true,
                    };
                
                    AddMoveValue(startvalue, endValue, CurValueEntityIdx++, true,
                        triggerData.CoreHPDelta > 0,
                        moveParams,
                        targetMoveParams, triggerData.Idx);
                }
                else if (effectUnit != null)
                {
                    InternalShowValue(effectUnit, startvalue, endValue, triggerData.CoreHPDelta > 0, triggerData);
        
                }
                //InternalShowValue(effectUnit, value, entityIdx++);
        
                idx++;
        
        
        
            }
        
        }
        //
        public void  UnShowDisplayValues()
        {
            moveValueList.Clear();
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
        
        protected List<BattleMoveValueEntityData> moveValueList = new();
        public void AddMoveValue(int startValue, int endValue, int entityIdx = -1, bool isLoop = false, bool isAdd = false,
            MoveParams moveParams = null, MoveParams targetMoveParams = null, int triggerDataIdx = -1)
        {
            
            if (triggerDataIdx != -1)
            {
                for (int i = moveValueList.Count - 1; i >= 0; i--)
                {
                    if (moveValueList[i].TriggerDataIdx == triggerDataIdx)
                    {
                        moveValueList.RemoveAt(i);
                    }
                }
            }
            
            
            var data = ReferencePool.Acquire<BattleMoveValueEntityData>();
            data.Init(GameEntry.Entity.GenerateSerialId(), startValue, endValue, entityIdx, isLoop,
                isAdd, moveParams, targetMoveParams, triggerDataIdx);

            moveValueList.Add(data);
        }

        
        private float showMoveValueTime = 0.4f;
        protected async void ShowMoveValues()
        {
            if(moveValueList.Count <= 0)
                return;
            
            showMoveValueTime += Time.deltaTime;
            if (showMoveValueTime > 0.4f)
            {
                showMoveValueTime = 0;
                
                BattleMoveValueEntity entity = null;

                BattleMoveValueEntityData data = null;
                do
                {
                    data = null;
                    if (moveValueList.Count > 0)
                    {
                        data = moveValueList[0];
                        moveValueList.RemoveAt(0);
                    }
                    // if (moveValueQueue.Count <= 0)
                    // {
                    //     showMoveValueTime = 0.8f;
                    // }
                } while(data != null && data.EntityIdx < ShowValueEntityIdx);
                
                if(data == null)
                    return;
                
                //var data = moveValueQueue.Dequeue();

                entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(data);
                
                // if (data is UnitStateIconValueEntityData unitStateIconValueEntityData)
                // {
                //     entity = await GameEntry.Entity.ShowBattleUnitStateMoveValueEntityAsync(unitStateIconValueEntityData);
                // }
                // else 
                
                if (GameEntry.Entity.HasEntity(entity.Id))
                {
                    var entityIdx = entity.BattleMoveValueEntityData.EntityIdx;
                    if (entityIdx == -1)
                    {
                    }
                    else if (entityIdx < ShowValueEntityIdx)
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
        
        public void Update()
        {
            
            if(BattleManager.Instance.BattleState == EBattleState.EndBattle)
                return;

            ShowMoveValues();

        }
        
        public void ShowHurtDisplayValue(int effectUnitIdx, [CanBeNull] List<int> actionUnitIdxs)
        {
            //UnShowDisplayValues();
            ShowHurtDisplayValues(effectUnitIdx, actionUnitIdxs);
        }
        
        public async void ShowHurtDisplayValues(int effectUnitIdx, [CanBeNull] List<int> actionUnitIdxs)
        {

            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdxs),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdxs));


            InternalShowHurtDisplayValue(effectUnitIdx, triggerDataDict);
            

            

        }
        
        private void InternalShowHurtDisplayValue(int effectUnitIdx, Dictionary<int, List<TriggerData>> triggerDataDict)
        {
            //_curValueEntityIdx = CurValueEntityIdx;
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            
            if (effectUnit is BattleSoliderEntity)
            {
                // foreach (var kv in triggerDataDict)
                // {
                //     foreach (var triggerData in kv.Value)
                //     {
                //         if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                //         {
                //             continue;
                //         }
                //         //CurValueEntityIdx += 1;
                //         
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
        
        
    }
}