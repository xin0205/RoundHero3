using System.Collections.Generic;
using System.Linq;
using Animancer;
using GameFramework.Event;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    // public class UnitSelection
    // {
    //     public EUnitCamp UnitCamp;
    //     public Type UnitType;
    //
    //     public UnitSelection(EUnitCamp unitCamp, Type unitType)
    //     {
    //         UnitCamp = unitCamp;
    //         UnitType = unitType;
    //     }
    // }
    
    public class TempTriggerData
    {
        public int TargetGridPosIdx = -1;
        public ETempUnitType TriggerType = ETempUnitType.Null;
        public Data_BattleUnit UnitData;
        //public BattleSoliderEntity BattleSoliderEntity;
        public int UnitOriGridPosIdx = -1;
        public List<int> TempUnitMovePaths = new ();
        public int CardEffectUnitID = -1;
        public TriggerBuffData TriggerBuffData = new ();
        
        
        public void Reset()
        {
            TargetGridPosIdx = -1;
            TriggerType = ETempUnitType.Null;
            UnitData = null;
            //BattleSoliderEntity = null;
            UnitOriGridPosIdx = -1;
            TempUnitMovePaths.Clear();
            CardEffectUnitID = -1;
            TriggerBuffData.Clear();
            
        }

        public TempTriggerData Copy()
        {
            var tempTriggerData = new TempTriggerData();
            tempTriggerData.TriggerType = TriggerType;
            if (TriggerType == ETempUnitType.Null)
            {
                return tempTriggerData;
            }

            tempTriggerData.TargetGridPosIdx = TargetGridPosIdx;
            
            if (UnitData is Data_BattleSolider solider)
            {
                tempTriggerData.UnitData = solider.Copy();
            }
            else if (UnitData is Data_BattleHero hero)
            {
                tempTriggerData.UnitData = hero.Copy();
            }
            else if (UnitData is Data_BattleMonster monster)
            {
                tempTriggerData.UnitData = monster.Copy();
            }
            
            tempTriggerData.UnitOriGridPosIdx = UnitOriGridPosIdx;
            tempTriggerData.TempUnitMovePaths = new List<int>(TempUnitMovePaths);
            tempTriggerData.CardEffectUnitID = CardEffectUnitID;
            tempTriggerData.TriggerBuffData = TriggerBuffData.Copy();

            return tempTriggerData;

        }
    }

    public class BuffValue
    {
        public BuffData BuffData;
        public List<float> ValueList;
        public int UnitIdx;
        public int TargetGridPosIdx = -1;

        public BuffValue Copy()
        {
            var buffValue = new BuffValue();
            buffValue.BuffData = BuffData.Copy();
            buffValue.ValueList = new List<float>(ValueList);
            buffValue.UnitIdx = UnitIdx;
            buffValue.TargetGridPosIdx = TargetGridPosIdx;
            return buffValue;
        }
    }

    
    public class BattleUnitManager : Singleton<BattleUnitManager>
    {
        public Dictionary<int, BattleUnitEntity>  BattleUnitEntities = new ();
        
        
        public Random Random;
        private int randomSeed;
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            GameEntry.Event.Subscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
        }
        public void Destory()
        {
            GameEntry.Event.Unsubscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
            foreach (var kv in BattleUnitEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value);
                
            }
            BattleUnitEntities.Clear();
        }

        public int GetIdx()
        {
            return DataManager.Instance.DataGame.User.CurGamePlayData.BattleData.UnitIdx++;
        }

        public int GetTempIdx()
        {
            return 999999;
        }
        
        public Dictionary<int, Data_BattleUnit> BattleUnitDatas => DataManager.Instance.DataGame.User.CurGamePlayData.BattleData.BattleUnitDatas;
        
        public BattleUnitEntity GetUnitByGridPosIdxMoreCamps(int gridPosIdx, EUnitCamp? selfUnitCamp = null, List<ERelativeCamp> unitCamps = null)
        {
            if (unitCamps == null)
                return null;
            
            foreach (var unitCamp in unitCamps)
            {
                var unit = GetUnitByGridPosIdx(gridPosIdx, selfUnitCamp, unitCamp);
                if (unit != null)
                {
                    return unit;
                }
            }
        
            return null;
        }
        
        public BattleUnitEntity GetUnitByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null, int exceptUnitID = -1)
        {
            foreach (var kv in BattleUnitEntities)
            {
                if (kv.Value is BattleUnitEntity unit)
                {
                    if(unitRole != null && unit.UnitRole != unitRole)
                        continue;
                    
                    if(unit.UnitIdx == exceptUnitID)
                        continue;
                }
                
                if (kv.Value is IMoveGrid moveGrid && moveGrid.GridPosIdx == gridPosIdx)
                {
                    if (unitCamp == ERelativeCamp.Us && kv.Value.UnitCamp == selfUnitCamp ||
                        unitCamp == ERelativeCamp.Enemy && kv.Value.UnitCamp != selfUnitCamp || 
                        unitCamp == null)
                    {
                        return kv.Value;
                    }
                }
            }
            
            return null;
            
            // foreach (var kv in BattleUnitEntities)
            // {
            //     if (attackType != null)
            //     {
            //         if (kv.Value is BattleSoliderEntity battleSoliderEntity)
            //         {
            //             var drBuff =
            //                 CardManager.Instance.GetBuffTable(battleSoliderEntity.BattleSoliderEntityData.BattleSoliderData
            //                     .CardID);
            //             var soliderAttackType = GameUtility.GetEnum<EActionType>(drBuff.TriggerRange);
            //             if (soliderAttackType != attackType)
            //             {
            //                 continue;
            //             }
            //         }
            //         else if (kv.Value is BattleMonsterEntity battleEnemyEntity)
            //         {
            //             var drEnemy = GameEntry.DataTable.GetEnemy(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.EnemyTypeID);
            //             var drBuff = GameEntry.DataTable.GetBuff(GameUtility.GetEnum<EBuffID>(drEnemy.OwnBuff));
            //             var enemyAttackType = GameUtility.GetEnum<EActionType>(drBuff.TriggerRange);
            //             if (enemyAttackType != attackType)
            //             {
            //                 continue;
            //             }
            //         }
            //         
            //     }
            //     
            //     if (kv.Value is IMoveGrid moveGrid && moveGrid.GridPosIdx == gridPosIdx &&
            //         (unitCamp == null || unitCamp == kv.Value.UnitCamp))
            //     {
            //         return kv.Value;
            //     }
            // }
            
        }
        
        
        public List<Data_BattleUnit> GetUnitsByCamp(EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null)
        {
            var units = new List<Data_BattleUnit>();
            foreach (var kv in BattleUnitEntities)
            {
                if (unitCamp == ERelativeCamp.Us && kv.Value.UnitCamp == selfUnitCamp ||
                    unitCamp == ERelativeCamp.Enemy && kv.Value.UnitCamp != selfUnitCamp || 
                    unitCamp == null)
                {
                    units.Add(kv.Value.BattleUnit);
                }
            }
            
            return units;
            
            
            
        }
        
        
        // public IUnit GetUnitByIDMoreCamps(int id, List<EUnitCamp> unitCamps = null,
        //     EActionType? attackType = null)
        // {
        //     foreach (var unitCamp in unitCamps)
        //     {
        //         var unit = GetUnitByID(id, unitCamp, attackType);
        //         if (unit != null)
        //         {
        //             return unit;
        //         }
        //     }
        //
        //     return null;
        // }
        
        public BattleUnitEntity GetUnitByIdx(int id)
        {
            // foreach (var kv in BattleUnitEntities)
            // {
            //     if (attackType != null)
            //     {
            //         if (kv.Value is BattleSoliderEntity battleSoliderEntity)
            //         {
            //             var drBuff =
            //                 CardManager.Instance.GetBuffTable(battleSoliderEntity.BattleSoliderEntityData.BattleSoliderData
            //                     .CardID);
            //             var soliderAttackType = GameUtility.GetEnum<EActionType>(drBuff.TriggerRange);
            //             if (soliderAttackType != attackType)
            //             {
            //                 continue;
            //             }
            //         }
            //         else if (kv.Value is BattleMonsterEntity battleEnemyEntity)
            //         {
            //             var drEnemy = GameEntry.DataTable.GetEnemy(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.EnemyTypeID);
            //             var drBuff = GameEntry.DataTable.GetBuff(GameUtility.GetEnum<EBuffID>(drEnemy.OwnBuff));
            //             var enemyAttackType = GameUtility.GetEnum<EActionType>(drBuff.TriggerRange);
            //             if (enemyAttackType != attackType)
            //             {
            //                 continue;
            //             }
            //         }
            //         
            //     }
            //     
            //     if (kv.Value.ID == id &&
            //         (unitType == null || unitType == kv.Value.UnitCamp))
            //     {
            //         return kv.Value;
            //     }
            // }
            
            foreach (var kv in BattleUnitEntities)
            {
                if (kv.Value.UnitIdx == id)
                {
                    return kv.Value;
                }
                
                
            }
            
            return null;
        }

        public int GetUnitIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null)
        {
            var unit = GetUnitByGridPosIdx(gridPosIdx, selfUnitCamp, unitCamp, unitRole);
            if(unit != null) 
                return unit.UnitIdx;

            return -1;
        }

        public int GetUnitHP(int cardID)
        {
            return BattleCurseManager.Instance.SameUnitSameCurHP_GetUnitHP(cardID);
        }


        public void RoundStartTrigger()
        {
            RefreshRoundStates();
            
            foreach (var kv in BattleUnitEntities)
            {
                if (kv.Value.BattleUnit is Data_BattleMonster monster)
                {
                    monster.IsCalculateAction = false;
                }
                
                if (kv.Value.BattleUnit is Data_BattleSolider solider)
                {
                    solider.RoundMoveTimes = 0;
                    solider.RoundAttackTimes = 0;

                }

                kv.Value.BattleUnit.AddHeroHP = 0;
            }
        }

        public void RefreshRoundStates()
        {
            foreach (var kv in BattleUnitEntities)
            {
                var keys = kv.Value.BattleUnit.UnitState.UnitStates.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    //kv.Value.BattleUnit.RemoveState(keys[i]);
                }
  
            }
        }
        
        
        
        public void RefreshDamageState()
        {
            foreach (var kv in BattleUnitEntities)
            {
                kv.Value.RefreshDamageState();
            }
        }
        
        public void OnRefreshUnitData(object sender, GameEventArgs e)
        {
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                // if (kv.Value.UnitCamp == EUnitCamp.Third)
                // {
                //     (kv.Value as BattleMonsterEntity).RefreshData();
                // }
                kv.Value.RefreshData();
            }
        }
        
        private List<BuffData> tmpBuffDatas = new List<BuffData>();
        public List<BuffData> GetBuffDatas(Data_BattleUnit battleUnit)
        {
            if (battleUnit is Data_BattleSolider soliderData)
            {
                return CardManager.Instance.GetBuffData(soliderData.CardIdx);
            }
            else if (battleUnit is Data_BattleMonster monsterData)
            {
                return BattleEnemyManager.Instance.GetBuffData(monsterData.MonsterID);
            }
            else if (battleUnit is Data_BattleHero heroData)
            {
                return HeroManager.Instance.GetBuffData(heroData.HeroID);
            }
            
            return tmpBuffDatas;
        }

        // public void RoundEnd()
        // {
        //     
        //     foreach (var kv in GamePlayManager.Instance.GamePlayData.BattleData.BattleUnitDatas)
        //     {
        //         BattleUnitManager.Instance.GetBuffValue(GamePlayManager.Instance.GamePlayData, kv.Value, out List<DRBuff> drBuffs, out List<List<float>> valueList);
        //
        //         if (drBuffs != null)
        //         {
        //             var idx = 0;
        //             foreach (var drBuff in drBuffs)
        //             {
        //                 var range = GameUtility.GetRange(kv.Value.GridPosIdx, drBuff.TriggerRange, kv.Value.UnitCamp,
        //                     drBuff.TriggerUnitCamps, drBuff.HeroInRangeTrigger, false, false);
        //
        //                 var isSubCurHP = false;
        //                 foreach (var rangeGridPosIdx in range)
        //                 {
        //                     var unit = GameUtility.GetUnitByGridPosIdx(rangeGridPosIdx);
        //                     if (unit == null)
        //                         continue;
        //
        //                     var triggerData = BattleBuffManager.Instance.UsActionEndTrigger(drBuff.BuffID, valueList[idx], kv.Value.ID, kv.Value.ID, unit.ID);
        //                     if (triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
        //                           triggerData.BattleUnitAttribute == EUnitAttribute.CurHP &&
        //                           triggerData.Value + triggerData.DeltaValue < 0)
        //                     {
        //                         isSubCurHP = true;
        //                     }
        //                 }
        //                 idx++;
        //                 BattleUnitStateManager.Instance.CheckUnitState(kv.Value.ID);
        //             }
        //         }
        //         
        //
        //         
        //     }
        // }

        
        
        public void GetBuffValue(Data_GamePlay gamePlayData, int unitID, out List<BuffValue> triggerBuffDatas)
        {
            if (BattleUnitManager.Instance.BattleUnitDatas.ContainsKey(unitID))
            {
                var unit  = BattleUnitManager.Instance.BattleUnitDatas[unitID];
                GetBuffValue(gamePlayData, unit, out triggerBuffDatas);
            }
            else
            {
                triggerBuffDatas = null;
            }

        }

        public void GetBuffValue(Data_GamePlay gamePlayData, Data_BattleUnit unit, out List<BuffValue> triggerBuffDatas, int targetGridPosIdx = -1)
        {
            triggerBuffDatas = new List<BuffValue>();
            
            if(unit == null)
                return;

            InternalGetBuffValue(unit, out List<BuffData> buffDatas, out List<List<float>> valueList);
            var idx = 0;
            foreach (var buffData in buffDatas)
            {
                triggerBuffDatas.Add(new BuffValue()
                {
                    BuffData = buffData,
                    ValueList = new List<float>(valueList[idx++]),
                    UnitIdx = unit.Idx,
                    TargetGridPosIdx = targetGridPosIdx,
                });
            };
            

            
            // idx = 0;
            // foreach (var unitID in unit.Links)
            // {
            //     var linkUnit = GameUtility.GetUnitByID(gamePlayData, unitID);
            //     InternalGetBuffValue(gamePlayData, linkUnit, out List<BuffData> linkDrBuffs, out List<List<float>> linkValueList);    
            //     
            //     foreach (var drBuff in linkDrBuffs)
            //     {
            //         idx = 0;
            //         triggerBuffDatas.Add(new BuffValue()
            //         {
            //             DrBuff = drBuff,
            //             ValueList = linkValueList[idx++],
            //             UnitID = linkUnit.ID,
            //         });
            //     };
            // }
            
           
            
        }

        private void InternalGetBuffValue(Data_BattleUnit unit, out  List<BuffData> buffDatas, out List<List<float>> valueList)
        {
            //Data_GamePlay gamePlayData, 
            if (unit is Data_BattleSolider solider)
            {
                buffDatas = CardManager.Instance.GetBuffData(solider.CardIdx);
                valueList = CardManager.Instance.GetBuffValues(solider.CardIdx);
            }
            else if (unit is Data_BattleMonster monster)
            {
                buffDatas = BattleEnemyManager.Instance.GetBuffData(monster.MonsterID);
                valueList = BattleEnemyManager.Instance.GetBuffValues(monster.MonsterID);
            }
            else if (unit is Data_BattleHero hero)
            {
                buffDatas = HeroManager.Instance.GetBuffData(hero.HeroID);
                valueList = HeroManager.Instance.GetBuffValues(hero.HeroID);


            }
            else
            {
                buffDatas = new List<BuffData>();
                valueList = new List<List<float>>();
            }

        }
        
        // public void GetSecondaryBuffValue(Data_GamePlay gamePlayData, Data_BattleUnit unit, out List<BuffValue> triggerBuffDatas)
        // {
        //     triggerBuffDatas = new List<BuffValue>();
        //     
        //     var buffStrs = new List<string>();;
        //     List<List<float>> valueList = new List<List<float>>();
        //     
        //     if (unit is Data_BattleMonster monster)
        //     {
        //         buffStrs = BattleEnemyManager.Instance.GetSecondaryBuff(monster.MonsterID);
        //         valueList = BattleEnemyManager.Instance.GetSecondaryBuffValues(monster.MonsterID);
        //     }
        //
        //     var idx = 0;
        //     foreach (var buffStr in buffStrs)
        //     {
        //         idx = 0;
        //         triggerBuffDatas.Add(new BuffValue()
        //         {
        //             BuffData = BattleBuffManager.Instance.GetBuffData(buffStr),
        //             ValueList = valueList[idx++],
        //             UnitIdx = unit.Idx,
        //         });
        //     };
        //
        // }


        public bool OnGridUnitContainCard(int cardID)
        {
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value is Data_BattleSolider solider)
                {
                    var soliderDrCard = CardManager.Instance.GetCardTable(solider.CardIdx);
                    var drCard = CardManager.Instance.GetCardTable(cardID);
                    
                    return soliderDrCard?.Id == drCard?.Id;
                }
            }

            return false;
        }
        
        public Data_BattleUnit GetBattleUnitData(BattleUnitEntity unit)
        {
            if (unit is BattleSoliderEntity soliderEntity)
            {
                return soliderEntity.BattleSoliderEntityData.BattleSoliderData;

            }
            else if (unit is BattleHeroEntity heroEntity)
            {
                return heroEntity.BattleHeroEntityData.BattleHeroData;
            }
            else if (unit is BattleMonsterEntity monsterEntity)
            {
                return monsterEntity.BattleMonsterEntityData.BattleMonsterData;
            }

            return null;
        }

        public EActionType GetMoveType(int unitID)
        {
            
            var unit = GetUnitByIdx(unitID);
            if (unit is BattleSoliderEntity soliderEntity)
            {
                var cardTable = CardManager.Instance.GetCardTable(soliderEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                return cardTable.MoveType;
            }
            else if (unit is BattleMonsterEntity monsterEntity)
            {
                var drEnemy = GameEntry.DataTable.GetEnemy(monsterEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID);
                return drEnemy.MoveType;
            }
            else if (unit is BattleHeroEntity heroEntity)
            {
                var drHero = GameEntry.DataTable.GetHero(heroEntity.BattleHeroEntityData.BattleHeroData.HeroID);
                return drHero.ActionType;
            }

            return EActionType.Empty;
        }
        
        public List<int> GetMoveRanges(int unitID, int gridPosIdx)
        {
            var rangeList = new List<int>();
            var actionType = BattleUnitManager.Instance.GetMoveType(unitID);
            
            var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
            foreach (var points in Constant.Battle.ActionTypePoints[actionType])
            {
                foreach (var point in points)
                {
                    if(point == Vector2Int.zero)
                        continue;
                    
                    var targetCoord = coord + point;
                    if (!GameUtility.InGridRange(targetCoord))
                        break;

                    var targetGridPosIdx = GameUtility.GridCoordToPosIdx(targetCoord);
                    if(GamePlayManager.Instance.GamePlayData.BattleData.GridTypes[targetGridPosIdx] == EGridType.Obstacle)
                        break;
                    
                    if(GamePlayManager.Instance.GamePlayData.BattleData.GridTypes[targetGridPosIdx] == EGridType.Unit)
                        continue;

                    rangeList.Add(GameUtility.GridCoordToPosIdx(targetCoord));
                }

            }

            return rangeList;
        }
        
        public List<int> GetAttackRanges(int unitID, int gridPosIdx)
        {
            var rangeList = new List<int>();
            var battleUnitData = GetUnitByIdx(unitID)?.BattleUnit;
            if(battleUnitData == null)
                return rangeList;
            
            BattleUnitManager.Instance.GetBuffValue(GamePlayManager.Instance.GamePlayData,
                battleUnitData, out List<BuffValue> triggerBuffDatas);
            if (triggerBuffDatas.Count > 0)
            {
                var drBuff = triggerBuffDatas[0].BuffData;
                rangeList = GameUtility.GetRange(gridPosIdx,
                    drBuff.TriggerRange,
                    battleUnitData.UnitCamp, drBuff.TriggerUnitCamps,
                    false);
            
               
            }
            
            // var triggerBuffData = triggerBuffDatas.Find(data => data.DrBuff.FlyType != EFlyType.Empty);
            // if (triggerBuffData != null)
            // {
            //     var drBuff = triggerBuffData.DrBuff;
            //     rangeList = GameUtility.GetRange(gridPosIdx,
            //         drBuff.TriggerRange,
            //         battleUnitData.UnitCamp, drBuff.TriggerUnitCamps,
            //         drBuff.HeroInRangeTrigger,
            //         drBuff.InclueCenter, false);
            //
            //    
            // }
            return rangeList;

           
        }

        public int GetUnitCount(EUnitCamp unitCamp)
        {
            var count = 0;
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp == unitCamp)
                {
                    count++;
                }
            }
            return count;
            
        }

    }
}