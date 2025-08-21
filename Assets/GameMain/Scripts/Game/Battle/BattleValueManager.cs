using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, Entity>  BattleValueEntities = new ();
        private int curValueEntityIdx = 0;
        private int _curValueEntityIdx = 0;
        private int showValueEntityIdx = 0;
        
        public void ShowHurtDisplayValue(int effectUnitIdx, int actionUnitIdx)
        {
            //UnShowDisplayValues();
            ShowHurtDisplayValues(effectUnitIdx, actionUnitIdx);
        }

        private void InternalShowHurtDisplayValue(int effectUnitIdx, Dictionary<int, List<TriggerData>> triggerDataDict)
        {
            _curValueEntityIdx = curValueEntityIdx;
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            
            if (effectUnit is BattleSoliderEntity)
            {
                foreach (var kv in triggerDataDict)
                {
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
                        {
                            continue;
                        }
                        curValueEntityIdx += 1;
                        
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
                    ShowValues(kv.Value, _curValueEntityIdx);
                    idx++;
                    //entityIdx += kv.Value.Count;
                }
            }
            else
            {
                var startValue = 0;
                var endValue = 0;

                foreach (var kv in triggerDataDict)
                {
                    foreach (var triggerData in kv.Value)
                    {
                        startValue += (int)triggerData.ActualValue;
                        endValue += BlessManager.Instance.AddCurHPByAttackDamage()
                            ? (int)(triggerData.Value + triggerData.DeltaValue)
                            : (int)triggerData.ActualValue;
                    }
                }

                if (startValue != 0)
                {
                    curValueEntityIdx += 1;
                    InternalShowValue(effectUnit, startValue, endValue, _curValueEntityIdx);
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
            
            _curValueEntityIdx = curValueEntityIdx;
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
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
                        {
                            continue;
                        }
                        curValueEntityIdx += 1;
                        
                    }
                    
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
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
                        if (triggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
                        {
                            continue;
                        }
                        curValueEntityIdx += 1;
                        
                    }
                    ShowValues(kv.Value, curValueEntityIdx);
                    //entityIdx += kv.Value.Count;
                }
                else
                {
                    curValueEntityIdx += 1;
                    
                    var startValue = 0;
                    var endValue = 0;
                    foreach (var triggerData in kv.Value)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
                        {
                            continue;
                        }
                        
                        startValue += (int)triggerData.ActualValue;
                        endValue += BlessManager.Instance.AddCurHPByAttackDamage()
                            ? (int)(triggerData.Value + triggerData.DeltaValue)
                            : (int)triggerData.ActualValue;
                    }

                    // if (startValue != 0)
                    // {
                    //     
                    // }
                    InternalShowValue(effectUnit, startValue, endValue, _curValueEntityIdx);
                }

            }
   
            
        }

        public async void ShowActionSort(int sort)
        {
            _curValueEntityIdx = curValueEntityIdx;
            curValueEntityIdx += 1;
            
            var effectUnitPos = Root.position;
            
            // effectUnitPos.y += 1f;
            // effectUnitPos.z -= 0.5f;
            
            var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);

            uiLocalPoint.y += 50f;

            var entity = await GameEntry.Entity.ShowBattleValueEntityAsync(
                uiLocalPoint, sort, _curValueEntityIdx++);

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
                DeltaPos = new Vector2(0, 25f),
                IsUIGO = true,
            };

            var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(value, value, _curValueEntityIdx++, true, false,
                moveParams,
                targetMoveParams);

            
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

        private async void InternalShowValue(BattleUnitEntity effectUnit, int startValue, int endValue, int entityIdx)
        {
            
            if (effectUnit == null)
            {
                return;
            }

            
            if (effectUnit is BattleMonsterEntity)
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
                    DeltaPos = new Vector2(0, 75f),
                    IsUIGO = false,
                };

                var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(startValue, endValue, _curValueEntityIdx++,
                    true, effectUnit is BattleSoliderEntity,
                    moveParams,
                    targetMoveParams);

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
                
                var moveParams = new MoveParams()
                {
                    FollowGO = effectUnit.gameObject,
                    DeltaPos = new Vector2(0, 25f),
                    IsUIGO = false,
                };
            
                var targetMoveParams = new MoveParams()
                {
                    FollowGO = startValue < 0 ? AreaController.Instance.UICore :  effectUnit.gameObject,
                    DeltaPos = new Vector2(0, startValue < 0 ? -25f : 75f),
                    IsUIGO = startValue < 0,
                };

                var entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(startValue, endValue, _curValueEntityIdx++, true,
                    effectUnit is BattleSoliderEntity && startValue < 0,
                    moveParams,
                    targetMoveParams);

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
                if (triggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
                {
                    continue;
                }
                
                var startvalue = (int)triggerData.ActualValue;
                var endValue = BlessManager.Instance.AddCurHPByAttackDamage()
                    ? (int)(triggerData.Value + triggerData.DeltaValue)
                    : (int)triggerData.ActualValue;
                // if(value == 0)
                //     continue;

                // GameUtility.DelayExcute(idx * 0.25f, () =>
                // {
                //     
                // });
                InternalShowValue(effectUnit, startvalue, endValue, _curValueEntityIdx);
                //InternalShowValue(effectUnit, value, entityIdx++);

                idx++;



            }

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