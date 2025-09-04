using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, Entity>  BattleUnitStateIconEntities = new ();
        private int curUnitStateIconEntityIdx = 0;
        private int _curUnitStateIconEntityIdx = 0;
        private int showUnitStateIconEntityIdx = 0;
        
        
        public void ShowHurtDisplayIcon(int effectUnitIdx, int actionUnitIdx)
        {
            
            _curUnitStateIconEntityIdx = curUnitStateIconEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdx),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdx));

            foreach (var kv in triggerDataDict)
            {
                foreach (var triggerData in kv.Value)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.RoleState)
                    {
                        continue;
                    }
                    curUnitStateIconEntityIdx += 1;
                }
               
            }
            
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                ShowIcons(kv.Value, _curUnitStateIconEntityIdx);

            }
        }
        

        public async void ShowDisplayIcon(int actionUnitIdx)
        {
            
            //var actionUnit =  BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);
            
            _curUnitStateIconEntityIdx = curUnitStateIconEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(actionUnitIdx),
                BattleFightManager.Instance.GetInDirectAttackDatas(actionUnitIdx));

            foreach (var kv in triggerDataDict)
            {
                foreach (var triggerData in kv.Value)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.RoleState)
                    {
                        continue;
                    }
                    curUnitStateIconEntityIdx += 1;
                }
               
            }
            
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {
                ShowIcons(kv.Value, _curUnitStateIconEntityIdx);

            }
   

        }
        private async void ShowIcons(List<TriggerData> triggerDatas, int entityIdx)
        {
            var idx = 0;
            foreach (var triggerData in triggerDatas)
            {
                if (triggerData.TriggerDataType != ETriggerDataType.RoleState)
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

                GameUtility.DelayExcute(idx *0.25f, () =>
                {
                    InternalShowIcon(triggerData, unitState, value, _curUnitStateIconEntityIdx);
                });
                //InternalShowValue(effectUnit, value, entityIdx++);

                idx++;



            }

            
        }
        private async void InternalShowIcon(TriggerData triggerData, EUnitState unitState, int value, int entityIdx)
        {

            InternalAnimtionChangeUnitState(unitState, value, triggerData, entityIdx, true);


        }

        public void UnShowDisplayIcons()
        {
            
            showUnitStateIconEntityIdx = curUnitStateIconEntityIdx;
            
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

        public async Task AnimtionChangeUnitState(EUnitState unitState, int value, TriggerData triggerData, int entityIdx, bool isLoop)
        {
            _curUnitStateIconEntityIdx = curUnitStateIconEntityIdx;
            curUnitStateIconEntityIdx += 1;
            await InternalAnimtionChangeUnitState(unitState, value, triggerData, entityIdx, isLoop);
        }

        private async Task InternalAnimtionChangeUnitState(EUnitState unitState, int value, TriggerData triggerData, int entityIdx, bool isLoop)
        {
            
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);


            var moveParams = new MoveParams();
            
            if (actionUnit != null)
            {
                moveParams.FollowGO = actionUnit.gameObject;
                moveParams.DeltaPos = new Vector2(0, 25f);
                moveParams.IsUIGO = false;
            }
            else if (triggerData.TriggerDataSubType == ETriggerDataSubType.Card)
            {
                var cardEntity = BattleCardManager.Instance.GetCardEntity(triggerData.TriggerCardIdx);

                if (cardEntity != null)
                {
                    moveParams.FollowGO = cardEntity.gameObject;
                    moveParams.DeltaPos = new Vector2(0, 25f);
                    moveParams.IsUIGO = true;
                }
                else
                {
                    moveParams.FollowGO = BattleController.Instance.HandCardPos.gameObject;
                    moveParams.DeltaPos = new Vector2(0, 25f);
                    moveParams.IsUIGO = true;
                }
                
            }
            
            var targetMoveParams = new MoveParams()
            {
                FollowGO = effectUnit.gameObject,
                DeltaPos = (actionUnit != null && actionUnit.UnitIdx == effectUnit.UnitIdx) ? new Vector2(0, 100f) : new Vector2(0, 25f),
                IsUIGO = false,
            };

            AddMoveValue(unitState, value, value,  CurValueEntityIdx++,
                isLoop, false,
                moveParams,
                targetMoveParams);

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
        
        public async void ShowTacticHurtDisplayIcons(int effectUnitIdx)
        {

            var triggerDataDict = BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);

            InternalShowHurtDisplayIcon(effectUnitIdx, triggerDataDict);

            
        }
        
        private void InternalShowHurtDisplayIcon(int effectUnitIdx, Dictionary<int, List<TriggerData>> triggerDataDict)
        {
            var entityIdx = CurValueEntityIdx;
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            
            foreach (var kv in triggerDataDict)
            {
                foreach (var triggerData in kv.Value)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.RoleState)
                    {
                        continue;
                    }
                    curUnitStateIconEntityIdx += 1;
                        
                }

            }
                
            var idx = 0;
            foreach (var kv in triggerDataDict)
            {

                ShowIcons(kv.Value, entityIdx);
                idx++;
                entityIdx += kv.Value.Count;
            }
        }
        
    }

}