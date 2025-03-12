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
        //private bool isShowDisplayValue = false;
        private int curValueEntityIdx = 0;
        private int showValueEntityIdx = 0;
        
        public Random Random;
        private int randomSeed;

        // public void Init(int randomSeed)
        // {
        //     this.randomSeed = randomSeed;
        //     Random = new System.Random(this.randomSeed);
        //     curValueEntityIdx = 0;
        //     showValueEntityIdx = 0;
        //
        // }
        
        public void ShowDisplayValue(int unitIdx)
        {
            UnShowDisplayValues();
            ShowDisplayValues(unitIdx);
        }
        
        public void ShowHurtDisplayValue(int unitIdx)
        {
            UnShowDisplayValues();
            ShowHurtDisplayValues(unitIdx);
        }
        
        public async void ShowHurtDisplayValues(int unitIdx)
        {
            //isShowDisplayValue = true;
            
            BattleValueEntities.Clear();
            
            var entityIdx = curValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(unitIdx),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(unitIdx));
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
            //isShowDisplayValue = true;
            
            BattleValueEntities.Clear();
            
            var entityIdx = curValueEntityIdx;
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unitIdx),
                BattleFightManager.Instance.GetInDirectAttackDatas(unitIdx));
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
                value += (int)triggerData.ActualValue;// (int)(triggerData.Value + triggerData.DeltaValue);
            }
            
            
            Entity entity;
            if (effectUnit is BattleSoliderEntity solider)
            {
                //var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnit.UnitCamp);
                
                var effectUnitPos = effectUnit.Root.position;
                
                // var effectUnitFlyEndGridPosIdx =
                //     BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);
                //
                // if (effectUnitFlyEndGridPosIdx != -1)
                // {
                //     effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyEndGridPosIdx);
                // }

                //var targetPos = heroEntity.Root.position;
                
                // var heroMovePaths = BattleFightManager.Instance.GetMovePaths(heroEntity.UnitIdx, actionUnit.UnitIdx);
                // if (heroMovePaths != null && heroMovePaths.Count > 0)
                // {
                //     targetPos = GameUtility.GridPosIdxToPos(heroMovePaths[heroMovePaths.Count - 1]);
                // }

                effectUnitPos.y += 1f;
                //targetPos.y += 1f;
                
                // var pos = AreaController.Instance.UICamera.WorldToScreenPoint(AreaController.Instance.UICore.transform.position);
                // var uiCorePos = AreaController.Instance.UICamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.transform.position.z));
                //

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
                // var effectUnitFlyGridPosIdx =
                //     BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);
                //
                // if (effectUnitFlyGridPosIdx != -1)
                // {
                //     effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyGridPosIdx);
                // }
                
                effectUnitPos.y += 1f;
                effectUnitPos.z -= 0.3f;
                
                entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(
                    effectUnitPos, value, entityIdx);

                //entity.transform.parent = effectUnit.Root;
                        
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
                    GameEntry.Entity.HideEntity(kv.Value.Entity);
                }
              
            }
            BattleValueEntities.Clear();
            
        }
    }

    public class BattleValueManager : Singleton<BattleValueManager>
    {
        public void A(int effectUnitIdx)
        {
            
        }
    }
    
    // public class BattleValueManager: Singleton<BattleValueManager>
    // {
    //     public Dictionary<int, Entity>  BattleValueEntities = new ();
    //     //private bool isShowDisplayValue = false;
    //     private int curEntityIdx = 0;
    //     private int showEntityIdx = 0;
    //     
    //     public Random Random;
    //     private int randomSeed;
    //
    //     public void Init(int randomSeed)
    //     {
    //         this.randomSeed = randomSeed;
    //         Random = new System.Random(this.randomSeed);
    //         curEntityIdx = 0;
    //         showEntityIdx = 0;
    //
    //     }
    //     
    //     public void ShowDisplayValue(int unitIdx)
    //     {
    //         UnShowDisplayValues();
    //         ShowDisplayValues(unitIdx);
    //     }
    //
    //     public async void ShowDisplayValues(int unitIdx)
    //     {
    //         //isShowDisplayValue = true;
    //         
    //         BattleValueEntities.Clear();
    //         
    //         var entityIdx = curEntityIdx;
    //         var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unitIdx),
    //             BattleFightManager.Instance.GetInDirectAttackDatas(unitIdx));
    //         curEntityIdx += triggerDataDict.Count;
    //
    //         
    //         // foreach (var triggerData in triggerDatas)
    //         // {
    //         //     var unit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
    //         //
    //         //     if (unit != null)
    //         //     {
    //         //         curEntityIdx++;
    //         //     }
    //         //         
    //         // }
    //
    //         var idx = 0;
    //         foreach (var kv in triggerDataDict)
    //         {
    //             var _entityIdx = entityIdx;
    //             var values = kv.Value;
    //             //ShowValues(kv.Value, entityIdx);
    //             GameUtility.DelayExcute(0.25f * idx, () =>
    //             {
    //                 ShowValues(values, _entityIdx);
    //             });
    //             idx++;
    //             entityIdx++;
    //         }
    //
    //     }
    //
    //     private async void ShowValues(List<TriggerData> triggerDatas, int entityIdx)
    //     {
    //         var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].ActionUnitIdx);
    //         var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerDatas[0].EffectUnitIdx);
    //
    //         var value = 0;
    //         foreach (var triggerData in triggerDatas)
    //         {
    //             value += (int)triggerData.ActualValue;// (int)(triggerData.Value + triggerData.DeltaValue);
    //         }
    //         
    //         
    //         Entity entity;
    //         if (effectUnit is BattleSoliderEntity solider)
    //         {
    //             //var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnit.UnitCamp);
    //             
    //             var effectUnitPos = effectUnit.Root.position;
    //             
    //             // var effectUnitFlyEndGridPosIdx =
    //             //     BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);
    //             //
    //             // if (effectUnitFlyEndGridPosIdx != -1)
    //             // {
    //             //     effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyEndGridPosIdx);
    //             // }
    //
    //             //var targetPos = heroEntity.Root.position;
    //             
    //             // var heroMovePaths = BattleFightManager.Instance.GetMovePaths(heroEntity.UnitIdx, actionUnit.UnitIdx);
    //             // if (heroMovePaths != null && heroMovePaths.Count > 0)
    //             // {
    //             //     targetPos = GameUtility.GridPosIdxToPos(heroMovePaths[heroMovePaths.Count - 1]);
    //             // }
    //
    //             effectUnitPos.y += 1f;
    //             //targetPos.y += 1f;
    //             
    //             entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(effectUnitPos, effectUnitPos,
    //                 value, entityIdx, true);
    //             
    //             entity.transform.parent = effectUnit.Root;
    //             
    //             if ((entity as BattleMoveValueEntity).BattleMoveValueEntityData.EntityIdx < showEntityIdx)
    //             {
    //                 GameEntry.Entity.HideEntity(entity);
    //             }
    //             else
    //             {
    //                 BattleValueEntities.Add(entity.Entity.Id, entity);
    //             }
    //         }
    //         else
    //         {
    //             var effectUnitPos = effectUnit.Root.position;
    //             // var effectUnitFlyGridPosIdx =
    //             //     BattleFightManager.Instance.GetFlyEndGridPosIdx(actionUnit.UnitIdx, effectUnit.UnitIdx);
    //             //
    //             // if (effectUnitFlyGridPosIdx != -1)
    //             // {
    //             //     effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitFlyGridPosIdx);
    //             // }
    //             
    //             effectUnitPos.y += 1f;
    //             
    //             entity = await GameEntry.Entity.ShowBattleDisplayValueEntityAsync(
    //                 effectUnitPos, value, entityIdx);
    //
    //             //entity.transform.parent = effectUnit.Root;
    //                     
    //             if ((entity as BattleDisplayValueEntity).BattleDisplayValueEntityData.EntityIdx < showEntityIdx)
    //             {
    //                 GameEntry.Entity.HideEntity(entity);
    //             }
    //             else
    //             {
    //                 BattleValueEntities.Add(entity.Entity.Id, entity);
    //             }
    //         }
    //
    //         entityIdx++;
    //     }
    //     
    //     public void UnShowDisplayValues()
    //     {
    //
    //         showEntityIdx = curEntityIdx;
    //         
    //         foreach (var kv in BattleValueEntities)
    //         {
    //             if (GameEntry.Entity.HasEntity(kv.Value.Entity.Id))
    //             {
    //                 GameEntry.Entity.HideEntity(kv.Value.Entity);
    //             }
    //           
    //         }
    //         BattleValueEntities.Clear();
    //         
    //     }
    // }
}