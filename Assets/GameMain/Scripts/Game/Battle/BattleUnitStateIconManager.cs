using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, Entity>  BattleUnitStateIconEntities = new ();
        private int curUnitStateIconEntityIdx = 0;
        private int showUnitStateIconEntityIdx = 0;

        
        public void ShowHurtDisplayIcon(int effectUnitIdx, int actionUnitIdx)
        {
            
            var entityIdx = curUnitStateIconEntityIdx;
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
                ShowIcons(kv.Value, entityIdx++);

            }
        }
        

        public async void ShowDisplayIcon(int actionUnitIdx)
        {
            
            //var actionUnit =  BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);
            
            var entityIdx = curUnitStateIconEntityIdx;
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
                ShowIcons(kv.Value, entityIdx++);

            }
   

        }
        private async void ShowIcons(List<TriggerData> triggerDatas, int entityIdx)
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
                if (triggerData.TriggerDataType != ETriggerDataType.RoleState)
                {
                    return;
                }

                var value = (int)triggerData.ActualValue;
                var unitState = triggerData.UnitStateDetail.UnitState;

                GameUtility.DelayExcute(idx *0.25f, () =>
                {
                    InternalShowIcon(effectUnit, unitState, value, entityIdx++);
                });
                //InternalShowValue(effectUnit, value, entityIdx++);

                idx++;



            }

            
        }
        private async void InternalShowIcon(BattleUnitEntity effectUnit, EUnitState unitState, int value, int entityIdx)
        {
            
            if (effectUnit == null)
            {
                return;
            }

            AnimtionChangeUnitState(unitState, value, effectUnit, entityIdx, true);


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
        
        public async Task AnimtionChangeUnitState(EUnitState unitState, int value, BattleUnitEntity unitEntity, int entityIdx, bool isLoop)
        {

            var moveParams = new MoveParams()
            {
                
                FollowGO = unitEntity.gameObject,
                DeltaPos = new Vector2(0, 25f),
                IsUIGO = false,
            };
            
            var targetMoveParams = new MoveParams()
            {
                FollowGO = unitEntity.gameObject,
                DeltaPos = new Vector2(0, 125f),
                IsUIGO = false,
            };
            
            var entity = await GameEntry.Entity.ShowBattleMoveIconEntityAsync(unitState, value, entityIdx, isLoop, moveParams, targetMoveParams);

            if (GameEntry.Entity.HasEntity(entity.Id))
            {
                var _entityIdx = entity.BattleMoveIconEntityData.EntityIdx;
                if (_entityIdx == -1)
                {
                
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
        
        // public async Task AnimationAddUnitState(EUnitState unitState, int value, BattleUnitEntity unitEntity, int entityIdx, bool isLoop)
        // {
        //
        //     var moveParams = new MoveParams()
        //     {
        //         
        //         FollowGO = unitEntity.gameObject,
        //         DeltaPos = new Vector2(0, 125f),
        //         IsUIGO = false,
        //     };
        //     
        //     var targetMoveParams = new MoveParams()
        //     {
        //         FollowGO = unitEntity.gameObject,
        //         DeltaPos = new Vector2(0, 25f),
        //         IsUIGO = false,
        //     };
        //     
        //     var entity = await GameEntry.Entity.ShowBattleMoveIconEntityAsync(unitState, value, entityIdx, isLoop, moveParams, targetMoveParams);
        //     
        //     if (GameEntry.Entity.HasEntity(entity.Id))
        //     {
        //         if ((entity as BattleMoveIconEntity).BattleMoveIconEntityData.EntityIdx < showUnitStateIconEntityIdx)
        //         {
        //         
        //             GameEntry.Entity.HideEntity(entity);
        //         }
        //         else
        //         {
        //         
        //             BattleUnitStateIconEntities.Add(entity.Entity.Id, entity);
        //         }
        //     }
        //
        // }
        //
        public async void ShowTacticHurtDisplayIcons(int effectUnitIdx)
        {

            var triggerDataDict = BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);

            //InternalShowIcon(effectUnitIdx, triggerDataDict);

            
        }
    }

}