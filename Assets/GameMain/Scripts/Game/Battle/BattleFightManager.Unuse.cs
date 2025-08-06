using UnityEngine;

namespace RoundHero
{
    public partial class BattleFightManager : Singleton<BattleFightManager>
    {
        
        // public void CalculateUnitPaths(EUnitCamp unitCamp, List<int> actionUnitIDs, List<int> obstacleEnemies, Dictionary<int, List<int>> movePaths)
        // {
        //     var curObstacleMask = new Dictionary<int, EGridType>();
        //
        //     var gridObstacles = GetGridObstacles();
        //
        //     foreach (var kv in RoundFightData.GamePlayData.BattleData.GridTypes)
        //     {
        //         if (kv.Value == EGridType.Obstacle)
        //         {
        //             curObstacleMask[kv.Key] = EGridType.Obstacle; 
        //         }
        //         else if (kv.Value == EGridType.TemporaryUnit)
        //         {
        //             curObstacleMask[kv.Key] = EGridType.Unit; 
        //         }
        //         else
        //         {
        //             curObstacleMask[kv.Key] = EGridType.Empty; 
        //         }
        //     }
        //
        //     foreach (var kv in  RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
        //     {
        //         if (kv.Value.UnitCamp == (unitCamp == EUnitCamp.Enemy ? EUnitCamp.Third : EUnitCamp.Enemy))
        //         {
        //             curObstacleMask[kv.Value.GridPosIdx] =  EGridType.Unit;    
        //         }
        //     }
        //     
        //     foreach (var kv in  RoundFightData.GamePlayData.BattleData.GridPropDatas)
        //     {
        //         var drGridProp = GameEntry.DataTable.GetGridProp(kv.Value.GridPropID);
        //         // if (drGridProp.GridType == EGridType.Obstacle)
        //         // {
        //         //     curObstacleMask[kv.Value.GridPosIdx] = EGridType.Obstacle;    
        //         // }
        //     }
        //     
        //     foreach (var enemyGridPosIdx in obstacleEnemies)
        //     {
        //         curObstacleMask[enemyGridPosIdx] =  EGridType.Unit;
        //     }
        //
        //     var playerData = RoundFightData.GamePlayData.GetPlayerData(EUnitCamp.Player1);
        //     curObstacleMask[playerData.BattleHero.GridPosIdx] =  EGridType.Unit; 
        //
        //     var heroCoord = GameUtility.GridPosIdxToCoord(playerData.BattleHero.GridPosIdx);
        //     var unitPaths = new Dictionary<int, Dictionary<int, PathState>>();
        //     
        //     var cacheBuffDatas = new Dictionary<int, BuffData>();
        //     foreach (var enemyKey in actionUnitIDs)
        //     {
        //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[enemyKey] as Data_BattleMonster;
        //         var drEnemy = GameEntry.DataTable.GetEnemy(battleUnit.MonsterID);
        //         var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
        //         cacheBuffDatas.Add(enemyKey, buffData);
        //     }
        //     
        //     var oriGridPosIdxs = new Dictionary<int, int>();
        //     foreach (var key in  actionUnitIDs)
        //     {
        //         var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //         oriGridPosIdxs.Add(key, battleUnitData.GridPosIdx);
        //
        //     }
        //
        //     foreach (var key in actionUnitIDs)
        //     {
        //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key] as Data_BattleMonster;
        //         
        //         var pathStates = new Dictionary<int, PathState>();
        //         unitPaths.Add(key, pathStates);
        //             
        //         var enemyCoord = GameUtility.GridPosIdxToCoord(battleUnit.GridPosIdx);
        //         var drEnemy = GameEntry.DataTable.GetEnemy(battleUnit.MonsterID);
        //         
        //         var unitActionRange =
        //             GameUtility.GetRange(battleUnit.GridPosIdx,
        //                 drEnemy.MoveType, null, null,  true);
        //         foreach (var actionGridPosIdx in unitActionRange)
        //         {
        //             battleUnit.GridPosIdx = actionGridPosIdx;
        //             var unitActionCoord = GameUtility.GridPosIdxToCoord(actionGridPosIdx);
        //
        //             var drBuff = cacheBuffDatas[key];
        //             
        //             var range = GameUtility.GetRange(actionGridPosIdx, drBuff.TriggerRange, unitCamp,
        //                 drBuff.TriggerUnitCamps, true);
        //             var inRange = range.Contains(BattleHeroManager.Instance.BattleHeroData.GridPosIdx);
        //             if(!inRange && battleUnit.GridPosIdx != actionGridPosIdx)
        //                 continue;
        //
        //             if (gridObstacles.Contains(actionGridPosIdx) && battleUnit.GridPosIdx != actionGridPosIdx)
        //                 continue;
        //     
        //             if (unitActionCoord == heroCoord)
        //                 continue;
        //                 
        //             var enemyMoveDis = GameUtility.GetDis(unitActionCoord, enemyCoord);
        //             pathStates.Add(GameUtility.GridCoordToPosIdx(unitActionCoord),
        //                 new PathState(unitActionCoord, 0, enemyMoveDis));
        //         }
        //             
        //     }
        //     
        //     foreach (var key in  actionUnitIDs)
        //     {
        //         var battleEnemy = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //         battleEnemy.GridPosIdx = oriGridPosIdxs[key];
        //     }
        //     
        //     foreach (var actionUnitID in actionUnitIDs)
        //     {
        //         var unitPath = unitPaths[actionUnitID];
        //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitID];
        //         var buffData = cacheBuffDatas[actionUnitID];
        //
        //         var maxUnitTargetCoord = new unitTargetCoord();
        //         maxUnitTargetCoord.GridPosIdx = battleUnit.GridPosIdx;
        //         foreach (var path in unitPath)
        //         {
        //             var posIdx = path.Key;
        //
        //             var realPaths =
        //                 FightManager.Instance.GetRunPaths(battleUnit.GridPosIdx, posIdx);
        //             var realTargetPosIdx = realPaths[realPaths.Count - 1];
        //
        //             if (InObstacle(curObstacleMask, realPaths))
        //             {
        //                 continue;
        //             }
        //
        //             var attackCount = GameUtility.GetAttackCount(realTargetPosIdx, buffData.TriggerRange, unitCamp,
        //                 buffData.TriggerUnitCamps, true);
        //             if (attackCount > maxUnitTargetCoord.AttackCount)
        //             {
        //                 maxUnitTargetCoord.AttackCount = attackCount;
        //                 maxUnitTargetCoord.GridPosIdx = realTargetPosIdx;
        //                 if (movePaths.ContainsKey(actionUnitID))
        //                 {
        //                     movePaths[actionUnitID] = new List<int>(realPaths);
        //                 }
        //                 else
        //                 {
        //                     movePaths.Add(actionUnitID, new List<int>(realPaths));
        //                 }
        //             }
        //
        //         }
        //
        //         
        //
        //         curObstacleMask[battleUnit.GridPosIdx] = EGridType.Empty;
        //         battleUnit.GridPosIdx = maxUnitTargetCoord.GridPosIdx;
        //         curObstacleMask[maxUnitTargetCoord.GridPosIdx] = EGridType.Unit;
        //
        //         
        //
        //     }
        //     
        //     foreach (var key in  actionUnitIDs)
        //     {
        //         var battleEnemy = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //         battleEnemy.GridPosIdx = oriGridPosIdxs[key];
        //     }
        //
        //     // //从n中取m的组合
        //     // var enemyActionQueues = GameUtility.GetPermutation(actionUnitIDs.Count, actionUnitIDs.Count);
        //     //
        //     // foreach (var enemyActionQueue in enemyActionQueues)
        //     // {
        //     //
        //     //     foreach (var queue in enemyActionQueue)
        //     //     {
        //     //         var actionUnitID = actionUnitIDs[queue];
        //     //         var unitPath = unitPaths[actionUnitID];
        //     //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitID];
        //     //         var drBuff = cacheDrBuffs[actionUnitID];
        //     //
        //     //         var maxUnitTargetCoord = new unitTargetCoord();
        //     //         maxUnitTargetCoord.GridPosIdx = battleUnit.GridPosIdx;
        //     //         foreach (var path in unitPath)
        //     //         {
        //     //             var posIdx = path.Key;
        //     //
        //     //             var realPaths =
        //     //                 FightManager.Instance.GetRunPaths(battleUnit.GridPosIdx, posIdx);
        //     //             var realTargetPosIdx = realPaths[realPaths.Count - 1];
        //     //
        //     //
        //     //             if (InObstacle(curObstacleMask, realPaths))
        //     //             {
        //     //                 continue;
        //     //             }
        //     //
        //     //             var attackCount = GameUtility.GetAttackCount(realTargetPosIdx, drBuff.TriggerRange, unitCamp,
        //     //                 drBuff.TriggerUnitCamps, drBuff.HeroInRangeTrigger, drBuff.TriggerRange == EActionType.Self,
        //     //                 true);
        //     //             if (attackCount > maxUnitTargetCoord.AttackCount)
        //     //             {
        //     //                 maxUnitTargetCoord.AttackCount = attackCount;
        //     //                 maxUnitTargetCoord.GridPosIdx = realTargetPosIdx;
        //     //                 if (movePaths.ContainsKey(actionUnitID))
        //     //                 {
        //     //                     movePaths[actionUnitID] = new List<int>(realPaths);
        //     //                 }
        //     //                 else
        //     //                 {
        //     //                     movePaths.Add(actionUnitID, new List<int>(realPaths));
        //     //                 }
        //     //             }
        //     //
        //     //         }
        //     //
        //     //         
        //     //
        //     //         curObstacleMask[battleUnit.GridPosIdx] = EGridType.Empty;
        //     //         battleUnit.GridPosIdx = maxUnitTargetCoord.GridPosIdx;
        //     //         curObstacleMask[maxUnitTargetCoord.GridPosIdx] = EGridType.Unit;
        //     //
        //     //
        //     //     }
        //     //     
        //     //     foreach (var key in  actionUnitIDs)
        //     //     {
        //     //         var battleEnemy = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //     //         battleEnemy.GridPosIdx = oriGridPosIdxs[key];
        //     //     }
        //     // }
        //
        // }
        
        // public void TriggerRangeSort(Data_BattleMonster enemy, BuffData buffData, EUnitCamp unitCamp, List<int> unitActionRange)
        // {
        //     var drEnemy = GameEntry.DataTable.GetEnemy(enemy.MonsterID);
        //     
        //     for (int i = 0; i < drEnemy.AttackTargets.Count; i++)
        //     {
        //         switch (drEnemy.AttackTargets[i])
        //         {
        //             case EAttackTarget.Hero:
        //                 unitActionRange.Sort((triggerGridPosIdx1, triggerGridPosIdx2) =>
        //                 {
        //                     var triggerRange1 = GameUtility.GetRange(triggerGridPosIdx1, buffData.TriggerRange, unitCamp,
        //                         buffData.TriggerUnitCamps, true);
        //                     var inRange1 = triggerRange1.Contains(HeroManager.Instance.BattleHeroData.GridPosIdx) ? 1 : 0;
        //                     
        //                     var triggerRange2 = GameUtility.GetRange(triggerGridPosIdx2, buffData.TriggerRange, unitCamp,
        //                         buffData.TriggerUnitCamps, true);
        //                     var inRange2 = triggerRange2.Contains(HeroManager.Instance.BattleHeroData.GridPosIdx) ? 1 : 0;
        //                     
        //                     return inRange2 - inRange1;
        //
        //                 });
        //                 break;
        //             case EAttackTarget.MoreEnemy:
        //                 unitActionRange.Sort((triggerGridPosIdx1, triggerGridPosIdx2) =>
        //                 {
        //
        //                     var triggerRange1 = GameUtility.GetRange(triggerGridPosIdx1, buffData.TriggerRange, unitCamp,
        //                     buffData.TriggerUnitCamps, true);
        //                     //var trigger1Count = triggerRange1.Count;
        //                     // foreach (var triggerGridPosIdx in triggerRange1)
        //                     // {
        //                     //     var unit = GameUtility.GetUnitByGridPosIdxMoreCamps(triggerGridPosIdx,true, unitCamp, buffData.TriggerUnitCamps);
        //                     //     if (unit != null)
        //                     //         trigger1Count += 1;
        //                     // }
        //                     
        //                     var triggerRange2 = GameUtility.GetRange(triggerGridPosIdx2, buffData.TriggerRange, unitCamp,
        //                         buffData.TriggerUnitCamps, true);
        //                     //var trigger2Count = triggerRange2.Count;
        //                     // foreach (var triggerGridPosIdx in triggerRange2)
        //                     // {
        //                     //     var unit = GameUtility.GetUnitByGridPosIdxMoreCamps(triggerGridPosIdx,true, unitCamp, buffData.TriggerUnitCamps);
        //                     //     if (unit != null)
        //                     //         trigger2Count += 1;
        //                     // }
        //
        //                     return triggerRange2.Count - triggerRange1.Count;
        //
        //
        //                 });
        //                 break;
        //             case EAttackTarget.MoreUs:
        //                 break;
        //             case EAttackTarget.MoreUnit:
        //                 break;
        //             case EAttackTarget.LessEnemy:
        //                 break;
        //             case EAttackTarget.LessUs:
        //                 break;
        //             case EAttackTarget.LessUnit:
        //                 break;
        //             case EAttackTarget.PassMoreEnemy:
        //                 break;
        //             case EAttackTarget.PassMoreUs:
        //                 break;
        //             case EAttackTarget.PassMoreUnit:
        //                 break;
        //             case EAttackTarget.FastEnemy:
        //                 break;
        //             case EAttackTarget.FastUs:
        //                 break;
        //             case EAttackTarget.FastUnit:
        //                 break;
        //             case EAttackTarget.CloseEnemy:
        //                 break;
        //             case EAttackTarget.CloseUs:
        //                 break;
        //             case EAttackTarget.CloseUnit:
        //                 break;
        //             case EAttackTarget.LessHPEnemy:
        //                 break;
        //             case EAttackTarget.LessHPUs:
        //                 break;
        //             case EAttackTarget.LessHPUnit:
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //
        //
        // }
        //
        
        // public int GetDamage(int unitID)
        // {
        //     Data_BattleUnit unit  = null;
        //     if (RoundFightData.GamePlayData.BattleData.BattleUnitDatas.ContainsKey(unitID))
        //     {
        //         unit  = BattleUnitManager.Instance.BattleUnitDatas[unitID];
        //     }
        //     
        //     BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unitID, out List<BuffValue> triggerBuffDatas);
        //     var damage = 0;
        //     
        //     foreach (var triggerBuffData in triggerBuffDatas)
        //     {
        //         if (triggerBuffData.BuffData.UnitAttribute == EUnitAttribute.HP &&  triggerBuffData.ValueList[0] <= 0)
        //         {
        //             damage += (int) triggerBuffData.ValueList[0];
        //         }
        //     }
        //
        //     return damage + unit.BaseDamage;
        // }
        //
        // public int GetFlyEndGridPosIdx(int actionUnitIdx, int effectUnitIdx)
        // {
        //     Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
        //     
        //     if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(actionUnitIdx))
        //     {
        //         moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[actionUnitIdx].MoveData
        //             .MoveUnitDatas;
        //     }
        //     
        //     foreach (var kv in moveDataDict)
        //     {
        //         var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
        //         
        //         if(effectUnitIdx != kv.Value.MoveActionData.MoveUnitIdx)
        //             continue;
        //         
        //         
        //         return moveGridPosIdx[moveGridPosIdx.Count - 1];
        //     }
        //     
        //     return -1;
        // }
        //
        // public int GetFlyStartGridPosIdx(int actionUnitIdx, int effectUnitIdx)
        // {
        //     Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
        //     
        //     if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(actionUnitIdx))
        //     {
        //         moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[actionUnitIdx].MoveData
        //             .MoveUnitDatas;
        //     }
        //     
        //     foreach (var kv in moveDataDict)
        //     {
        //         var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
        //         
        //         if(effectUnitIdx != kv.Value.MoveActionData.MoveUnitIdx)
        //             continue;
        //         
        //         
        //         return moveGridPosIdx[0];
        //     }
        //     
        //     return -1;
        // }
        
        // public List<int> GetMovePaths(int unitIdx)
        // {
        //     if(RoundFightData.EnemyMovePaths.ContainsKey(unitIdx))
        //     {
        //         return RoundFightData.EnemyMovePaths[unitIdx];
        //     }
        //
        // }
        
        // public List<int> GetMovePaths(int moveUnitIdx, int actionUnitIdx = -1)
        // {
        //     List<int> movePaths = new List<int>();
        //     var unitData = GameUtility.GetUnitDataByIdx(moveUnitIdx);
        //     if(RoundFightData.EnemyMovePaths.ContainsKey(moveUnitIdx) && RoundFightData.EnemyMovePaths[moveUnitIdx] != null && unitData != null)
        //     {
        //
        //         if(RoundFightData.EnemyMovePaths[moveUnitIdx].Count > 0 && RoundFightData.EnemyMovePaths[moveUnitIdx][0] != unitData.GridPosIdx)
        //         {
        //             movePaths.Add(unitData.GridPosIdx);
        //             movePaths.AddRange(RoundFightData.EnemyMovePaths[moveUnitIdx]);
        //         }
        //         else
        //         {
        //             movePaths = RoundFightData.EnemyMovePaths[moveUnitIdx];
        //         }
        //     }
        //
        //     // else if (RoundFightData.EnemyAttackDatas.ContainsKey(actionUnitIdx))
        //     // {
        //     //     var triggerDataList = RoundFightData.EnemyAttackDatas[actionUnitIdx].MoveData.MoveUnitDatas.Values.ToList();
        //     //     foreach (var moveUnitData in triggerDataList)
        //     //     {
        //     //         if (moveUnitData.MoveActionData.MoveUnitIdx == moveUnitIdx)
        //     //             movePaths = moveUnitData.MoveActionData.MoveGridPosIdxs;
        //     //
        //     //     }
        //     // }
        //     else
        //     {
        //         //var uniData = GameUtility.GetUnitDataByIdx(moveUnitIdx);
        //         if (unitData != null)
        //         {
        //             movePaths.Add(unitData.GridPosIdx);
        //         }
        //         
        //     }
        //     
        //     
        //
        //     return movePaths;
        //
        // }
        
        // private void CacheUnitUnitBeMoveDatas(ActionData actionData)
        // {
        //     foreach (var kv in actionData.UnitMovePaths)
        //     {
        //         actionData.UnitMovePaths[kv.Key] = CacheUnitMoveDatas(kv.Key,
        //             kv.Value, actionData.UnitBeMoveDatas, actionData);
        //     }
        //     
        // }
        
        // private int GetHurt(BattleUnitEntity unit, int hpValue, ref int addDefenseCount)
        // {
        //     var hurt = 0;
        //     if (hpValue > unit.MaxHP - unit.CurHP)
        //     {
        //         hurt += unit.MaxHP - unit.CurHP;
        //     }
        //     else
        //     {
        //         if (hpValue < 0)
        //         {
        //             if (Math.Abs(hpValue) >= addDefenseCount)
        //             {
        //                 addDefenseCount = 0;
        //                 hpValue += addDefenseCount;
        //             }
        //             else
        //             {
        //                 addDefenseCount += hpValue;
        //                 hpValue = 0;
        //             }
        //         }
        //
        //         hurt += hpValue;
        //     }
        //
        //     return hurt;
        // }
        //
        //
        // public int GetTotalDelta(int unitID, EHeroAttribute heroAttribute)
        // {
        //     var fightUnitData = GameUtility.GetUnitDataByIdx(unitID, true);
        //     var unitData = GameUtility.GetUnitDataByIdx(unitID, false);
        //
        //     if (fightUnitData == null)
        //         return 0;
        //
        //     if (unitData == null)
        //         return 0;
        //
        //     var fightHeroData = fightUnitData as Data_BattleHero;
        //     var heroData = unitData as Data_BattleHero;
        //
        //     switch (heroAttribute)
        //     {
        //         case EHeroAttribute.CurHeart:
        //
        //             break;
        //         case EHeroAttribute.MaxHeart:
        //             break;
        //         case EHeroAttribute.HP:
        //
        //             if (fightHeroData != null && heroData != null)
        //             {
        //                 // var hpDelta = fightHeroData.RoundHeroHPDelta;
        //                 // hpDelta = Mathf.Clamp(hpDelta, -unitData.CurHP, heroData.MaxHP - heroData.CurHP);
        //                 //
        //                 // return hpDelta;
        //
        //                 //return deltaHeart * fightHeroData.MaxHP + (deltaHeart == 0 ? fightHeroData.CurHP - curHeroData.CurHP : 
        //
        //                 // if (fightHeroData.Attribute.GetAttribute(EHeroAttribute.CurHeart) <= 0)
        //                 // {
        //                 //     return - curHeroData.CurHP;
        //                 // }
        //                 // else 
        //
        //                 var deltaHeart = fightHeroData.Attribute.GetAttribute(EHeroAttribute.CurHeart) -
        //                                  heroData.Attribute.GetAttribute(EHeroAttribute.CurHeart);
        //                 
        //                 if (deltaHeart < 0)
        //                 {
        //                     return -((fightHeroData.MaxHP - fightHeroData.CurHP) + heroData.CurHP);
        //                 }
        //                 else
        //                 {
        //                     return fightHeroData.CurHP - heroData.CurHP;
        //                 }
        //             }
        //             else
        //             {
        //                 return fightUnitData.CurHP - unitData.CurHP;
        //             }
        //         case EHeroAttribute.MaxHP:
        //             break;
        //
        //         case EHeroAttribute.MaxCardCountEachRound:
        //             break;
        //         case EHeroAttribute.Coin:
        //             return (int) (
        //                 fightHeroData.Attribute.GetAttribute(EHeroAttribute.Coin) -
        //                 heroData.Attribute.GetAttribute(EHeroAttribute.Coin));
        //         case EHeroAttribute.Refresh:
        //             break;
        //         case EHeroAttribute.Gem:
        //             break;
        //         case EHeroAttribute.Empty:
        //             break;
        //
        //
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(heroAttribute), heroAttribute, null);
        //     }
        //
        //     return 0;
        // }
        
        // public Data_BattleUnit GetLastRoundUnitByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null,
        //     ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null)
        // {
        //     return InternalGetUnitByGridPosIdx(RoundFightData.GamePlayData.LastBattleData.BattleUnitDatas, gridPosIdx,
        //         selfUnitCamp, unitCamp, unitRole);
        //
        // }
        
        // public int GetUnitIDByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null,
        //     EUnitRole? unitRole = null)
        // {
        //     var unit = GetUnitByGridPosIdx(gridPosIdx, selfUnitCamp, unitCamp, unitRole);
        //     if (unit != null)
        //         return unit.Idx;
        //
        //     return -1;
        // }

        // public TriggerData Unit_UnitAttribute(int triggerSoliderID, int actionSoliderID, int effectUnitID,
        //     EUnitAttribute attribute, float attributeValue)
        // {
        //
        //     return BattleRoleAttribute(triggerSoliderID, actionSoliderID, effectUnitID, attribute, attributeValue);
        // }
        
        // public void RoundStartTrigger()
        // {
        //     if(ActionProgress != EActionProgress.RoundStart)
        //         return;
        //     
        //     foreach (var kv in RoundFightData.RoundStartDatas)
        //     {
        //         foreach (var triggerDatas in kv.Value.TriggerDatas.Values)
        //         {
        //             foreach (var triggerData in triggerDatas)
        //             {
        //                 TriggerAction(triggerData);
        //             }
        //             
        //         }
        //         
        //         
        //     }
        //
        //     GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        //     GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
        //
        //
        //     GameUtility.DelayExcute(1f, () =>
        //     {
        //         AcitonUnitIdx = 0;
        //         BattleManager.Instance.NextAction();
        //         BattleManager.Instance.ContinueAction();
        //     });
        // }
        
        // private float UnitAttack(int unitID, ActionData actionData)
        // {
        //
        //     var isAttack = false;
        //     foreach (var trigger in actionData.TriggerDatas)
        //     {
        //         foreach (var triggerData in trigger.Value)
        //         {
        //             isAttack = true;
        //             //TriggerAction(triggerData);
        //             BattleBulletManager.Instance.AddTriggerData(triggerData);
        //         }
        //     }
        //
        //     var time = 0.1f;
        //     if (isAttack || actionData.MoveData.MoveUnitDatas.Count > 0)
        //     {
        //         time += 1f;
        //         
        //         //var unit = GetUnitByIdx(unitID);
        //         var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(unitID);
        //         unitEntity.BattleUnit.AttackInRound = true;
        //         unitEntity.TargetPosIdx = actionData.TriggerDatas[0][0].EffectUnitGridPosIdx;
        //         unitEntity.Attack(actionData);
        //         GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        //         GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
        //
        //     }
        //     
        //     var moveTime = 0f;
        //     var maxMoveTime = 0f;
        //     BattleBulletManager.Instance.AddMoveActionData(unitID, actionData.MoveData);
        //     foreach (var kv in actionData.MoveData.MoveUnitDatas)
        //     {
        //         var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.UnitIdx);
        //         moveTime = effectUnitEntity.GetMoveTime(kv.Value.UnitActionState, kv.Value.MoveActionData);
        //         //effectUnitEntity.Run(kv.Value.MoveActionData);
        //         
        //         // if (kv.Value.UnitActionState == EUnitActionState.Fly)
        //         // {
        //         //     moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Fly(kv.Value.MoveActionData);
        //         // }
        //         // else if (kv.Value.UnitActionState == EUnitActionState.Run)
        //         // {
        //         //     moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Run(kv.Value.MoveActionData);
        //         // }
        //
        //         if (moveTime > maxMoveTime)
        //         {
        //             maxMoveTime = moveTime;
        //         }
        //     }
        //
        //     time += maxMoveTime;
        //
        //     return time;
        // }
        // public void ThirdUnitMove()
        // {
        //     UnitMove(RoundFightData.ThirdUnitMovePaths, RoundFightData.ThirdUnitMoveDatas, RoundFightData.ThirdUnitAttackDatas,
        //         EActionProgress.ThirdUnitMove);
        //     
        // }
        
        // public void CheckHPDeltaTriggerData(int actionUnitID, TriggerData triggerData, Dictionary<int, List<TriggerData>> triggerDatas)
        // {
        //     var hpDelta = 0;
        //     
        //     var effectUnit = GameUtility.GetUnitByID(triggerData.EffectUnitID);
        //     if(effectUnit == null)
        //         return;
        //
        //     var triggerValue = triggerData.Value + triggerData.DeltaValue;
        //     var value = effectUnit.AddHeroHP;
        //             
        //     if(BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) && effectUnit.CurHP <= 0)
        //         return;
        //
        //     if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && ! GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData,
        //         EBuffID.Spec_CurseUnEffect))
        //         return;
        //             
        //     effectUnit.AddHeroHP = 0;
        //     //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
        //     if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
        //         return;
        //
        //     var unit = GameUtility.GetUnitByID(triggerData.EffectUnitID, true);
        //     if(unit == null)
        //         return;
        //             
        //     if (unit.UnitCamp == EUnitCamp.Enemy)
        //     {
        //         triggerData.ChangeHPInstantly = true;
        //         return;
        //     }
        //
        //     if (unit.UnitRole != EUnitRole.Hero)
        //     {
        //         triggerData.ChangeHPInstantly = true;
        //     }
        //
        //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
        //     var isHeroUnit = playerData != null && playerData.BattleHero != null &&
        //                      playerData.BattleHero.ID == unit.ID;
        //             
        //     hpDelta = (int) (isHeroUnit ? triggerValue : Math.Abs(value));
        //     
        //     
        //     if (playerData != null && playerData.BattleHero != null && hpDelta != 0)
        //     {
        //         var hpDeltaTriggerData = new TriggerData()
        //         {
        //             EffectUnitID = playerData.BattleHero.ID,
        //             TriggerDataType = ETriggerDataType.RoleAttribute,
        //             BattleUnitAttribute = EUnitAttribute.HP,
        //             Value = hpDelta,
        //             ChangeHPInstantly = true,
        //         };
        //         hpDeltaTriggerData.ActionUnitID = hpDeltaTriggerData.Value < 0 ? actionUnitID : -1;
        //         hpDeltaTriggerData.OwnUnitID = hpDeltaTriggerData.Value < 0 ? actionUnitID : -1;
        //             
        //         if(!triggerDatas.ContainsKey(playerData.BattleHero.ID))
        //         {
        //             triggerDatas.Add(playerData.BattleHero.ID, new List<TriggerData>());
        //         }
        //
        //         BattleBuffManager.Instance.CacheTriggerData(hpDeltaTriggerData, triggerDatas[playerData.BattleHero.ID]);
        //
        //     }
        //     
        // }
        // public void CalculateThirdUnitPaths()
        // {
        //     CalculateUnitPaths(EUnitCamp.Third, RoundFightData.ThirdUnitMovePaths);
        // }
        // public List<int> GetGridObstacles()
        // {
        //     var obstacles = new List<int>(Constant.Area.ObstacleCount);
        //     
        //     foreach (var kv in BattleAreaManager.Instance.GridEntities)
        //     {
        //         if (kv.BattleGridEntityData.GridType == EGridType.Obstacle)
        //         {
        //             obstacles.Add(kv.BattleGridEntityData.GridPosIdx);
        //             
        //         }
        //     }
        //     
        //     return obstacles;
        // }
        
        // private void CacheUnitLinks()
        // {
        //     foreach (var kv in BattleUnitDatas)
        //     {
        //         foreach (var linkID in kv.Value.LinkIDs)
        //         {
        //             var drLink = GameEntry.DataTable.GetLink(linkID);
        //
        //             var linkageRange = GameUtility.GetRange(kv.Value.GridPosIdx, drLink.LinkRange, kv.Value.UnitCamp,
        //                 drLink.LinkUnitCamps, true);
        //
        //             foreach (var gridPosIdx in linkageRange)
        //             {
        //                 if (gridPosIdx == kv.Value.GridPosIdx)
        //                     continue;
        //
        //                 var battleUnitData =
        //                     GetUnitByGridPosIdxMoreCamps(gridPosIdx, kv.Value.UnitCamp, drLink.LinkUnitCamps);
        //                 if (battleUnitData == null)
        //                     continue;
        //
        //                 var linkUnit =
        //                     GetUnitByGridPosIdxMoreCamps(gridPosIdx, kv.Value.UnitCamp, drLink.LinkUnitCamps);
        //                 if (linkUnit == null)
        //                     continue;
        //
        //                 if (drLink.LinkType == ELinkType.Send)
        //                 {
        //                     linkUnit.Links.Add(kv.Value.Idx);
        //                 }
        //                 else if (drLink.LinkType == ELinkType.Receive)
        //                 {
        //                     kv.Value.Links.Add(linkUnit.Idx);
        //                 }
        //             }
        //         }
        //
        //     }
        // }
        //
        // private void CacheUnitActionRange()
        // {
        //     foreach (var kv in BattleUnitDatas)
        //     {
        //         var buffDatas =  BattleUnitManager.Instance.GetBuffDatas(kv.Value);
        //         CacheUnitActionRange(kv.Value, kv.Value, EUnitActionType.Own, buffDatas);
        //
        //         // foreach (var unitID in kv.Value.Links)
        //         // {
        //         //     var linkDrBuffs = BattleUnitManager.Instance.GetBuffDatas(BattleUnitDatas[unitID]);
        //         //     CacheUnitActionRange(BattleUnitDatas[unitID], kv.Value, EUnitActionType.Linkage, linkDrBuffs);
        //         // }
        //     }
        // }

         // public Dictionary<int, MoveUnitData> GetHurtDirectMoveDatas(int actionUnitIdx, int effectGridPosIdx)
        // {
        //     var triggerDataDict = new Dictionary<int, List<TriggerData>>();
        //
        //     foreach (var kv in RoundFightData.EnemyMoveDatas)
        //     {
        //         foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
        //         {
        //             if(!(kv2.Value.ActionUnitIdx == actionUnitIdx && kv2.Value.EffectGridPosIdx == effectGridPosIdx))
        //                 continue;
        //             
        //             triggerDataDict.Add(kv2.Key, kv2.Value);
        //                 
        //         }
        //     }
        //     
        //     foreach (var kv in RoundFightData.EnemyAttackDatas)
        //     {
        //         var triggerDataList = kv.Value.TriggerDatas.Values.ToList();
        //         foreach (var datas in triggerDataList)
        //         {
        //             foreach (var triggerData in datas)
        //             {
        //                 if (triggerData.EffectUnitIdx == unitIdx)
        //                 {
        //                     if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
        //                     {
        //                         triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
        //                     }
        //                     triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
        //                 }
        //             }
        //         }
        //     }
        //     
        //     foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
        //     {
        //         var triggerDataList = kv.Value.TriggerDatas.Values.ToList();
        //         foreach (var datas in triggerDataList)
        //         {
        //             foreach (var triggerData in datas)
        //             {
        //                 if (triggerData.EffectUnitIdx == unitIdx)
        //                 {
        //                     if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
        //                     {
        //                         triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
        //                     }
        //                     triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
        //                 }
        //             }
        //         }
        //     }
        //
        //
        //     return triggerDataDict;
        // }
        
        // public List<int> GetEnemyAttackHurtFlyPaths(int actionUnitIdx, int effectUnitIdx)
        // {
        //     Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
        //     
        //     if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(actionUnitIdx))
        //     {
        //         moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[actionUnitIdx].MoveData
        //             .MoveUnitDatas;
        //     }
        //     
        //     foreach (var kv in moveDataDict)
        //     {
        //         var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
        //         
        //         if(effectUnitIdx != kv.Value.MoveActionData.MoveUnitIdx)
        //             continue;
        //
        //         return moveGridPosIdx;
        //     }
        //
        //     return null;
        // }
        // private void CacheUnitActionRange(Data_BattleUnit ownUnitData, Data_BattleUnit actionUnitData,
        //     EUnitActionType actionType,
        //     List<BuffData> buffDatas)
        // {
        //     if (buffDatas == null)
        //         return;
        //
        //     foreach (var buffData in buffDatas)
        //     {
        //         var triggerRange = buffData.TriggerRange;
        //         if (buffData.BuffTriggerType == EBuffTriggerType.Pass)
        //             continue;
        //
        //         if (!buffData.RangeTrigger)
        //             continue;
        //
        //         List<ERelativeCamp> unitCamps = triggerRange == EActionType.UnitMaxDirect
        //             ? buffData.TriggerUnitCamps
        //             : null;
        //         //var attackTypes = triggerRange == EActionType.MaxDirect8 ? drBuff.TriggerUnitAttackTypes : null;
        //         var range = GameUtility.GetRange(actionUnitData.GridPosIdx, triggerRange, actionUnitData.UnitCamp,
        //             unitCamps,true);
        //
        //         foreach (var gridPosIdx in range)
        //         {
        //             var soliderActionRange = new UnitActionRange()
        //             {
        //                 OwnUnitID = ownUnitData.Idx,
        //                 ActionUnitID = actionUnitData.Idx,
        //                 UnitActionType = actionType,
        //                 BuffTriggerType = buffData.BuffTriggerType,
        //             };
        //
        //             if (UnitActionRange.ContainsKey(gridPosIdx))
        //             {
        //                 UnitActionRange[gridPosIdx].Add(soliderActionRange);
        //             }
        //             else
        //             {
        //                 var soliders = new List<UnitActionRange>();
        //                 UnitActionRange.Add(gridPosIdx, soliders);
        //                 soliders.Add(soliderActionRange);
        //             }
        //         }
        //     }
        // }
        
        private void CacheUnitLinkIDs()
        {
            // var player1UnLinkPosIdx =
            //     BattleBuffManager.Instance.GetUnLinkPosIdxs(RoundFightData.GamePlayData, EUnitCamp.Player1);
            // var player2UnLinkPosIdx =
            //     BattleBuffManager.Instance.GetUnLinkPosIdxs(RoundFightData.GamePlayData, EUnitCamp.Player2);
            // var enemyUnLinkPosIdx =
            //     BattleBuffManager.Instance.GetUnLinkPosIdxs(RoundFightData.GamePlayData, EUnitCamp.Enemy);
            //
            //
            // var propLinkDict = new GameFrameworkMultiDictionary<int, int>();
            //
            // foreach (var kv in BattleGridPropManager.Instance.GridPropDatas)
            // {
            //     var drGridProp = GameEntry.DataTable.GetGridProp(kv.Value.GridPropID);
            //     foreach (var buffID in drGridProp.GridPropIDs)
            //     {
            //         var buffStr = Enum.GetName(typeof(EBuffID), buffID);
            //
            //         var isLink = Enum.TryParse(buffStr, out ELinkID linkID);
            //
            //         if (isLink)
            //         {
            //             var drLink = GameEntry.DataTable.GetLink(linkID);
            //             var range = GameUtility.GetRange(kv.Value.GridPosIdx, drLink.LinkRange);
            //             foreach (var gridPosIdx in range)
            //             { 
            //                 propLinkDict.Add(gridPosIdx, drGridProp.Id);
            //             }
            //
            //         }
            //     }
            // }
            //
            // foreach (var kv in BattleUnitDatas)
            // {
            //     if (kv.Value.UnitCamp == EUnitCamp.Player1 && player1UnLinkPosIdx.Contains(kv.Value.GridPosIdx))
            //         continue;
            //     if (kv.Value.UnitCamp == EUnitCamp.Player2 && player2UnLinkPosIdx.Contains(kv.Value.GridPosIdx))
            //         continue;
            //
            //     if (kv.Value.UnitCamp == EUnitCamp.Enemy && enemyUnLinkPosIdx.Contains(kv.Value.GridPosIdx))
            //         continue;
            //
            //
            //     foreach (var funeID in kv.Value.FuneIdxs)
            //     {
            //         var buffData = BattleBuffManager.Instance.GetBuffData(funeID);
            //         if (buffData == null)
            //             continue;
            //         // if (buffData.BuffTriggerType == EBuffTriggerType.Link)
            //         // {
            //         //     var linkID = GameUtility.FuneIDToLinkID(FuneManager.Instance.GetFuneID(funeID));
            //         //     kv.Value.LinkIDs.Add(linkID);
            //         // }
            //     }
            //
            //     if (propLinkDict.Contains(kv.Value.GridPosIdx))
            //     {
            //         foreach (var gridPropID in propLinkDict[kv.Value.GridPosIdx])
            //         {
            //             var drGridProp = GameEntry.DataTable.GetGridProp(gridPropID);
            //             foreach (var buffIDStr in drGridProp.GridPropIDs)
            //             {
            //                 var buffStr = Enum.GetName(typeof(EBuffID), buffIDStr);
            //                 var linkID = Enum.Parse<ELinkID>(buffStr);
            //                 if (linkID != null)
            //                 {
            //                     kv.Value.LinkIDs.Add(linkID);
            //                 }
            //             }
            //         }
            //
            //     }
            //
            //     foreach (var linkID in kv.Value.BattleLinkIDs)
            //     {
            //         kv.Value.LinkIDs.Add(linkID);
            //     }
            //     
            //     var buffDatas = BattleUnitManager.Instance.GetBuffDatas(kv.Value);
            //     foreach (var buffData in buffDatas)
            //     {
            //         // if (buffData.BuffTriggerType == EBuffTriggerType.Link)
            //         // {
            //         //     var buffStr = Enum.GetName(typeof(EBuffID), buffData.BuffID);
            //         //     var linkID = Enum.Parse<ELinkID>(buffStr);
            //         //     kv.Value.LinkIDs.Add(linkID);
            //         // }
            //     }

                // if (kv.Value is Data_BattleSolider solider)
                // {
                //     var drBuffs = CardManager.Instance.GetBuffTable(solider.CardID);
                //     foreach (var drBuff in drBuffs)
                //     {
                //         if (drBuff.BuffTriggerType == EBuffTriggerType.Link)
                //         {
                //             var buffStr = Enum.GetName(typeof(EBuffID), drBuff.BuffID);
                //             var linkID = Enum.Parse<ELinkID>(buffStr);
                //             kv.Value.LinkIDs.Add(linkID);
                //         }
                //     }
                // }
                // else if (kv.Value is Data_BattleMonster monster)
                // {
                //     var drBuffs = BattleEnemyManager.Instance.GetBuffTable(monster.MonsterID);
                //     foreach (var drBuff in drBuffs)
                //     {
                //         if (drBuff.BuffTriggerType == EBuffTriggerType.Link)
                //         {
                //             var buffStr = Enum.GetName(typeof(EBuffID), drBuff.BuffID);
                //             var linkID = Enum.Parse<ELinkID>(buffStr);
                //             kv.Value.LinkIDs.Add(linkID);
                //         }
                //     }
                // }


            //}
        }
    }
}