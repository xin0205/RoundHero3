using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoundHero
{

    public partial class BattleFightManager : Singleton<BattleFightManager>
    {
        public void CalculateEnemyPaths()
        {
            CalculateUnitPaths(EUnitCamp.Enemy, RoundFightData.EnemyMovePaths);
        }

        public void CalculateHeroHPDelta(MoveActionData moveActionData)
        {

            //moveActionData.MoveUnitIdx, 
            CalculateHeroHPDelta(moveActionData.TriggerDataDict, true);
        }

        public void CalculateHeroHPDelta(ActionData actionData)
        {
            // foreach (var kv in actionData.MoveData.MoveUnitDatas)
            // {
            //     CalculateHeroHPDelta(kv.Value.MoveActionData);
            // }

            //actionData.ActionUnitID, 
            CalculateHeroHPDelta(actionData.TriggerDatas);
        }


        public void CalculateHeroHPDelta(Dictionary<int, List<TriggerData>> triggerDatas,
            bool isMoveTriggerData = false)
        {
            // foreach (var kv in triggerDatas)
            // {
            //     for (int i = 0; i < kv.Value.Count; i++)
            //     {
            //         var triggerData = kv.Value[i];
            //         var effectUnit = GetUnitByID(triggerData.EffectUnitID);
            //         if (effectUnit == null)
            //             continue;
            //                 
            //         // if (!triggerDatas.ContainsKey(triggerData.EffectUnitID))
            //         // {
            //         //     triggerDatas.Add(triggerData.EffectUnitID, new List<TriggerData>());
            //         // }
            //         
            //         
            //         if (effectUnit.AddHeroHP > 0)
            //         {
            //             //var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(effectUnit.UnitCamp);
            //             // var addHeroHPTriggerData = new TriggerData()
            //             // {
            //             //     ActionUnitID = triggerData.ActionUnitID,
            //             //     EffectUnitID = playerData.BattleHero.ID,
            //             //     TriggerDataType = ETriggerDataType.RoleAttribute,
            //             //     BattleUnitAttribute = EUnitAttribute.HP,
            //             //     Value = effectUnit.AddHeroHP,
            //             //     ChangeHPInstantly = true,
            //             // };
            //             //
            //             // BattleBuffManager.Instance.CacheTriggerData(addHeroHPTriggerData, triggerDatas[kv.Key]);
            //             //effectUnit.AddHeroHP = 0;
            //         }
            //     }
            // }


            var hpDeltaList = new List<HPDeltaData>();

            foreach (var kv in triggerDatas)
            {

                foreach (var triggerData in kv.Value)
                {

                    if (triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
                    {
                        var hpDeltaData = HeroManager.Instance.AddHPDelta(triggerData);
                        if (hpDeltaData != null)
                        {
                            AddHPDetlaData(PlayerManager.Instance.PlayerData.UnitCamp, hpDeltaData);
                            //RoundFightData.HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Add(hpDeltaData);
                            hpDeltaList.Add(hpDeltaData);
                        }

                    }
                    else
                    {

                        var effectUnit = GameUtility.GetUnitDataByIdx(triggerData.EffectUnitIdx);
                        if (effectUnit == null)
                            continue;

                        var triggerValue = triggerData.Value + triggerData.DeltaValue;

                        //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                        if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
                            continue;

                        if (effectUnit.UnitCamp == EUnitCamp.Enemy)
                        {
                            //triggerData.ChangeHPInstantly = true;
                            continue;
                        }


                        var value = triggerData.CoreHPDelta;
                        // if (value > 0)
                        // {
                        //     triggerData.HeroHPDelta = value;
                        // }
                        
                        if (RoundFightData.GamePlayData.BlessCount(EBlessID.AddCurHPByAttackDamage,
                                BattleManager.Instance.CurUnitCamp) > 0)
                        {
                            value = Math.Abs((int)triggerValue);
                        }

                        effectUnit.AddCoreHP = 0;

                        if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) &&
                            !effectUnit.Exist())
                            continue;

                        if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && !GameUtility.ContainRoundState(
                                GamePlayManager.Instance.GamePlayData,
                                EBuffID.Spec_CurseUnEffect))
                            continue;

                        // if (effectUnit.CurHP >= 0 && effectUnit is Data_BattleSolider)
                        // {
                        //     triggerData.HeroHPDelta = true;
                        // }


                        // var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
                        // var isHeroUnit = playerData != null && playerData.BattleHero != null &&
                        //                  playerData.BattleHero.Idx == unit.Idx;
                        var units = GameUtility.GetUnitsByCamp(effectUnit.UnitCamp);

                        var isCoreUnit = units.Exists((battleUnit =>
                            battleUnit.Idx == effectUnit.Idx && battleUnit is Data_BattleCore));
                        //hpDeltaDict[effectUnit.UnitCamp].HPDelta += (int) (isCoreUnit ? triggerValue : Math.Abs(value));
                        //hpDeltaDict[effectUnit.UnitCamp].Key = isMoveTriggerData ? kv.Key : playerData.BattleHero.Idx;

                        var hpDeltaData = HeroManager.Instance.AddHPDelta(triggerData);
                        hpDeltaData.HPDelta = (int)(isCoreUnit ? triggerValue : value);
                        //RoundFightData.HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Add(hpDeltaData);
                        AddHPDetlaData(PlayerManager.Instance.PlayerData.UnitCamp, hpDeltaData);
                        hpDeltaList.Add(hpDeltaData);
                    }


                }



            }

            var playerData = RoundFightData.GamePlayData.GetPlayerData(PlayerManager.Instance.PlayerData.UnitCamp);
            foreach (var hpDeltaData in hpDeltaList)
            {

                // && kv2.Value.HPDelta != 0
                if (playerData != null && playerData.BattleHero != null)
                {
                    playerData.BattleHero.ChangeHP(hpDeltaData.HPDelta);

                }
            }

            // var hpDeltaDict = new Dictionary<EUnitCamp, HPDeltaData>()
            // {
            //     [EUnitCamp.Player1] = new HPDeltaData(),
            //     [EUnitCamp.Player2] = new HPDeltaData(),
            // };
            //
            //
            // foreach (var kv in triggerDatas)
            // {
            //     for (int i = kv.Value.Count - 1; i >= 0; i--)
            //     {
            //         var triggerData = kv.Value[i];
            //         
            //         var effectUnit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID);
            //         if(effectUnit == null)
            //             continue;
            //
            //         var triggerValue = triggerData.Value + triggerData.DeltaValue;
            //         var value = effectUnit.AddHeroHP;
            //         
            //         if(BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) && effectUnit.CurHP <= 0)
            //             continue;
            //
            //         if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && ! GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData,
            //                 EBuffID.Spec_CurseUnEffect))
            //             continue;
            //         
            //         effectUnit.AddHeroHP = 0;
            //         //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
            //         if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
            //             continue;
            //
            //         var unit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID, true);
            //         if(unit == null)
            //             continue;
            //         
            //         if (unit.UnitCamp == EUnitCamp.Enemy)
            //         {
            //             triggerData.ChangeHPInstantly = true;
            //             continue;
            //         }
            //
            //         if (unit.UnitRole != EUnitRole.Hero)
            //         {
            //             triggerData.ChangeHPInstantly = true;
            //         }
            //
            //         var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
            //         var isHeroUnit = playerData != null && playerData.BattleHero != null &&
            //                          playerData.BattleHero.ID == unit.ID;
            //         
            //         hpDeltaDict[unit.UnitCamp].Value += (int) (isHeroUnit ? triggerValue : Math.Abs(value));
            //         hpDeltaDict[unit.UnitCamp].Key = isMoveTriggerData ? kv.Key : playerData.BattleHero.ID;
            //
            //         if (unit.UnitRole == EUnitRole.Hero && !triggerData.ChangeHPInstantly)
            //         {
            //             kv.Value.RemoveAt(i);
            //         }
            //     }
            //     
            //     // foreach (var triggerData in kv.Value)
            //     // {
            //     //     var effectUnit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID);
            //     //     if(effectUnit == null)
            //     //         continue;
            //     //
            //     //     var triggerValue = triggerData.Value + triggerData.DeltaValue;
            //     //     var value = effectUnit.AddHeroHP;
            //     //     
            //     //     if(BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) && effectUnit.CurHP <= 0)
            //     //         continue;
            //     //
            //     //     if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && ! GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData,
            //     //             EBuffID.Spec_CurseUnEffect))
            //     //         continue;
            //     //     
            //     //     effectUnit.AddHeroHP = 0;
            //     //     //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
            //     //     if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
            //     //         continue;
            //     //
            //     //     var unit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID, true);
            //     //     if(unit == null)
            //     //         continue;
            //     //     
            //     //     if (unit.UnitCamp == EUnitCamp.Enemy)
            //     //     {
            //     //         triggerData.ChangeHPInstantly = true;
            //     //         continue;
            //     //     }
            //     //
            //     //     if (unit.UnitRole != EUnitRole.Hero)
            //     //     {
            //     //         triggerData.ChangeHPInstantly = true;
            //     //     }
            //     //
            //     //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
            //     //     var isHeroUnit = playerData != null && playerData.BattleHero != null &&
            //     //                      playerData.BattleHero.ID == unit.ID;
            //     //     
            //     //     hpDeltaDict[unit.UnitCamp].Value += (int) (isHeroUnit ? triggerValue : Math.Abs(value));
            //     //     hpDeltaDict[unit.UnitCamp].Key = isMoveTriggerData ? kv.Key : playerData.BattleHero.ID;
            //     // }
            //     
            //
            //     
            // }
            //
            // foreach (var kv2 in hpDeltaDict)
            // {
            //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(kv2.Key);
            //     if (playerData != null && playerData.BattleHero != null && kv2.Value.Value != 0)
            //     {
            //         var triggerData = new TriggerData()
            //         {
            //             EffectUnitID = playerData.BattleHero.ID,
            //             TriggerDataType = ETriggerDataType.RoleAttribute,
            //             BattleUnitAttribute = EUnitAttribute.HP,
            //             Value = kv2.Value.Value,
            //             ChangeHPInstantly = true,
            //         };
            //         triggerData.ActionUnitID = triggerData.Value < 0 ? actionUnitID : -1;
            //         triggerData.OwnUnitID = triggerData.Value < 0 ? actionUnitID : -1;
            //         
            //         if(!triggerDatas.ContainsKey(kv2.Value.Key))
            //         {
            //             triggerDatas.Add(kv2.Value.Key, new List<TriggerData>());
            //         }
            //
            //         BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas[kv2.Value.Key]);
            //
            //     }
            // }
        }

        public void CalculateUnitPaths(EUnitCamp unitCamp, List<int> actionUnitIdxs, List<int> obstacleEnemies,
            Dictionary<int, List<int>> movePaths)
        {

            RefreshObstacleMask();
            //var gridObstacles = GetGridObstacles();

            //var heroCoord = GameUtility.GridPosIdxToCoord(playerData.BattleHero.GridPosIdx);
            //var unitPaths = new Dictionary<int, Dictionary<int, PathState>>();

            var cacheBuffDatas = new Dictionary<int, BuffData>();
            foreach (var unitkey in actionUnitIdxs)
            {
                var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[unitkey] as Data_BattleMonster;
                var drEnemy = GameEntry.DataTable.GetEnemy(battleUnit.MonsterID);
                var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                cacheBuffDatas.Add(unitkey, buffData);
            }

            var oriGridPosIdxs = new Dictionary<int, int>();
            foreach (var key in actionUnitIdxs)
            {
                var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
                oriGridPosIdxs.Add(key, battleUnitData.GridPosIdx);
            }

            var retGetRange = new List<int>(50);
            var retGetRange2 = new List<int>(50);

            actionUnitIdxs.Sort((actionUnitIdx1, actionUnitIdx2) =>
            {
                var unit1 = GetUnitByIdx(actionUnitIdx1);
                var unit2 = GetUnitByIdx(actionUnitIdx2);

                return unit1.GridPosIdx - unit2.GridPosIdx;

            });


            foreach (var actionUnitIdx in actionUnitIdxs)
            {
                var buffData = cacheBuffDatas[actionUnitIdx];
                var battleUnitData =
                    RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitIdx] as Data_BattleMonster;

                if (!battleUnitData.Exist())
                    continue;

                var drEnemy = GameEntry.DataTable.GetEnemy(battleUnitData.MonsterID);

                retGetRange.Clear();
                retGetRange2.Clear();

                retGetRange.Add(battleUnitData.GridPosIdx);
                var intersectDict = GameUtility.GetActionGridPosIdxs(battleUnitData.UnitCamp, battleUnitData.GridPosIdx,
                    drEnemy.MoveType,
                    buffData.TriggerRange, buffData.TriggerUnitCamps, buffData.BuffTriggerType, ref retGetRange,
                    ref retGetRange2, true);


                var isFindPath = false;


                foreach (var kv in intersectDict)
                {
                    var intersectList = kv.Value;
                    for (int i = 0; i < intersectList.Count; i++)
                    {
                        var intersectGridPosIdx = intersectList[i];

                        if (curObstacleMask[intersectGridPosIdx] == EGridType.Obstacle)
                        {
                            continue;
                        }

                        var runPaths = new List<int>(16);

                        var realPaths =
                            BattleFightManager.Instance.GetRunPaths(curObstacleMask, battleUnitData.GridPosIdx,
                                intersectGridPosIdx, runPaths);


                        var realTargetPosIdx = realPaths[realPaths.Count - 1];

                        // if(realTargetPosIdx == battleUnitData.GridPosIdx && realTargetPosIdx != intersectGridPosIdx)
                        //     continue;

                        if (realTargetPosIdx != intersectGridPosIdx)
                            continue;

                        if (InObstacle(curObstacleMask, realPaths))
                        {
                            continue;
                        }



                        // if(realTargetPosIdx == battleUnitData.GridPosIdx)
                        //     continue;

                        battleUnitData.GridPosIdx = realTargetPosIdx;
                        RefreshUnitGridPosIdx();

                        // var actionGridPosIdx = GameUtility.GetActionGridPosIdx(curObstacleMask, realTargetPosIdx, buffData.TriggerRange, true);
                        //  if (actionGridPosIdx == -1)
                        // {
                        //     battleUnitData.GridPosIdx = oriGridPosIdxs[actionUnitIdx];
                        //     RefreshUnitGridPosIdx();
                        //
                        //     continue;
                        // }

                        RoundFightData.GamePlayData.BattleData.GridTypes[oriGridPosIdxs[actionUnitIdx]] =
                            EGridType.Empty;
                        RoundFightData.GamePlayData.BattleData.GridTypes[realTargetPosIdx] = EGridType.Unit;
                        RefreshObstacleMask();

                        // curObstacleMask[oriGridPosIdxs[actionUnitIdx]] = EGridType.Empty;
                        // curObstacleMask[realTargetPosIdx] = EGridType.Unit;
                        // battleUnitData.GridPosIdx = realTargetPosIdx;
                        // RefreshUnitGridPosIdx();

                        movePaths.Add(actionUnitIdx, realPaths);
                        isFindPath = true;
                        break;
                    }



                    if (isFindPath)
                        break;
                }

                if (!isFindPath)
                {

                    foreach (var kv in intersectDict)
                    {
                        var unit = GetUnitByIdx(kv.Key);

                        SearchPath(curObstacleMask, actionUnitIdx, battleUnitData.GridPosIdx,
                            unit.GridPosIdx, movePaths,
                            drEnemy.MoveType.ToString().Contains("Direct8"));
                        if (movePaths.ContainsKey(battleUnitData.Idx) && movePaths[battleUnitData.Idx].Count > 0)
                        {
                            RoundFightData.GamePlayData.BattleData.GridTypes[battleUnitData.GridPosIdx] =
                                EGridType.Empty;
                            RoundFightData.GamePlayData.BattleData.GridTypes[
                                    movePaths[battleUnitData.Idx][movePaths[battleUnitData.Idx].Count - 1]] =
                                EGridType.Unit;
                            RefreshObstacleMask();
                            // curObstacleMask[battleUnitData.GridPosIdx] = EGridType.Empty;
                            // curObstacleMask[movePaths[battleUnitData.Idx][movePaths[battleUnitData.Idx].Count - 1]] =
                            //     EGridType.Unit;
                            break;
                        }
                    }
                }



                // if (!isFindPath)
                // {
                //
                //     foreach (var kv in intersectDict)
                //     {
                //         var unit = GetUnitByIdx(kv.Key);
                //
                //         SearchPath(curObstacleMask, actionUnitIdx, battleUnitData.GridPosIdx,
                //             unit.GridPosIdx, movePaths,
                //             enemyData.MoveType.ToString().Contains("Direct8"));
                //         if (movePaths.ContainsKey(battleUnitData.Idx) && movePaths[battleUnitData.Idx].Count > 0)
                //         {
                //             curObstacleMask[battleUnitData.GridPosIdx] = EGridType.Empty;
                //             curObstacleMask[movePaths[battleUnitData.Idx][movePaths[battleUnitData.Idx].Count - 1]] =
                //                 EGridType.Unit;
                //             break;
                //         }
                //     }
                //
                //
                // }
                if (!movePaths.ContainsKey(actionUnitIdx))
                {
                    movePaths.Add(actionUnitIdx, new List<int>() { battleUnitData.GridPosIdx });
                }

                movePaths[actionUnitIdx] = CacheUnitMoveDatas(actionUnitIdx,
                    movePaths[actionUnitIdx],
                    unitCamp == EUnitCamp.Enemy ? RoundFightData.EnemyMoveDatas : RoundFightData.ThirdUnitMoveDatas,
                    EUnitActionState.Run);

                RefreshObstacleMask();
            }

            RefreshPropMoveDirectUseInRound();

            foreach (var key in actionUnitIdxs)
            {
                var battleEnemy = BattleUnitDatas[key];
                battleEnemy.GridPosIdx = oriGridPosIdxs[key];
            }

            RefreshUnitGridPosIdx();



        }

        public void CalculateUnitPaths(EUnitCamp unitCamp, Dictionary<int, List<int>> movePaths)
        {
            RefreshPropMoveDirectUseInRound();

            movePaths.Clear();

            var unitIdxs = new List<int>();

            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                var battleUnitData = kv.Value;

                if (battleUnitData.UnitCamp != unitCamp)
                    continue;


                if (battleUnitData.GetAllStateCount(EUnitState.UnMove) > 0 &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                // var battleEnemyData = battleUnitData as Data_BattleMonster;
                // var drEnemy = GameEntry.DataTable.GetEnemy(battleEnemyData.MonsterID);
                // var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                //
                // var attackRange = GameUtility.GetRange(battleEnemyData.GridPosIdx, buffData.TriggerRange, unitCamp,
                //     buffData.TriggerUnitCamps, true);
                //
                // if (attackRange.Contains(BattleHeroManager.Instance.BattleHeroData.GridPosIdx))
                // {
                //     movePaths.Add(battleUnitData.ID, new List<int>()
                //     {
                //         battleUnitData.GridPosIdx
                //     });
                //     continue;
                // }

                unitIdxs.Add(battleUnitData.Idx);
            }

            // unitIDs.Sort((unit1ID, unit2ID) =>
            // {
            //     var unit1 = BattleUnitManager.Instance.GetUnitByID(unit1ID).BattleUnit;
            //     var unit2 = BattleUnitManager.Instance.GetUnitByID(unit2ID).BattleUnit;
            //     
            //     
            //     if (unit1.UnitCamp != EUnitCamp.Enemy || unit2.UnitCamp != EUnitCamp.Enemy)
            //     {
            //         return 0;
            //     }
            //
            //     var drEnemy1 = GameEntry.DataTable.GetEnemy(unit1.ID);
            //     var drEnemy2 = GameEntry.DataTable.GetEnemy(unit2.ID);
            //     
            //     if (drEnemy2.AttackType == EAttackType.Lock && drEnemy1.AttackType == EAttackType.Dynamic)
            //         return -1;
            //
            //
            //     return 1;
            //
            // });


            List<int> obstacleEnemies = new List<int>();
            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != unitCamp)
                    continue;

                obstacleEnemies.Add(kv.Value.GridPosIdx);
            }

            // for (int i = unitIDs.Count - 1; i >= 0; i--)
            // {
            //     if(i >= Constant.Enemy.EnemyActionCount)
            //         unitIDs.RemoveAt(i);
            // }

            if (unitIdxs.Count > 0)
            {
                CalculateUnitPaths(unitCamp, unitIdxs, obstacleEnemies, movePaths);

            }

        }
        
        private void SearchPath(Dictionary<int, EGridType> gridTypes, int actionUnitIdx, int startGridPosIdx,
            int endGridPosIdx, Dictionary<int, List<int>> movePaths, bool isQblique)
        {
            var paths = GameUtility.GetPaths(gridTypes, startGridPosIdx, endGridPosIdx, isQblique);
            if (paths.Count > 0)
            {
                movePaths.Add(actionUnitIdx, paths);
            }

        }

        public Dictionary<int, List<TriggerData>> GetInDirectAttackDatas(int unitIdx)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();


            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {

                    foreach (var triggerDatas in kv2.Value.MoveActionData.TriggerDataDict)
                    {
                        foreach (var triggerData in triggerDatas.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }
                            if (triggerData.ActionUnitIdx != unitIdx && triggerData.InterrelatedActionUnitIdx != unitIdx)
                            {
                                continue;
                            }

                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }

                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    foreach (var triggerDatas in kv2.Value.MoveActionData.TriggerDataDict)
                    {
                        foreach (var triggerData in triggerDatas.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }
                            if (triggerData.ActionUnitIdx != unitIdx && triggerData.InterrelatedActionUnitIdx != unitIdx)
                            {
                                continue;
                            }

                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }


            return triggerDataDict;
        }
        
        public Dictionary<int, List<TriggerData>> GetRounbStartBuffDatas(int unitIdx)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();

            foreach (var kv in RoundFightData.RoundStartBuffDatas)
            {
                foreach (var datas in kv.Value.TriggerDatas.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }
            
                        if (triggerData.ActionUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }
            
                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }
   
            return triggerDataDict;
        }


        public Dictionary<int, List<TriggerData>> GetDirectAttackDatas(int unitIdx)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();

            // if (RoundFightData.EnemyMoveDatas.ContainsKey(unitIdx))
            // {
            //     var triggerDataList = RoundFightData.EnemyMoveDatas[unitIdx].TriggerDatas.Values.ToList();
            // }
            // if (isRoundStarBuff)
            // {
            //     foreach (var kv in RoundFightData.RoundStartBuffDatas)
            //     {
            //         foreach (var datas in kv.Value.TriggerDatas.Values.ToList())
            //         {
            //             foreach (var triggerData in datas)
            //             {
            //                 // if (triggerData.ActualValue == 0)
            //                 // {
            //                 //     continue;
            //                 // }
            //
            //                 if (triggerData.ActionUnitIdx == unitIdx)
            //                 {
            //                     if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
            //                     {
            //                         triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
            //                     }
            //
            //                     triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
            //                 }
            //             }
            //         }
            //     }
            // }
            
            
            
            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {
                foreach (var datas in kv.Value.TriggerDataDict.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.ActionUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }


            // if (RoundFightData.SoliderMoveDatas.ContainsKey(unitIdx))
            // {
            //     var triggerDataList = RoundFightData.SoliderMoveDatas[unitIdx].TriggerDatas.Values.ToList();
            //     
            //     
            // }
            foreach (var kv in RoundFightData.SoliderMoveDatas)
            {
                foreach (var datas in kv.Value.TriggerDataDict.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.ActionUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }
            
            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {
                foreach (var datas in kv.Value.TriggerDataDict.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.ActionUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in kv2)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.ActionUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in kv2)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.ActionUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in kv2)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.ActionUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }



            foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            {
                foreach (var triggerData in kv.Value)
                {
                    // if (triggerData.ActualValue == 0)
                    // {
                    //     continue;
                    // }

                    if (triggerData.ActionUnitIdx == unitIdx)
                    {
                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            return triggerDataDict;
        }


        public Dictionary<int, List<TriggerData>> GetHurtInDirectAttackDatas(int effectUnitIdx, int actionUnitIdx = -1)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {

                var triggerDataList = kv.Value.MoveData.MoveUnitDatas.Values.ToList();
                foreach (var moveUnitData in triggerDataList)
                {
                    foreach (var kv2 in moveUnitData.MoveActionData.TriggerDataDict)
                    {
                        foreach (var triggerData in kv2.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }

                            if (triggerData.EffectUnitIdx != effectUnitIdx && triggerData.InterrelatedEffectUnitIdx != effectUnitIdx)
                                continue;

                            if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                                triggerData.ActionUnitIdx != actionUnitIdx)
                                continue;

                            if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {

                var triggerDataList = kv.Value.MoveData.MoveUnitDatas.Values.ToList();
                foreach (var moveUnitData in triggerDataList)
                {
                    foreach (var kv2 in moveUnitData.MoveActionData.TriggerDataDict)
                    {
                        foreach (var triggerData in kv2.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }

                            if (triggerData.EffectUnitIdx != effectUnitIdx && triggerData.InterrelatedEffectUnitIdx != effectUnitIdx)
                                continue;

                            if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                                triggerData.ActionUnitIdx != actionUnitIdx)
                                continue;

                            if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                        }
                    }
                }

            }


            return triggerDataDict;
        }

        public Dictionary<int, List<TriggerData>> GetHurtDirectAttackDatas(int effectUnitIdx, int actionUnitIdx = -1)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();

            // if (isRoundStarBuff)
            // {
            //     foreach (var kv in RoundFightData.RoundStartBuffDatas)
            //     {
            //         foreach (var kv2 in kv.Value.TriggerDatas)
            //         {
            //             foreach (var triggerData in kv2.Value)
            //             {
            //                 if (triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx == triggerData.ActionUnitIdx)
            //                 {
            //                     continue;
            //                 }
            //             
            //                 if (triggerData.EffectUnitIdx != effectUnitIdx)
            //                 {
            //                     continue;
            //                 }
            //
            //                 if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
            //                 {
            //                     continue;
            //                 }
            //
            //                 if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
            //                 {
            //                     triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
            //                 }
            //
            //                 triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
            //             }
            //
            //         }
            //     }
            // }
            

            foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            {

                foreach (var triggerData in kv.Value)
                {
                    // if (triggerData.ActualValue == 0)
                    // {
                    //     continue;
                    // }

                    if (triggerData.EffectUnitIdx != effectUnitIdx)
                    {
                        continue;
                    }

                    if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                    {
                        continue;
                    }

                    if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                    {
                        triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                    }

                    triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                }
            }

            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {
                var triggerDataList = kv.Value.TriggerDataDict.Values.ToList();
                foreach (var datas in triggerDataList)
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                        {
                            continue;
                        }

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        {
                            continue;
                        }

                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                var triggerDataList = kv.Value.TriggerDatas.Values.ToList();
                foreach (var datas in triggerDataList)
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                        {
                            continue;
                        }

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        {
                            continue;
                        }

                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                var triggerDataList = kv.Value.TriggerDatas.Values.ToList();
                foreach (var datas in triggerDataList)
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                        {
                            continue;
                        }

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        {
                            continue;
                        }

                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderMoveDatas)
            {

                foreach (var kv2 in kv.Value.TriggerDataDict)
                {
                    foreach (var triggerData in kv2.Value)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                            continue;

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                            triggerData.ActionUnitIdx != actionUnitIdx)
                            continue;

                        if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                    }
                }
            }
            
            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {

                foreach (var kv2 in kv.Value.TriggerDataDict)
                {
                    foreach (var triggerData in kv2.Value)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                            continue;

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                            triggerData.ActionUnitIdx != actionUnitIdx)
                            continue;

                        if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                    }
                }
            }

            // foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            // {
            //
            //     foreach (var triggerData in kv.Value)
            //     {
            //         if (triggerData.EffectUnitIdx != effectUnitIdx)
            //         {
            //             continue;
            //         }
            //             
            //         if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
            //         {
            //             continue;
            //         }
            //             
            //         if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
            //         {
            //             triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
            //         }
            //         triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
            //     }
            // }


            return triggerDataDict;
        }

        public Dictionary<int, List<TriggerData>> GetTacticHurtAttackDatas(int effectUnitIdx)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();


            foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            {

                foreach (var triggerData in kv.Value)
                {
                    // if (triggerData.ActualValue == 0)
                    // {
                    //     continue;
                    // }

                    // if (triggerData.EffectUnitIdx != effectUnitIdx)
                    // {
                    //     continue;
                    // }

                    // if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                    // {
                    //     continue;
                    // }

                    if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                    {
                        triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                    }

                    triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                }
            }


            return triggerDataDict;
        }


        public void SubUnitState(Data_BattleUnit unitData, EUnitState unitState, List<TriggerData> triggerDatas)
        {
            var subCount = unitData.GetAllStateCount(unitState);
            
            
            if (unitData != null && subCount > 0)
            {
                var actualSubCount = subCount;
                if (unitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualSubCount > 1)
                {
                    actualSubCount = 1;
                }

                // if (!triggerDatas.ContainsKey(unitData.Idx))
                // {
                //     triggerDatas.Add(unitData.Idx, new List<TriggerData>());
                // }
                
                var subDamageTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, unitData.Idx,
                    unitData.Idx, unitData.Idx, unitState, -actualSubCount, ETriggerDataType.State);

                subDamageTriggerData.ActionUnitGridPosIdx =
                    subDamageTriggerData.EffectUnitGridPosIdx = unitData.GridPosIdx;
                SimulateTriggerData(subDamageTriggerData, triggerDatas);
                triggerDatas.Add(subDamageTriggerData);
            }
            
        }

    }
}