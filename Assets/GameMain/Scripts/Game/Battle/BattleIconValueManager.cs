using System.Collections.Generic;
using System.Linq;
using GameFramework;
using JetBrains.Annotations;
using UnityEngine;

namespace RoundHero
{
    public class BattleIconValueManager : Singleton<BattleIconValueManager>
    {
        public Dictionary<int, Entity>  BattleUnitStateIconEntities = new ();
        public int CurUnitStateIconEntityIdx = 0;
        
        private int ShowUnitStateIconEntityIdx = 0;
        
        public async void  ShowDisplayIcon(int actionUnitIdx)
        {
            
            //var actionUnit =  BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);
            
            var triggerDataDict = BattleFightManager.Instance.GetAttackDatas(actionUnitIdx);

            foreach (var kv in triggerDataDict)
            {
                foreach (var triggerData in kv.Value.TriggerDatas)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.State)
                    {
                        continue;
                    }
                    CurUnitStateIconEntityIdx += 1;
                }
                
                foreach (var moveUnitData in kv.Value.MoveData.MoveUnitDatas.Values.ToList())
                {
                    foreach (var kv2 in moveUnitData.MoveActionData.TriggerDataDict)
                    {
                        foreach (var triggerData in kv2.Value.TriggerDatas)
                        {
                            if (triggerData.TriggerDataType != ETriggerDataType.State)
                            {
                                continue;
                            }

                            CurUnitStateIconEntityIdx += 1;
                        }
                    }
                }
               
            }
            
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                ShowIcons(kv.Value.TriggerDatas);

                foreach (var moveUnitData in kv.Value.MoveData.MoveUnitDatas.Values.ToList())
                {
                    foreach (var kv2 in moveUnitData.MoveActionData.TriggerDataDict)
                    {
                        ShowIcons(kv2.Value.TriggerDatas);
                        
                    }
                }
            }
   

        }
        
        public async void ShowIcons(List<TriggerData> triggerDatas)
        {
            var idx = 0;
            foreach (var triggerData in triggerDatas)
            {
                if (triggerData.TriggerDataType != ETriggerDataType.State)
                {
                    continue;
                }
                
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                
                if (effectUnit == null)
                {
                    continue;
                }

                var value = (int)triggerData.ActualValue;
                var unitState = triggerData.UnitStateDetail.UnitState;

                InternalShowIcon(triggerData, unitState, value);
                // GameUtility.DelayExcute(idx *0.25f, () =>
                // {
                //     InternalShowIcon(triggerData, unitState, value, _curUnitStateIconEntityIdx);
                // });
                
                idx++;



            }

            
        }
        
        private async void InternalShowIcon(TriggerData triggerData, EUnitState unitState, int value)
        {

            InternalAnimtionChangeUnitState(unitState, value, triggerData, true);


        }
        
        private void InternalAnimtionChangeUnitState(EUnitState unitState, int value, TriggerData triggerData, bool isLoop)
        {
            
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            

            var moveParams = new MoveParams()
            {
                FollowGO = effectUnit.gameObject,
                DeltaPos = new Vector2(0, 25f),
                IsUIGO = false,
            };

            var targetMoveParams = new MoveParams()
            {
                FollowGO = effectUnit.gameObject,
                DeltaPos = new Vector2(0, -50f),
                IsUIGO = false,
            };
            

            AddUnitStateMoveValue(unitState, value, value,  CurUnitStateIconEntityIdx++,
                isLoop, false,
                moveParams,
                targetMoveParams, triggerData.Idx);

            // var entity = await GameEntry.Entity.ShowBattleUnitStateMoveValueEntityAsync(value, value, unitState, _curUnitStateIconEntityIdx++,
            //     isLoop, false,
            //     moveParams,
            //     targetMoveParams);

            // if (GameEntry.Entity.HasEntity(entity.Id))
            // {
            //     var _entityIdx = entity.BattleMoveValueEntityData.EntityIdx;
            //     if (_entityIdx == -1)
            //     {
            //         BattleUnitStateIconEntities.Add(entity.Entity.Id, entity);
            //     }
            //     else if (_entityIdx < showUnitStateIconEntityIdx)
            //     {
            //     
            //         GameEntry.Entity.HideEntity(entity);
            //     }
            //     else
            //     {
            //         BattleUnitStateIconEntities.Add(entity.Entity.Id, entity);
            //     }
            // }

        }
        
        
        protected List<BattleUnitStateValueEntityData> unitStateIconValueList = new();
        public void AddUnitStateMoveValue(EUnitState unitState, int startValue, int endValue, int entityIdx = -1, bool isLoop = false, bool isAdd = false,
            MoveParams moveParams = null, MoveParams targetMoveParams = null, int triggerDataIdx = -1)
        {
            if (triggerDataIdx != -1)
            {
                for (int i = unitStateIconValueList.Count - 1; i >= 0; i--)
                {
                    if (unitStateIconValueList[i].TriggerDataIdx == triggerDataIdx)
                    {
                        unitStateIconValueList.RemoveAt(i);
                    }
                }
            }

            var data = ReferencePool.Acquire<BattleUnitStateValueEntityData>();
            data.Init(GameEntry.Entity.GenerateSerialId(), startValue, endValue, unitState, entityIdx, isLoop, isAdd, moveParams,
                targetMoveParams, triggerDataIdx);

            unitStateIconValueList.Add(data);
        }
        
        private float showMoveIconValueTime = 0.3f;
        protected async void ShowMoveIconValues()
        {
            if(unitStateIconValueList.Count <= 0)
                return;
            
            showMoveIconValueTime += Time.deltaTime;
            if (showMoveIconValueTime > 0.3f)
            {
                showMoveIconValueTime = 0;

                BattleUnitStateValueEntity entity = null;
                BattleUnitStateValueEntityData data = null;
                do
                {
                    data = null;
                    if (unitStateIconValueList.Count > 0)
                    {
                        data = unitStateIconValueList[0];
                        unitStateIconValueList.RemoveAt(0);
                    }
                    
                    // if (unitStateIconValueQueue.Count <= 0)
                    // {
                    //     showMoveValueIconTime = 0.8f;
                    // }

                } while(data != null && data.EntityIdx < ShowUnitStateIconEntityIdx);
                
                if(data == null)
                    return;
                
                //var data = moveValueQueue.Dequeue();

                entity = await GameEntry.Entity.ShowUnitStateIconValueEntityAsync(data);
                
                
                if (GameEntry.Entity.HasEntity(entity.Id))
                {
                    var entityIdx = entity.BattleUnitStateValueEntityData.EntityIdx;
                    if (entityIdx == -1)
                    {
                    }
                    else if (entityIdx < ShowUnitStateIconEntityIdx)
                    {
                
                        GameEntry.Entity.HideEntity(entity);
                    }
                    else
                    {
                
                        BattleUnitStateIconEntities.Add(entity.Entity.Id, entity);
                    }
                }
            }
        }
        
        public void Update()
        {
            
            if(BattleManager.Instance.BattleState == EBattleState.EndBattle)
                return;

            ShowMoveIconValues();

        }
        
        public void UnShowDisplayIcons()
        {
            
            ShowUnitStateIconEntityIdx = CurUnitStateIconEntityIdx;
            
            foreach (var kv in BattleUnitStateIconEntities)
            {
                if (GameEntry.Entity.HasEntity(kv.Value.Entity.Id))
                {
                    //Log.Debug("ID:" + kv.Value.Entity.Id + kv.Value.Entity.EntityAssetName);
                    GameEntry.Entity.HideEntity(kv.Value);
                }
              
            }
            BattleUnitStateIconEntities.Clear();
            
        }

        
        public void ShowHurtDisplayIcon(int effectUnitIdx, [CanBeNull] List<int> actionUnitIdxs)
        {
            
            //_curUnitStateIconEntityIdx = CurUnitStateIconEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdxs),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdxs));

            foreach (var kv in triggerDataDict)
            {
                foreach (var triggerData in kv.Value)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.State)
                    {
                        continue;
                    }
                    CurUnitStateIconEntityIdx += 1;
                }
               
            }
            
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                ShowIcons(kv.Value);

            }
        }

        public async void ShowTacticHurtDisplayIcons(int effectUnitIdx)
        {

            var triggerDataDict = BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);

            InternalShowHurtDisplayIcon(effectUnitIdx, triggerDataDict);

            
        }
        
        private void InternalShowHurtDisplayIcon(int effectUnitIdx, Dictionary<int, List<TriggerData>> triggerDataDict)
        {
            var entityIdx = CurUnitStateIconEntityIdx;
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            
            foreach (var kv in triggerDataDict)
            {
                foreach (var triggerData in kv.Value)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.State)
                    {
                        continue;
                    }
                    CurUnitStateIconEntityIdx += 1;
                        
                }

            }
                
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {

                ShowIcons(kv.Value);
                idx++;
                entityIdx += kv.Value.Count;
            }
        }

        public void AnimtionChangeUnitState(EUnitState unitState, int value, TriggerData triggerData, int entityIdx, bool isLoop)
        {
            CurUnitStateIconEntityIdx += 1;
            InternalAnimtionChangeUnitState(unitState, value, triggerData, isLoop);
        }

    }
}