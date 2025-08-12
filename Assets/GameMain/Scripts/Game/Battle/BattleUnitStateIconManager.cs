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
                    if (triggerData.BuffValue.BuffData.BuffValueType != EBuffValueType.State)
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
                    InternalShowIcon(actionUnit, effectUnit, unitState, value, _curUnitStateIconEntityIdx);
                });
                //InternalShowValue(effectUnit, value, entityIdx++);

                idx++;



            }

            
        }
        private async void InternalShowIcon(BattleUnitEntity actionUnit, BattleUnitEntity effectUnit, EUnitState unitState, int value, int entityIdx)
        {
            
            if (effectUnit == null)
            {
                return;
            }

            InternalAnimtionChangeUnitState(unitState, value, actionUnit, effectUnit, entityIdx, true);


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

        public async Task AnimtionChangeUnitState(EUnitState unitState, int value, BattleUnitEntity actionUnit,
            BattleUnitEntity effectUnit, int entityIdx, bool isLoop)
        {
            _curUnitStateIconEntityIdx = curUnitStateIconEntityIdx;
            curUnitStateIconEntityIdx += 1;
            await InternalAnimtionChangeUnitState(unitState, value, actionUnit, effectUnit, entityIdx, isLoop);
        }

        private async Task InternalAnimtionChangeUnitState(EUnitState unitState, int value, BattleUnitEntity actionUnit, BattleUnitEntity effectUnit, int entityIdx, bool isLoop)
        {

            var moveParams = new MoveParams()
            {
                
                FollowGO = actionUnit.gameObject,
                DeltaPos = new Vector2(0, 25f),
                IsUIGO = false,
            };
            
            var targetMoveParams = new MoveParams()
            {
                FollowGO = effectUnit.gameObject,
                DeltaPos = (actionUnit != null && actionUnit.UnitIdx == effectUnit.UnitIdx) ? new Vector2(0, 125f) : new Vector2(0, 25f),
                IsUIGO = false,
            };
            
            var entity = await GameEntry.Entity.ShowBattleMoveIconEntityAsync(unitState, value, _curUnitStateIconEntityIdx++, isLoop, moveParams, targetMoveParams);

            if (GameEntry.Entity.HasEntity(entity.Id))
            {
                var _entityIdx = entity.BattleMoveIconEntityData.EntityIdx;
                if (_entityIdx == -1)
                {
                    BattleUnitStateIconEntities.Add(entity.Entity.Id, entity);
                }
                if (_entityIdx < showUnitStateIconEntityIdx)
                {
                
                    GameEntry.Entity.HideEntity(entity);
                }
                else
                {
                    BattleUnitStateIconEntities.Add(entity.Entity.Id, entity);
                }
            }

        }
        
        public async void ShowTacticHurtDisplayIcons(int effectUnitIdx)
        {

            var triggerDataDict = BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);

            //InternalShowIcon(effectUnitIdx, triggerDataDict);

            
        }
    }

}