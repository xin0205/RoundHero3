

using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework;
using JetBrains.Annotations;
using UnityEngine;


namespace RoundHero
{
    public static partial class GameUtility
    {
        public static void GetFuneText(int funeID, ref string name, ref string desc)
        {
            var funeName =
                Utility.Text.Format(Constant.Localization.FuneName, funeID); 
            
            name = GameEntry.Localization.GetString(funeName);
            
            var drBuff = GameEntry.DataTable.GetBuff(funeID);
            
            var values = new List<float>();
            foreach (var value in drBuff.BuffValues)
            {
                var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
                if (val != 0)
                {
                    values.Add(val);
                }
               
            }
            
            var buffDesc =
                Utility.Text.Format(Constant.Localization.FuneDesc, funeID.ToString());

            desc = GetStrByValues(GameEntry.Localization.GetString(buffDesc), values);
        }
        
        // public static void GetItemText(EBlessID blessID, ref string name, ref string desc)
        // {
        //     var blessName =
        //         Utility.Text.Format(Constant.Localization.BlessName, blessID); 
        //     
        //     name = GameEntry.Localization.GetString(blessName);
        //     
        //     var drBless = GameEntry.DataTable.GetBless(blessID);
        //     
        //     var values = new List<float>();
        //     foreach (var value in drBless.Values1)
        //     {
        //         var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
        //         if (val != 0)
        //         {
        //             values.Add(val);
        //         }
        //        
        //     }
        //     
        //     var blessDesc =
        //         Utility.Text.Format(Constant.Localization.BlessDesc, blessID.ToString());
        //
        //     desc = GetStrByValues(GameEntry.Localization.GetString(blessDesc), values);
        // }
        
        public static void GetCardText(int cardID, ref string name, ref string desc)
        {
            var cardName =
                Utility.Text.Format(Constant.Localization.CardName, cardID); 

            name = GameEntry.Localization.GetString(cardName);

            // var drBuff = GameEntry.DataTable.GetBuff(buffID);
            var drCard = GameEntry.DataTable.GetCard(cardID);

            var values = new List<float>();
            foreach (var value in drCard.Values0)
            {
                var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
                if (val != 0)
                {
                    values.Add(val);
                }
               
            }
            
            foreach (var value in drCard.Values1)
            {
                var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
                if (val != 0)
                {
                    values.Add(val);
                }
            }
            
            var cardDesc =
                Utility.Text.Format(Constant.Localization.CardDesc, cardID);

            desc = GetStrByValues(GameEntry.Localization.GetString(cardDesc), values);

            
            
        }
        
        public static void GetEnemyText(int enemyID, ref string name, ref string desc)
        {
            var enemyName =
                Utility.Text.Format(Constant.Localization.EnemyName, enemyID); 

            name = GameEntry.Localization.GetString(enemyName);

            // var drBuff = GameEntry.DataTable.GetBuff(buffID);
            var drEnemy = GameEntry.DataTable.GetEnemy(enemyID);

            var values = new List<float>();
            foreach (var value in drEnemy.OwnBuffValues1)
            {
                var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
                if (val != 0)
                {
                    values.Add(val);
                }
               
            }
            
            foreach (var value in drEnemy.SpecBuffValues)
            {
                var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
                if (val != 0)
                {
                    values.Add(val);
                }
            }
            
            var enemyDesc =
                Utility.Text.Format(Constant.Localization.EnemyDesc, enemyID);

            desc = GetStrByValues(GameEntry.Localization.GetString(enemyDesc), values);

            
            
        }
        
        public static void GetBlessText(int blessID, ref string name, ref string desc)
        {
            var blessName =
                Utility.Text.Format(Constant.Localization.BlessName, blessID); 

            name = GameEntry.Localization.GetString(blessName);

            var drBless = GameEntry.DataTable.GetBless(blessID);

            var values = new List<float>();
            foreach (var value in drBless.Values1)
            {
                var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
                if (val != 0)
                {
                    values.Add(val);
                }
               
            }

            var cardDesc =
                Utility.Text.Format(Constant.Localization.BlessDesc, blessID);

            desc = GetStrByValues(GameEntry.Localization.GetString(cardDesc), values);

            
            
        }

        public static void GetItemText(EItemType itemType, int itemID, ref string name, ref string desc)
        {
            switch (itemType)
            {
                case EItemType.Card:
                    GetCardText(itemID, ref name, ref desc);
                    break;
                case EItemType.Bless:
                    GetBlessText(itemID, ref name, ref desc);
                    break;
                case EItemType.Fune:
                    GetFuneText(itemID, ref name, ref desc);
                    break;
                case EItemType.Coin:
                    name = Constant.Localization.Attribute_Coin;
                    desc = Constant.Localization.Attribute_Coin;
                    break;
                case EItemType.HP:
                    name = Constant.Localization.Attribute_HP;
                    desc = Constant.Localization.Attribute_HP;
                    break;
                case EItemType.Heart:
                    name = Constant.Localization.Attribute_Heart;
                    desc = Constant.Localization.Attribute_Heart;
                    break;
                default:
                    name = "";
                    desc = "";
                    break;
            }
        }

        public static string GetStrByValues(string str, List<string> values, bool showSign = false)
        {
            var fValues = new List<float>();
            foreach (var value in values)
            {
                fValues.Add(BattleBuffManager.Instance.GetBuffValue(value));
            }

            return GetStrByValues(str, fValues, showSign);
        }

        public static string GetStrByValues(string str, List<float> values, bool showSign = false)
        {
            var strValues = new List<string>();
            foreach (var value in values)
            {
                strValues.Add(value > 0 ? showSign ? "+" + value : value.ToString() : value.ToString());
            }
            
            if (strValues.Count == 0)
            {
                return str;
            }
            else if (strValues.Count == 1)
            {
                return Utility.Text.Format(str,
                    strValues[0]);
            }
            else if (strValues.Count == 2)
            {
                return Utility.Text.Format(str,
                    strValues[0],strValues[1]);
            }
            else if (strValues.Count == 3)
            {
                return Utility.Text.Format(str,
                    strValues[0],strValues[1],strValues[2]);
            }
            else if (strValues.Count == 4)
            {
                return Utility.Text.Format(str,
                    strValues[0],strValues[1],strValues[2],strValues[3]);
            }
            else if (strValues.Count == 5)
            {
                return Utility.Text.Format(str,
                    strValues[0],strValues[1],strValues[2],strValues[3], strValues[4]);
            }
            else if (strValues.Count == 6)
            {
                return Utility.Text.Format(str,
                    strValues[0],strValues[1],strValues[2],strValues[3], strValues[4], strValues[5]);
            }
            else if (strValues.Count == 7)
            {
                return Utility.Text.Format(str,
                    strValues[0],strValues[1],strValues[2],strValues[3], strValues[4], strValues[5], strValues[6]);
            }
            
            return str;
        }
        
        public static int GetDis(Vector2Int coord1, Vector2Int coord2)
        {
            
            var absX = Mathf.Abs(coord1.x - coord2.x);
            var absY = Mathf.Abs(coord1.y - coord2.y);
            var dis =  absX < absY ? absX : absY;
            if (absX > absY)
            {
                dis += (absX - absY);
            }
            else
            {
                dis += (absY - absX);
            }

            return dis;
        }
        
        public static bool InGridRange(Vector2Int point)
        {
            return point.x >= 0 && point.y >= 0 && point.x <= Constant.Area.GridSize.x - 1 &&
                   point.y <= Constant.Area.GridSize.y - 1;
        }

        //private static List<int> rangeList = new(25);
        
        public static List<int> GetRange(int gridPosIdx, string actionType, string selfCampStr, string unitCampStr, bool isBattleData = true, bool allRange = false)
        {
            //var nActionType = GameUtility.GetEnum<EActionType>(actionType);
            // List<UnitSelection> unitSelections = null;
            // if(unitCamps != null)
            // {
            //     unitSelections = new List<UnitSelection>();
            //     foreach (var unitCamp in unitCamps)
            //     {
            //         unitSelections.Add(new UnitSelection(GetEnum<EUnitCamp>(unitCamp), unitType));
            //     }
            //
            // }
            
            //var nAttackTypes = new List<EActionType>();
            // if (attackTypes != null)
            // {
            //     foreach (var attackType in attackTypes)
            //     {
            //         nAttackTypes.Add(GameUtility.GetEnum<EActionType>(attackType));
            //     }
            // }

            var nActionType = GameUtility.GetEnum<EActionType>(actionType);
            var unitCamps = new List<ERelativeCamp>();
            ERelativeCamp? unitCamp = null;
            if (unitCampStr != null)
            {
                unitCamp = GameUtility.GetEnum<ERelativeCamp>(unitCampStr);
                unitCamps.Add((ERelativeCamp)unitCamp);
            }
            
            EUnitCamp selfUnitCamp = EUnitCamp.Empty;
            if (selfCampStr != null)
            {
                selfUnitCamp = GameUtility.GetEnum<EUnitCamp>(selfCampStr);
            }

            
            
            return GetRange(gridPosIdx, nActionType, selfUnitCamp, unitCamps, isBattleData, allRange);

        }
        
        // public static List<int> GetRangeMoreCamps(int gridPosIdx, EActionType actionType, EUnitCamp? selfUnitCamp = null, List<EUnitCamp>? unitCamps = null,
        //     bool includeCenter = true, bool isBattleData = true)
        // {
        //     if (unitCamps == null)
        //     {
        //         return GetRange(gridPosIdx, actionType, null, unitCamps, includeCenter, isBattleData);
        //     }
        //     else
        //     {
        //
        //         return GetRange(gridPosIdx, actionType, selfUnitCamp, unitCamps, includeCenter, isBattleData);
        //     }
        //
        // }

        
        private static List<int> retGetRange = new (50);
        //private static Dictionary<int, Data_BattleUnit> cachePosToUnits = new();

        public static Data_BattleUnit GetUnitByGridPosIdx(int gridPosIdx, bool isBattleData = true)
        {
            // if (cachePosToUnits.ContainsKey(gridPosIdx))
            //     return cachePosToUnits[gridPosIdx];
            
            var unit = isBattleData
                ? BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx)
                : BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx)?.BattleUnitData;

            // if (unit != null)
            // {
            //     cachePosToUnits.Add(gridPosIdx, unit);
            // }

            return unit;
        }

        public static List<Data_BattleUnit> GetUnitsByCamp(EUnitCamp? selfUnitCamp = null,
            ERelativeCamp? unitCamp = null, bool isBattleData = true)
        {
            return isBattleData
                ? BattleFightManager.Instance.GetUnitsByCamp(selfUnitCamp, unitCamp)
                : BattleUnitManager.Instance.GetUnitsByCamp(selfUnitCamp, unitCamp);

        }

        public static Data_BattleUnit GetUnitDataByIdx(int unitID, bool isBattleData = true)
        {
            // if (cachePosToUnits.ContainsKey(gridPosIdx))
            //     return cachePosToUnits[gridPosIdx];
            
            var unit = isBattleData
                ? BattleFightManager.Instance.GetUnitByIdx(unitID)
                : BattleUnitManager.Instance.GetUnitByIdx(unitID)?.BattleUnitData;

            // if (unit != null)
            // {
            //     cachePosToUnits.Add(gridPosIdx, unit);
            // }

            return unit;
        }
        
        
        public static Data_BattleUnit GetUnitByGridPosIdxMoreCamps(int gridPosIdx,  bool isBattleData = true, EUnitCamp? selfUnitCamp = null, List<ERelativeCamp> unitCamps = null)
        {
            var unit = isBattleData
                ? BattleFightManager.Instance.GetUnitByGridPosIdxMoreCamps(gridPosIdx, selfUnitCamp, unitCamps)
                : BattleUnitManager.Instance.GetUnitByGridPosIdxMoreCamps(gridPosIdx, selfUnitCamp, unitCamps)?.BattleUnitData;
        
            return unit;
        }
        
        public static Data_BattleHero GetHero(bool isBattleData = true)
        {
            var unit = isBattleData
                ? BattleFightManager.Instance.PlayerData.BattleHero
                : BattlePlayerManager.Instance.PlayerData.BattleHero;
        
            return unit;
        }
        
        public static Data_BattleUnit GetUnitByID(Data_GamePlay gamePlayData, int unitID)
        {
            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                
                if (kv.Value.Idx == unitID)
                {
                    return kv.Value;
                }
            }

            return null;

        }

        // public static List<int> GetRange(int gridPosIdx, EActionType actionType,
        //     EUnitCamp? selfUnitCamp = null, List<ERelativeCamp>? unitCamps = null,
        //     bool isBattleData = true)
        // {
        //     //retGetRange.Clear(); retGetRange,
        //     return GetRange(gridPosIdx, actionType, 
        //         selfUnitCamp, unitCamps, isBattleData);
        // }
        
        
        private static List<List<int>> rangeList = new (50);
        private static Dictionary<Vector2Int, Vector2Int> cacheCoords = new ();
        
        public static List<int> GetRange(int gridPosIdx, EActionType actionType, EUnitCamp selfUnitCamp, List<ERelativeCamp> unitCamps,
            bool isBattleData = true,  bool allRange = false, List<int> exceptGridPosIdxs = null)
        {

            var retGetRange = new List<int>(); 
            //bool heroInRangeTrigger = false, bool includeCenter = true, 
            
            unitCamps = unitCamps != null && unitCamps.Contains(ERelativeCamp.Empty) ? null : unitCamps;
            var isExtendActionType = Enum.GetName(typeof(EActionType), actionType).Contains("Extend");
            
            foreach (var range in rangeList)
            {
                range.Clear();
            }
            //retGetRange.Clear();

            

            if (actionType == EActionType.All)
            {
                var units = isBattleData
                    ? BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas
                    : BattleUnitManager.Instance.BattleUnitDatas;
                foreach (var kv in units)
                {
                    if(unitCamps == null)
                        continue;
                    
                    if(!kv.Value.Exist())
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Us) && !unitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != selfUnitCamp)
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Enemy) && !unitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == selfUnitCamp)
                        continue;
                    
                    retGetRange.Add(kv.Value.GridPosIdx);
                }
            }
            else if (actionType == EActionType.DeBuff)
            {
                var units = isBattleData
                    ? BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas
                    : BattleUnitManager.Instance.BattleUnitDatas;
                foreach (var kv in units)
                {
                    if(unitCamps == null)
                        continue;
                    
                    if(!kv.Value.Exist())
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Us) && !unitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != selfUnitCamp)
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Enemy) && !unitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == selfUnitCamp)
                        continue;

                    if (kv.Value.GetStateCountByEffectType(EUnitStateEffectType.DeBuff) >= 0)
                        continue;
                    
                    retGetRange.Add(kv.Value.GridPosIdx);
                }
            }
            else if (actionType == EActionType.UnFullCurHPUnit)
            {
                var units = isBattleData
                    ? BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas
                    : BattleUnitManager.Instance.BattleUnitDatas;
                foreach (var kv in units)
                {
                    if(unitCamps == null)
                        continue;
                    
                    if(!kv.Value.Exist())
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Us) && !unitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != selfUnitCamp)
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Enemy) && !unitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == selfUnitCamp)
                        continue;
                    
                    if(kv.Value.GridPosIdx == gridPosIdx)
                        continue;
                    
                    if(kv.Value.CurHP >= kv.Value.MaxHP)
                        continue;
                    
                    retGetRange.Add(kv.Value.GridPosIdx);
                }
            }
            else if (actionType == EActionType.UnitMaxDirect)
            {
                var maxCount = 0;
                var maxIdx = 0;
                var idx = 0;
                
                var direct8RangeNest = GameUtility.GetRangeNest(gridPosIdx, EActionType.Direct82Long, false);
                
                foreach (var list in direct8RangeNest)
                {
                    var unitCount = 0;
                    foreach (var posIdx in list)
                    {
                        var unit = GetUnitByGridPosIdxMoreCamps(posIdx, isBattleData, selfUnitCamp, unitCamps);
                        
                        if(unit == null)
                            continue;
                        
                        if(!unit.Exist())
                            continue;
                        
                        if(unitCamps.Contains(ERelativeCamp.Us) && selfUnitCamp != unit.UnitCamp)
                            continue;
                        
                        if(unitCamps.Contains(ERelativeCamp.Enemy)  && selfUnitCamp == unit.UnitCamp)
                            continue;
                        
                        
                        if(posIdx == gridPosIdx)
                            continue;
            
                        
                        unitCount += 1;
                    }
            
                    maxIdx = unitCount > maxCount ? idx : maxIdx;
                    maxCount = unitCount > maxCount ? unitCount : maxCount;
                    idx++;
                }

                foreach (var matchGridPosIdx in direct8RangeNest[maxIdx])
                {
                    retGetRange.Add(matchGridPosIdx);
                }
                
                //rangeList.Add(direct8RangeNest[maxIdx]);
            }
            else if (actionType == EActionType.HeroDirect)
             {
                 var idx = 0;
                 //var matchIdx = -1;
                 var maxIdx = -1;
                 var maxCount = 0;
                 var direct8RangeNest = GameUtility.GetRangeNest(gridPosIdx, EActionType.Direct82Long, false);
                 
                 foreach (var list in direct8RangeNest)
                 {
                     var unitCount = 0;
                     foreach (var posIdx in list)
                     {
                         if(posIdx == gridPosIdx)
                             continue;
                         
                         var unit = GetUnitByGridPosIdxMoreCamps(posIdx, isBattleData, selfUnitCamp, unitCamps);
                         
                         if(unit == null)
                             continue;
                         
                         if(!unit.Exist())
                             continue;
                         
                         // if(unit is not Data_BattleCore)
                         //     continue;
                         
                         
                         //
                         // if(unitCamps.Contains(ERelativeCamp.Enemy)  && selfUnitCamp == unit.UnitCamp)
                         //     continue;
                         
                         if (unitCamps.Contains(ERelativeCamp.Enemy) && selfUnitCamp != unit.UnitCamp)
                         {
                             unitCount += unit is Data_BattleCore ? 50 : 10;

                         }
                         
                         if (unitCamps.Contains(ERelativeCamp.Us) && selfUnitCamp == unit.UnitCamp)
                         {
                             unitCount += -1;

                         }
                         

                         // if (unitCamps.Contains(ERelativeCamp.Us) && selfUnitCamp == unit.UnitCamp ||
                         //     unitCamps.Contains(ERelativeCamp.Enemy) && selfUnitCamp != unit.UnitCamp)
                         // {
                         //     matchIdx = idx;
                         //     break;
                         // }

                         
                     }
            
                     // if (matchIdx != -1)
                     // {
                     //     foreach (var matchGridPosIdx in direct8RangeNest[matchIdx])
                     //     {
                     //         retGetRange.Add(matchGridPosIdx);
                     //     }
                     //     break;
                     // }

                     if (unitCount > 0)
                     {
                         maxIdx = unitCount > maxCount ? idx : maxIdx;
                         maxCount = unitCount > maxCount ? unitCount : maxCount;
                     }
                     
                     idx++;
                 }

                 if (maxIdx != -1)
                 {
                     foreach (var matchGridPosIdx in direct8RangeNest[maxIdx])
                     {
                         retGetRange.Add(matchGridPosIdx);
                     }
                 }
                 
             }
            //  else if (actionType == EActionType.UnitMaxXExtend)
            //  {
            //      var maxCount = 0;
            //      var range = GameUtility.GetRange(gridPosIdx, actionType);
            //      foreach (var rangeGridPosIdx in range)
            //      {
            //          var unit = GetUnitByGridPosIdxMoreCamps(rangeGridPosIdx, isBattleData, selfUnitCamp, unitCamps);
            //              
            //          if(unit == null)
            //              continue;
            //          
            //          if(unit.CurHP <= 0)
            //              continue;
            //          
            //          //rangeList.Add();
            //      }
            //  }
            //  else if (actionType == EActionType.Direct)
            //  {
            //      
            //      foreach (var list in direct8RangeNest)
            //      {
            //          foreach (var posIdx in list)
            //          {
            //              var unit = GetUnitByGridPosIdx(posIdx, isBattleData);
            //
            //              if (unit != null && unit.UnitRole == EUnitRole.Hero && heroInRangeTrigger)
            //              {
            //                  rangeList.Add(list);
            //                  break;
            //              }
            //          }
            //      }
            //
            //      foreach (var range in rangeList)
            //      {
            //          for (int i = range.Count - 1; i >= 0; i--)
            //          {
            //              var unit = GetUnitByGridPosIdx(range[i], isBattleData);
            //     
            //              if (unit == null || unit.CurHP <= 0)
            //              {
            //                  range.RemoveAt(i);
            //              }
            //          }
            //      }
            // }
            else if (actionType == EActionType.Cross_Long_Empty)
            {
                var gridTypes = isBattleData
                    ? BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GridTypes
                    : GamePlayManager.Instance.GamePlayData.BattleData.GridTypes;
                
                var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
                foreach (var points in Constant.Battle.ActionTypePoints[actionType])
                {
                    var range = new List<int>();
                    foreach (var point in points)
                    {
            
                        var targetCoord = coord + point;
                        if (!GameUtility.InGridRange(targetCoord))
                            continue;
                        
                        var targerGridPosIdx = GameUtility.GridCoordToPosIdx(targetCoord);
                        if (gridTypes[targerGridPosIdx] == EGridType.Empty)
                            range.Add(targerGridPosIdx);
            
                    }
            
                    
                    rangeList.Add(range);
            
                }
            }
            else
            {
                if (!Constant.Battle.ActionTypePoints.ContainsKey(actionType))
                    return retGetRange;
                
                var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);

                var idx = 0;
                foreach (var points in Constant.Battle.ActionTypePoints[actionType])
                {
                    // List<int> range;
                    // if (rangeList.Count > idx)
                    // {
                    //     range = rangeList[idx];
                    // }
                    // else
                    // {
                    //     range = new List<int>(50);
                    //     rangeList.Add(range);
                    // }
                    // idx++;
                    
                    foreach (var point in points)
                    {
                        if(actionType != EActionType.Self && point == Vector2Int.zero)
                            continue;

                        var targetCoord = coord + point;
                        if (actionType == EActionType.Row)
                        {
                            targetCoord = new Vector2Int(point.x, coord.y);
                        }
                        else if (actionType == EActionType.Column)
                        {
                            targetCoord = new Vector2Int(coord.x, point.y);
                        }

                        if (!GameUtility.InGridRange(targetCoord))
                            continue;
                        
                        var posIdx = GameUtility.GridCoordToPosIdx(targetCoord);
                        
                        if(exceptGridPosIdxs != null && exceptGridPosIdxs.Contains(posIdx))
                            continue;

                        if (unitCamps == null || allRange)
                        {
                            //range.Add(GridCoordToPosIdx(targetCoord));
                            retGetRange.Add(posIdx);
                        }
                        else
                        {
 
                             if (!isExtendActionType)
                             {
                                 var unit = GetUnitByGridPosIdxMoreCamps(posIdx, isBattleData, selfUnitCamp,
                                     isExtendActionType ? Constant.Battle.AllRelativeCamps : unitCamps);
                                 
                                 if (unit == null)
                                     continue;
                            
                                 if (!unit.Exist())
                                     continue;
                                 
                                 if(unitCamps.Contains(ERelativeCamp.Us) && !unitCamps.Contains(ERelativeCamp.Enemy) && selfUnitCamp != unit.UnitCamp)
                                     continue;
                            
                                 if(unitCamps.Contains(ERelativeCamp.Enemy)  && !unitCamps.Contains(ERelativeCamp.Us) && selfUnitCamp == unit.UnitCamp)
                                     continue;
                                 
                                 retGetRange.Add(posIdx);
                             }
                             else
                             {
                                 var gridType = GameUtility.GetGridType(posIdx, isBattleData);
                                 if(gridType == EGridType.Obstacle)
                                     break;
                                 
                                 Data_BattleUnit unit;
                                 
                                 if (isBattleData)
                                 {
                                     unit = GetUnitByGridPosIdx(posIdx, isBattleData);
                                     // if (unit != null)
                                     // {
                                     //     // if (unit.ID == HeroManager.Instance.BattleHeroData.ID)
                                     //     // {
                                     //     //     retGetRange.Add(posIdx);
                                     //     // }
                                     //     
                                     //     break;
                                     // }

                                 }
                                 else
                                 {
                                     unit = GetUnitByGridPosIdx(posIdx, isBattleData);
                                 }

                                 if (unit != null && unit.Exist())
                                 {
                                     // if (unit.ID == HeroManager.Instance.BattleHeroData.ID)
                                     // {
                                     //     retGetRange.Add(posIdx);
                                     // }
                                     retGetRange.Add(posIdx);    
                                     break;
                                 }
                                 
                                 // if (unit == null)
                                 //     continue;
                                 //
                                 // if (unit.CurHP <= 0)
                                 //     continue;
                                 //
                                 // retGetRange.Add(posIdx);
                                 // break;
                             }
                             
                            //range.Add(GridCoordToPosIdx(targetCoord));

                        }

                    }

                }

            }


            // if (isExtendActionType)
            // {
            //     foreach (var list in rangeList)
            //     {
            //         foreach (var posIdx in list)
            //         {
            //             Data_BattleUnit unit = null;
            //             if (unitCamps != null)
            //             {
            //                 if (isBattleData)
            //                 {
            //                     unit = GetUnitByGridPosIdxMoreCamps(posIdx, isBattleData, selfUnitCamp, unitCamps);
            //                 }
            //                 else
            //                 {
            //                     unit = GetUnitByGridPosIdx(posIdx, isBattleData);
            //                 }
            //                 
            //                 if (unit == null || unit.CurHP <= 0)
            //                     break;
            //             
            //                 // if (unit == null)
            //                 //     continue;
            //                 //
            //                 // if (unit.CurHP <= 0)
            //                 //     continue;
            //                 
            //                 retGetRange.Add(posIdx);
            //                 break;
            //             }
            //             else
            //             {
            //                 
            //                 retGetRange.Add(posIdx);
            //             }
            //
            //             
            //         }
            //     }
            // }
            // else
            // {
            //
            //     foreach (var list in rangeList)
            //     {
            //         foreach (var posIdx in list)
            //         {
            //             if (unitCamps != null)
            //             {
            //                 var unit = GetUnitByGridPosIdx(posIdx, isBattleData);;
            //
            //                 if(unit == null)
            //                     continue;
            //             
            //                 if(unit.CurHP <= 0)
            //                     continue;
            //             }
            //             
            //             
            //             retGetRange.Add(posIdx);
            //         }
            //     }
            // }

            // if (!retGetRange.Any(gridPosIdx =>
            // {
            //     var unit = FightManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
            //     return unit != null && unit.UnitRole == EUnitRole.Hero;
            // }))
            // {
            //     retGetRange.Clear();
            // }

            SortHeroIDToLast(retGetRange);

            return retGetRange;

        }
        
        public static bool HeroInRange(int gridPosIdx, EActionType actionType,
            bool isBattleData = true)
        {
            if (!Constant.Battle.ActionTypePoints.ContainsKey(actionType))
                return false;
            
            var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
            
            foreach (var points in Constant.Battle.ActionTypePoints[actionType])
            {
                foreach (var point in points)
                {
                    if(actionType != EActionType.Self && point == Vector2Int.zero)
                        continue;
                    
                    var targetCoord = coord + point;
                    if (actionType == EActionType.Row)
                    {
                        targetCoord = new Vector2Int(point.x, coord.y);
                    }
                    else if (actionType == EActionType.Column)
                    {
                        targetCoord = new Vector2Int(coord.x, point.y);
                    }

                    if (!GameUtility.InGridRange(targetCoord))
                        continue;
                    
                    var posIdx = GameUtility.GridCoordToPosIdx(targetCoord);


                    if (posIdx == GameUtility.GetHero(isBattleData).GridPosIdx)
                    {
                        return true;
                    }

                }

            }

            return false;
        }

        public static void SortHeroIDToLast(List<int> list)
        {
            GameUtility.InsertionSort(list, (gridPosIdx1, gridPosIdx2) =>
            {

                var unit1 = BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx1);
                var unit2 = BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx2);
                if (unit1 != null && unit1.UnitRole == EUnitRole.Hero)
                {
                    return 1;
                }
                else if (unit2 != null && unit2.UnitRole == EUnitRole.Hero)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
            

        }

        
        // public static int GetAttackCount(int gridPosIdx, EActionType actionType, List<int> retGetRange, EUnitCamp? selfUnitCamp = null, List<ERelativeCamp>? unitCamps = null,
        //     bool isBattleData = true)
        // {
        //
        //     return GameUtility.GetRange(gridPosIdx, actionType, retGetRange, selfUnitCamp, unitCamps, isBattleData).Count;
        //
        //     // if (!cacheActionTypeGridPosIdxs.ContainsKey(gridPosIdx))
        //     // {
        //     //     cacheActionTypeGridPosIdxs.Add(gridPosIdx, new Dictionary<EActionType, List<int>>());
        //     // }
        //     //
        //     // if (!cacheActionTypeGridPosIdxs[gridPosIdx].ContainsKey(actionType))
        //     // {
        //     //     cacheActionTypeGridPosIdxs[gridPosIdx].Add(actionType, new List<int>());
        //     // }
        //     //
        //     // foreach (var points in Constant.Battle.ActionTypePoints[actionType])
        //     // {
        //     //     foreach (var point in points)
        //     //     {
        //     //         if (actionType != EActionType.Self && point == Vector2Int.zero)
        //     //             continue;
        //     //         
        //     //         var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
        //     //
        //     //         var targetCoord = coord + point;
        //     //
        //     //         if (!GameUtility.InGridRange(targetCoord))
        //     //             continue;
        //     //
        //     //         var posIdx = GameUtility.GridCoordToPosIdx(targetCoord);
        //     //             
        //     //         cacheActionTypeGridPosIdxs[gridPosIdx][actionType].Add(posIdx);
        //     //     }
        //     //
        //     // }
        //     //
        //     // var cacheGridPosIdxs = cacheActionTypeGridPosIdxs[gridPosIdx][actionType];
        //     //     
        //     // foreach (var kv in FightManager.Instance.BattleUnitDatasByGridPosIdx)
        //     // {
        //     //     var unit = kv.Value;
        //     //     if (cacheGridPosIdxs.Contains(kv.Value.GridPosIdx) &&
        //     //         (unitCamps.Contains(ERelativeCamp.Us) && selfUnitCamp == unit.UnitCamp ||
        //     //          unitCamps.Contains(ERelativeCamp.Enemy) && selfUnitCamp != unit.UnitCamp))
        //     //     {
        //     //             
        //     //     }
        //     //
        //     // }
        //     
        //     
        //     // GameUtility.GetRange(gridPosIdx, actionType, retGetRange, selfUnitCamp,
        //     //     unitCamps, isBattleData);
        //     //
        //     // //return retGetRange.Count;
        //     //
        //     // var inRange = retGetRange.Contains(BattleHeroManager.Instance.BattleHeroData.GridPosIdx);
        //     //
        //     // var attackCount = 0;
        //     // if (inRange)
        //     // {
        //     //     foreach (var kv in FightManager.Instance.BattleUnitDatasByGridPosIdx)
        //     //     {
        //     //         if (gridPosIdx != kv.Value.GridPosIdx &&
        //     //             retGetRange.Contains(kv.Value.GridPosIdx) &&
        //     //             (unitCamps.Contains(ERelativeCamp.Us) && selfUnitCamp == kv.Value.UnitCamp ||
        //     //             unitCamps.Contains(ERelativeCamp.Enemy) && selfUnitCamp != kv.Value.UnitCamp))
        //     //         {
        //     //             attackCount += 1;
        //     //         }
        //     //
        //     //     }
        //     //     
        //     //     // foreach (var rangeGridPosIdx in retGetRange)
        //     //     // {
        //     //     //     var unit = FightManager.Instance.GetUnitByGridPosIdxMoreCamps(rangeGridPosIdx, selfUnitCamp, unitCamps);
        //     //     //     if (unit != null)
        //     //     //     {
        //     //     //         attackCount += 1;
        //     //     //     }
        //     //     // }
        //     // }
        //     //
        //     // return attackCount;
        // }

        private static Dictionary<EActionType, string> ActionTypeMaps = new Dictionary<EActionType, string>();
        public static int GetActionGridPosIdx(Dictionary<int, EGridType> obstacleMask, int actionGridPosIdx, EActionType attackType, bool isBattleData = true)
        {
            if (!ActionTypeMaps.ContainsKey(attackType))
            {
                ActionTypeMaps.Add(attackType, Enum.GetName(typeof(EActionType), attackType));
            }
        
            var isExtendActionType = ActionTypeMaps[attackType].Contains("Extend");
            var isOblique = ActionTypeMaps[attackType].Contains("Direct8");

            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionGridPosIdx);
            var heroUnitData = GameUtility.GetUnitDataByIdx(HeroManager.Instance.BattleHeroData.Idx, isBattleData);
            var heroCoord = GameUtility.GridPosIdxToCoord(heroUnitData.GridPosIdx);
    
            if (!isExtendActionType)
            {
                return actionGridPosIdx;
            }
            else
            {
                var deltaCoord = heroCoord - actionUnitCoord;

                if (!isOblique && Mathf.Abs(deltaCoord.x) == Mathf.Abs(deltaCoord.y))
                {
                    return -1;
                }

                if (deltaCoord.x != 0 && deltaCoord.y != 0)
                {
                    if (Mathf.Abs(deltaCoord.x) != Mathf.Abs(deltaCoord.y))
                    {
                        return -1;
                    }
                }
                        
                var normalizeX = deltaCoord.x > 0 ? 1 : deltaCoord.x < 0 ? -1 : 0;
                var normalizeY = deltaCoord.y > 0 ? 1 : deltaCoord.y < 0 ? -1 : 0;
                        
                var stepCoord = actionUnitCoord;
                while (true)
                {
                    stepCoord += new Vector2Int(normalizeX, normalizeY);
                    if (stepCoord == heroCoord)
                    {
                        return actionGridPosIdx;
                    }

                    var stepGridPosIdx = GameUtility.GridCoordToPosIdx(stepCoord);
                    if (obstacleMask[stepGridPosIdx] != EGridType.Empty)
                    {
                        return -1;
                    }
                    // var unit = GetUnitByGridPosIdx(stepGridPosIdx);
                    // if (unit != null && unit.CurHP > 0)
                    // {
                    //     return -1;
                    // }

                }

            }


        }
        
        public static Dictionary<int, List<int>> GetActionGridPosIdxs(EUnitCamp selfCamp, int gridPosIdx, EActionType moveType, EActionType attackType,
            List<ERelativeCamp> unitCamps, EBuffTriggerType buffTriggerType,   ref List<int> moveRange, ref List<int> heroHurtRange, bool isBattleData = true)
        {

            var actionUnit = GetUnitByGridPosIdx(gridPosIdx, isBattleData);
            
            var intersectDict = new Dictionary<int, List<int>>();
            var relatedEnemyUnits = GameUtility.GetUnitsByCamp(selfCamp, ERelativeCamp.Enemy);

            moveRange.Add(gridPosIdx);
            moveRange.AddRange(GetRange(gridPosIdx, moveType, selfCamp, unitCamps, isBattleData, true));
            var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
            relatedEnemyUnits.Sort((unit1, unit2) =>
            {
                if (unit2.UnitRole == EUnitRole.Hero && unit1.UnitRole != EUnitRole.Hero)
                    return 1;
                
                if (unit1.UnitRole == EUnitRole.Hero && unit2.UnitRole != EUnitRole.Hero)
                    return -1;
                
                var unit1Coord = GameUtility.GridPosIdxToCoord(unit1.GridPosIdx);
                var unit2Coord = GameUtility.GridPosIdxToCoord(unit2.GridPosIdx);
                
                var unit1Dis = GetBlockDistance(unit1Coord, coord);
                var unit2Dis = GetBlockDistance(unit2Coord, coord);

                if (unit2Dis < unit1Dis)
                    return -1;
                
                if (unit2Dis > unit1Dis)
                    return 1;

                return 0;

            });
            
            if (attackType == EActionType.HeroDirect)
            {
                attackType = EActionType.Direct82Long;
            }
            
            foreach (var battleUnitData in relatedEnemyUnits)
            {
                heroHurtRange = GetRange(battleUnitData.GridPosIdx, attackType, battleUnitData.UnitCamp, null, isBattleData, true);
                 
                var intersectList = moveRange.Intersect(heroHurtRange).ToList();

                var enemyCount = 0;
                var usCount = 0;
                for (int i = intersectList.Count - 1; i >= 0; i--)
                {
                    var range = GetRange(intersectList[i], attackType, selfCamp, unitCamps,
                        isBattleData, false, new List<int>(){gridPosIdx});

                    foreach (var rangeGridPosIdx in range)
                    {
                        if(rangeGridPosIdx == gridPosIdx)
                            continue;
                        
                        var unit = GetUnitByGridPosIdx(rangeGridPosIdx, isBattleData);
                        if (unit != null)
                        {

                            var relatedCamp = GetRelativeCamp(unit.UnitCamp, actionUnit.UnitCamp);
                            if (relatedCamp == ERelativeCamp.Enemy && unitCamps.Contains(relatedCamp))
                            {
                                enemyCount += 1;
                            }
                            else if (relatedCamp == ERelativeCamp.Us && unitCamps.Contains(relatedCamp))
                            {
                                usCount += 1;
                            }
                        }

                        
                    }
                    
                    if (enemyCount == 0 && usCount == 0)
                    {
                        intersectList.RemoveAt(i);
                    }
                    
                }
                
                intersectList.Sort((gridPosIdx1, gridPosIdx2) =>
                {
                    var range1 = GetRange(gridPosIdx1, attackType, selfCamp, unitCamps,
                        isBattleData, false, new List<int>(){gridPosIdx});

                    var range1EnemyCount = 0;
                    var range1SoliderCount = 0;
                    var range1UsCount = 0;
                    foreach (var rangeGridPosIdx in range1)
                    {
                        if(rangeGridPosIdx == gridPosIdx)
                            continue;
                        
                        var unit = GetUnitByGridPosIdx(rangeGridPosIdx, isBattleData);
                        if (unit != null)
                        {
                            
                            if (unit is Data_BattleSolider)
                            {
                                range1SoliderCount += 1;
                            }
                            else
                            {

                                var relatedCamp = GetRelativeCamp(unit.UnitCamp, actionUnit.UnitCamp);
                                if (relatedCamp == ERelativeCamp.Enemy)
                                {
                                    range1EnemyCount += 1;
                                }
                                else if (relatedCamp == ERelativeCamp.Us)
                                {
                                    range1UsCount += 1;
                                }
                            }
                        }
                    }
                    
                    var range2 = GetRange(gridPosIdx2, attackType, selfCamp, unitCamps,
                        isBattleData, false, new List<int>(){gridPosIdx});
                    
                    var range2EnemyCount = 0;
                    var range2SoliderCount = 0;
                    var range2UsCount = 0;
                    foreach (var rangeGridPosIdx in range2)
                    {
                        if(rangeGridPosIdx == gridPosIdx)
                            continue;
                        
                        var unit = GetUnitByGridPosIdx(rangeGridPosIdx, isBattleData);
                        if (unit != null)
                        {
                            if (unit is Data_BattleSolider)
                            {
                                range2SoliderCount += 1;
                            }
                            else
                            {
                                var relatedCamp = GetRelativeCamp(unit.UnitCamp, actionUnit.UnitCamp);
                                if (relatedCamp == ERelativeCamp.Enemy)
                                {
                                    range2EnemyCount += 1;
                                }
                                else if (relatedCamp == ERelativeCamp.Us)
                                {
                                    range2UsCount += 1;
                                }
                            }
                            
                            
                        }
                    }

                    if (buffTriggerType == EBuffTriggerType.SelectUnit)
                        return 0;

                    if (range1EnemyCount < range2EnemyCount)
                        return 1;
                    
                    if (range1EnemyCount > range2EnemyCount)
                        return -1;
                    
                    if (range1SoliderCount < range2SoliderCount)
                        return -1;
                    
                    if (range1SoliderCount > range2SoliderCount)
                        return 1;
                    
                    if (range1UsCount < range2UsCount)
                        return -1;
                    
                    if (range1UsCount > range2UsCount)
                        return 1;

                    return 0;

                });
                intersectDict.Add(battleUnitData.Idx, intersectList);
            }

            return intersectDict;

        }

        // private static List<List<int>> rangeNestList = new(8)
        // {
        //     new List<int>(8),
        //     new List<int>(8),
        //     new List<int>(8),
        //     new List<int>(8),
        //     new List<int>(8),
        //     new List<int>(8),
        //     new List<int>(8),
        //     new List<int>(8),
        // };
        public static int GetBlockDistance(Vector2Int block1, Vector2Int block2)
        {
            return Mathf.Abs(block1.x - block2.x) + Mathf.Abs(block1.y - block2.y);
        }
        
        public static List<List<int>> GetRangeNest(int gridPosIdx, EActionType actionType, bool inclueCenter = true)
        {
            // foreach (var list in rangeNestList)
            // {
            //     list.Clear();
            // }
            
            var rangeNestList = new List<List<int>>(8)
            {
                new List<int>(8),
                new List<int>(8),
                new List<int>(8),
                new List<int>(8),
                new List<int>(8),
                new List<int>(8),
                new List<int>(8),
                new List<int>(8),
            };

            var newActionType = actionType;
            
            if (actionType == EActionType.All)
            {
                
            }
            else if (actionType == EActionType.UnFullCurHPUnit)
            {
                
            }
            else if (actionType == EActionType.UnitMaxDirect)
            {
                
            }
            else if (actionType == EActionType.HeroDirect)
            {
                newActionType = EActionType.Direct82Long;

            }
            else if (actionType == EActionType.Cross_Long_Empty)
            {
                newActionType = EActionType.Cross2Long;
            }
            
            
            var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
            var idx = 0;
            if (Constant.Battle.ActionTypePoints.ContainsKey(newActionType))
            {
                foreach (var points in Constant.Battle.ActionTypePoints[newActionType])
                {
                    if (rangeNestList.Count < idx + 1)
                    {
                        rangeNestList.Add(new List<int>());
                    }

                    var list = new List<int>();
                    // var isMatch = true;
                    // if (actionType == EActionType.HeroDirect)
                    // {
                    //     isMatch = false;
                    // }
                
                    foreach (var point in points)
                    {
                        if(!inclueCenter && point == Vector2Int.zero)
                            continue;
                    
                        var targetCoord = coord + point;
                        var targetGridPosIdx = GridCoordToPosIdx(targetCoord);
                    
                        if (!GameUtility.InGridRange(targetCoord))
                            continue;
                    
                        if (actionType == EActionType.HeroDirect)
                        {
                            var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(
                                targetGridPosIdx);

                            // if (unit is BattleCoreEntity)
                            // {
                            //     isMatch = true;
                            // }
                        
                        }
                    
                    
                    
                        list.Add(targetGridPosIdx);
                    
                    }
                    rangeNestList[idx].AddRange(list);
                    // if (isMatch)
                    // {
                    //     rangeNestList[idx].AddRange(list);
                    // }
                    //
                    // if (actionType == EActionType.HeroDirect && isMatch)
                    // {
                    //     break;
                    // }
                

                    idx++;

                }
            }
            
        
            return rangeNestList;
        
        }

        public static bool InRange(int gridPosIdx, EActionType actionType, int targetGridPosIdx)
        {
            var coord = GridPosIdxToCoord(gridPosIdx);
            var targetCoord = GridPosIdxToCoord(targetGridPosIdx);

            var deltaCoord = targetCoord - coord;
            
            foreach (var points in Constant.Battle.ActionTypePoints[actionType])
            {
                foreach (var point in points)
                {
                    if (point == deltaCoord)
                        return true;
                }

            }

            return false;
        }



        public static ERelativePos? GetRelativePos(int centerGridPosIdx, int aroundGridPosIdx)
        {
            var centerCoord = GridPosIdxToCoord(centerGridPosIdx);
            var aroundCoord = GridPosIdxToCoord(aroundGridPosIdx);

            var deltaCoord = aroundCoord - centerCoord;
            if (deltaCoord.x != 0 && deltaCoord.y == 0)
            {
                deltaCoord.x /= Mathf.Abs(deltaCoord.x);
                return Constant.Battle.Coord2PosMap[deltaCoord];
            }
            else if (deltaCoord.y != 0 && deltaCoord.x == 0)
            {
                deltaCoord.y /= Mathf.Abs(deltaCoord.y);
                return Constant.Battle.Coord2PosMap[deltaCoord];
            }
            else if (Mathf.Abs(deltaCoord.x) ==  Mathf.Abs(deltaCoord.y))
            {
                deltaCoord.x /= Mathf.Abs(deltaCoord.x);
                deltaCoord.y /= Mathf.Abs(deltaCoord.y);
                return Constant.Battle.Coord2PosMap[deltaCoord];
            }
            
            return null;
            
            // if (!Constant.Battle.Coord2PosMap.ContainsKey(deltaCoord))
            // {
            //     return null;
            // }
            //
            // return Constant.Battle.Coord2PosMap[deltaCoord];
        }
        
        public static Vector3 GetMovePos(EUnitActionState unitActionState, List<int> moveGridPosIdxs, int idx, bool hasUnit)
        {
            var nextMoveGridPosIdx = -1;
            var beforeMoveGridPosIdx = -1;
            
            var moveGridPosIdx = moveGridPosIdxs[idx];
 
            var pos = GameUtility.GridPosIdxToPos(moveGridPosIdx);
            if (unitActionState == EUnitActionState.Fly || unitActionState == EUnitActionState.Rush)
                return pos;
            //var nextPos = pos;

            var relativePosLength = Enum.GetValues(typeof(ERelativePos)).Length;
            
            if (idx > 0  && (idx < moveGridPosIdxs.Count - 1 || hasUnit))
            {
                beforeMoveGridPosIdx = moveGridPosIdxs[idx - 1];
                
                if (hasUnit && idx == moveGridPosIdxs.Count - 1)
                {
                    var beforeCoord = GameUtility.GridPosIdxToCoord(beforeMoveGridPosIdx);
                    var coord = GameUtility.GridPosIdxToCoord(moveGridPosIdx);
                    var nextCoord = coord + (coord - beforeCoord);
                    nextMoveGridPosIdx = GameUtility.GridCoordToPosIdx(nextCoord);
                }
                else
                {
                    
                    nextMoveGridPosIdx = moveGridPosIdxs[idx + 1];
                    
                }
                
                
                //var nextPos = GameUtility.GridPosIdxToPos(nextMoveGridPosIdx);
                var moveGrids = BattleAreaManager.Instance.GetUnits(moveGridPosIdx);
                if (moveGrids.Count <= 0)
                    return pos;
                
                var nextRelativePos = GameUtility.GetRelativePos(moveGridPosIdx, nextMoveGridPosIdx);
                if (nextRelativePos == null)
                    return pos;
                
                var relativePos = GameUtility.GetRelativePos(moveGridPosIdx, beforeMoveGridPosIdx);
                if (relativePos == null)
                    return pos;
                
                var deltaRelativePos = ERelativePos.Left;
                if ((int) nextRelativePos < (int) relativePos)
                {
                    deltaRelativePos = (ERelativePos)(relativePosLength -
                                                      ((int) relativePos - (int) nextRelativePos));
                }
                else
                {
                    deltaRelativePos = (ERelativePos)(nextRelativePos - relativePos);
                }

                var target = (int)Constant.Battle.UnitPassRoute[deltaRelativePos] + (int)relativePos;
                target %= relativePosLength;

                var deltaPos = Constant.Battle.EPos2CoordMap[(ERelativePos) target];
                pos = new Vector3(pos.x + deltaPos.x * Constant.Area.GridLength.x / 3.0f, pos.y,
                    pos.z + deltaPos.y * Constant.Area.GridLength.y / 3.0f);
                 
            }

            return pos;
        }


        // public static ELinkID FuneIDToLinkID(EFuneID funeID)
        // {
        //     return Enum.Parse<ELinkID>(Enum.GetName(typeof(EFuneID), funeID));
        // }
        
        // public static ELinkID CardIDToLinkID(ECardID cardID)
        // {
        //     return Enum.Parse<ELinkID>(Enum.GetName(typeof(ECardID), cardID));
        // }
        
        public static ELinkID BuffIDToLinkID(EBuffID buffID)
        {
            return Enum.Parse<ELinkID>(Enum.GetName(typeof(EBuffID), buffID));
        }
        
        public static int GetUnitCount(Data_GamePlay gamePlayData, EUnitCamp selfCamp, ERelativeCamp relativeCamp)
        {
            var count = 0;

            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                if (relativeCamp == ERelativeCamp.Us && kv.Value.UnitCamp == selfCamp)
                {
                    count += 1;
                }
                else if (relativeCamp == ERelativeCamp.Enemy && kv.Value.UnitCamp != selfCamp)
                {
                    count += 1;
                }
                else if (relativeCamp == ERelativeCamp.Third)
                {
                    count += 1;
                }
            }

            return count;
        }

        public static bool IsSubCurHPTrigger(TriggerData triggerData)
        {
            if (triggerData == null)
                return false;
            
            return  triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                    triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                    triggerData.Value < 0;

        }
        
        public static bool IsSubCurHPBuffValue(BuffValue buffValue)
        {
            if (buffValue == null)
                return false;
            
            return  buffValue.BuffData.BuffValueType == EBuffValueType.Atrb &&
                    buffValue.BuffData.UnitAttribute == EUnitAttribute.HP &&
                    buffValue.ValueList[0] < 0;

        }
        
        public static bool IsAddCurHPTrigger(TriggerData triggerData)
        {
            if (triggerData == null)
                return false;
            
            return  triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                    triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                    triggerData.Value > 0;

        }
        
        public static bool IsAddCurHPBuffValue(BuffValue buffValue)
        {
            if (buffValue == null)
                return false;
            
            return  buffValue.BuffData.BuffValueType == EBuffValueType.Atrb &&
                    buffValue.BuffData.UnitAttribute == EUnitAttribute.HP &&
                    buffValue.ValueList[0] < 0;

        }

        public static bool ContainRoundState(Data_GamePlay gamePlay, EBuffID buffID)
        {
            foreach (var kv in gamePlay.BattleData.BattlePlayerDatas)
            {
                if (kv.Value.BattleBuffs.Contains(buffID))
                    return true;
            }

            return false;
        }

        public static bool CheckUnitCamp(List<ERelativeCamp> relativeCamps, EUnitCamp unitCamp1, EUnitCamp unitCamp2)
        {
            if (relativeCamps.Contains(ERelativeCamp.Us) && unitCamp1 == unitCamp2)
                return true;
            
            if (relativeCamps.Contains(ERelativeCamp.Enemy) && unitCamp1 != unitCamp2)
                return true;

            return false;

        }
        
        // public static float GetBuffValue(string value, int effectUnitIdx = -1)
        // {
        //     if (float.TryParse(value, out float floatValue))
        //     {
        //         return floatValue;
        //     }
        //     else if(Enum.TryParse(value, out EValueType valueType))
        //     {
        //         switch (valueType)
        //         {
        //             case EValueType.UnitHP:
        //                 var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
        //                 if (effectUnit != null)
        //                     return effectUnit.CurHP;
        //                 break;
        //             case EValueType.EffectUnitAttack:
        //                 break;
        //             case EValueType.Empty:
        //                 break;
        //             case EValueType.HandCardCount:
        //                 break;
        //             case EValueType.UnitCount:
        //                 break;
        //             case EValueType.UsBuffCount:
        //                 break;
        //             case EValueType.EnemyDeBuffCount:
        //                 break;
        //             default:
        //                 throw new ArgumentOutOfRangeException();
        //         }
        //
        //         return 0;
        //     }
        //
        //     return 0;
        // }

        private static AStarSearch.EPointType[,] cacheSearchMaps = new AStarSearch.EPointType[Constant.Area.GridSize.x, Constant.Area.GridSize.y];
        
        public static List<int> GetPaths(Dictionary<int, EGridType> gridTypes, int startPosIdx, int endPosIdx, bool isQblique = true,
            bool isIgnoreCorner = true)
        {
            for (int i = 0; i < Constant.Area.GridSize.x; i++)
            {
                for (int j = 0; j < Constant.Area.GridSize.y; j++)
                {
                    cacheSearchMaps[i, j] = AStarSearch.EPointType.Empty;
                    var gridPosIdx = GameUtility.GridCoordToPosIdx(new Vector2Int(i, j));
                    if (gridTypes.ContainsKey(gridPosIdx))
                    {
                        switch (gridTypes[gridPosIdx])
                        {
                            case EGridType.Empty:
                                break;
                            case EGridType.Obstacle:
                                cacheSearchMaps[i, j] = AStarSearch.EPointType.Obstacle;
                                break;
                            case EGridType.Unit:
                                cacheSearchMaps[i, j] = AStarSearch.EPointType.Pass;
                                break;
                            case EGridType.TemporaryUnit:
                                cacheSearchMaps[i, j] = AStarSearch.EPointType.Pass;
                                break;

                        }
                    }
                }    
            }
            

            var paths = AStarSearch.GetPathList(cacheSearchMaps, GridPosIdxToCoord(startPosIdx), GridPosIdxToCoord(endPosIdx), isQblique, isIgnoreCorner);

            var retPaths = new List<int>(paths.Count);
            
            // if (paths.Count >= 2)
            // {
            //     var coord = paths[paths.Count - 2];
            //     if (cacheSearchMaps[coord.x, coord.y] != AStarSearch.EPointType.Empty)
            //     {
            //         return retPaths;
            //     }
            // }
            
            

            for (int i = 0; i < paths.Count; i++)
            {
                retPaths.Add(GridCoordToPosIdx(paths[i]));
                if (i >= 1 && i < paths.Count - 1)
                {
                    var prePoint = paths[i - 1];
                    var curPoint = paths[i];
                    var nextPoint = paths[i + 1];

                    if (nextPoint - curPoint != curPoint - prePoint)
                    {
                        break;
                    }
                }

            }

            if (retPaths.Count > 0)
            {
                var endCoord = GridPosIdxToCoord(retPaths[retPaths.Count - 1]);
                if (cacheSearchMaps[endCoord.x, endCoord.y] != AStarSearch.EPointType.Empty)
                {
                    retPaths.RemoveAt(retPaths.Count - 1);
                }
            }
            

            return retPaths;

        }

        public static List<T> StringToEnum<T>(List<string> strs)
        {
            var ret = new List<T>();
            foreach (var str in strs)
            {
                var t = (T)Enum.Parse(typeof(T), str);
                ret.Add(t);

            }

            return ret;
        }

        public static ERelativeCamp GetRelativeCamp(EUnitCamp camp1, EUnitCamp camp2)
        {
            if (camp1 == camp2)
                return ERelativeCamp.Us;

            if (camp1 != camp2)
            {
                if (camp2 == EUnitCamp.Third)
                    return ERelativeCamp.Third;

                return ERelativeCamp.Enemy;
            }

            return ERelativeCamp.Empty;
        }
        
        public static int GetEndPosIdx(int startPosIdx, Vector2Int direct, int dis)
        {
            var idx = 0;
            var endCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            direct = GameUtility.GetDirect(direct);
            while (true)
            {
                idx++;
                endCoord += direct;
                if (!GameUtility.InGridRange(endCoord))
                {
                    endCoord -= direct;
                    break;
                }
                if (idx >= dis)
                {
                    break;
                }
                
                    
            }

            return GameUtility.GridCoordToPosIdx(endCoord);
        }

        public static List<int> GetMoveIdxs(int startIdx, int endIdx)
        {
            var moveIdxs = new List<int>();
            var startCoord = GameUtility.GridPosIdxToCoord(startIdx);
            var endCoord = GameUtility.GridPosIdxToCoord(endIdx);
            
            moveIdxs.Add(startIdx);

            var deltaCoord = endCoord - startCoord;
            deltaCoord.x = deltaCoord.x < 0 ? -1 : deltaCoord.x > 0 ? 1 : 0;
            deltaCoord.y = deltaCoord.y < 0 ? -1 : deltaCoord.y > 0 ? 1 : 0;

            var moveCoord = startCoord;

            var idx = 0;
            while (true)
            {
                
                moveCoord += deltaCoord;
                moveIdxs.Add(GameUtility.GridCoordToPosIdx(moveCoord));
                if (moveCoord == endCoord)
                {
                    break;
                }

                if (idx > 10)
                    break;
                idx++;
            }

            return moveIdxs;

        }
        
        public static Vector3 GetBetweenPoint(Vector3 start, Vector3 end, float percent=0.5f)
        {
            Vector3 normal = (end - start).normalized;
            float distance = Vector3.Distance(start, end);
            return normal * (distance * percent) + start;
        }

        public static List<Vector2Int> GetRelatedCoords(EActionType actionType, int gridPosIdx1, int gridPosIdx2)
        {
            var pointList = new List<List<Vector2Int>>();
            
            List<Vector2Int> coord1s = new List<Vector2Int>();
            //List<Vector2Int> coord2s = new List<Vector2Int>();
            
            var actionTypeStr = actionType.ToString();
            var delta = GameUtility.GridPosIdxToCoord(gridPosIdx1) - GameUtility.GridPosIdxToCoord(gridPosIdx2);

            if (actionTypeStr.Contains("Horizontal"))
            {
                if (delta.x == 0 && delta.y != 0)
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(-1, 0));
                        coord1s.Add(new Vector2Int(1, 0));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(-i - 1, 0));
                            coord1s.Add(new Vector2Int(i + 1, 0));
                        }
                    }
                }
                else if (delta.x != 0 && delta.y == 0)
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(0, -1));
                        coord1s.Add(new Vector2Int(0, 1));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(0, -i - 1));
                            coord1s.Add(new Vector2Int(0, i + 1));
                        }
                    }
                }
                else if ((delta.x > 0 && delta.y < 0) || (delta.x < 0 && delta.y > 0))
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(1, 1));
                        coord1s.Add(new Vector2Int(-1, -1));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(i+1, i+1));
                            coord1s.Add(new Vector2Int(-i-1, -i-1));
                        }
                    }
                }
                else if ((delta.x > 0 && delta.y > 0) || (delta.x < 0 && delta.y < 0))
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(1, -1));
                        coord1s.Add(new Vector2Int(-1, 1));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(i+1, -i-1));
                            coord1s.Add(new Vector2Int(-i-1, i+1));
                        }
                    }
                }
            }
            else if (actionTypeStr.Contains("Vertical"))
            {
                if (delta.x == 0 && delta.y != 0)
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(0, -1));
                        coord1s.Add(new Vector2Int(0, 1));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(0, -i-1));
                            coord1s.Add(new Vector2Int(0, i+1));
                        }
                    }
                }
                else if (delta.x != 0 && delta.y == 0)
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(-1, 0));
                        coord1s.Add(new Vector2Int(1, 0));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(-i-1, 0));
                            coord1s.Add(new Vector2Int(i+1, 0));
                        }
                    }
                    
                }
                else if ((delta.x > 0 && delta.y < 0) || (delta.x < 0 && delta.y > 0))
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(1, -1));
                        coord1s.Add(new Vector2Int(-1, 1));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(i+1, -i-1));
                            coord1s.Add(new Vector2Int(-i-1, i+1));
                        }
                    }
                }
                else if ((delta.x > 0 && delta.y > 0) || (delta.x < 0 && delta.y < 0))
                {
                    if (actionTypeStr.Contains("Short"))
                    {
                        coord1s.Add(new Vector2Int(1, 1));
                        coord1s.Add(new Vector2Int(-1, -1));
                    }
                    else if (actionTypeStr.Contains("Long") || actionTypeStr.Contains("Extends"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            coord1s.Add(new Vector2Int(i+1, i+1));
                            coord1s.Add(new Vector2Int(-i-1, -i-1));
                        }
                    }
                    
                    
                }
            }
            else if (actionType == EActionType.LineExtend)
            {
                var actionUnitCoord = GameUtility.GridPosIdxToCoord(gridPosIdx1);
                var effectUnitCoord = GameUtility.GridPosIdxToCoord(gridPosIdx2);
                var direct = actionUnitCoord - effectUnitCoord;
                direct = GameUtility.GetDirect(direct);

                for (int i = 0; i < Constant.Area.GridSize.x; i++)
                {
                    var endPosIdx = GameUtility.GetEndPosIdx(gridPosIdx1, direct, i + 1);
                    if (endPosIdx != gridPosIdx1)
                    {
                        var endPosUnit = GameUtility.GetUnitByGridPosIdx(endPosIdx);
                        if (endPosUnit != null)
                        {
                            coord1s.Add(GameUtility.GridPosIdxToCoord(endPosUnit.GridPosIdx));
                        }
                        
                        // if (endPosUnit != null)
                        // {
                        //     var isEndPosEnemy = BattleFightManager.Instance.IsEnemy(gridPosIdx1, endPosUnit.Idx);
                        //     
                        //     if (isEndPosEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                        //     {
                        //         coord1s.Add(GameUtility.GridPosIdxToCoord(endPosUnit.GridPosIdx));
                        //         
                        //         
                        //         break;
                        //     }
                        //     else if (!isEndPosEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                        //     {
                        //         coord1s.Add(GameUtility.GridPosIdxToCoord(endPosUnit.GridPosIdx));
                        //         break;
                        //     }
                        //
                        // }
                    }
                                
                }
            }
            //pointList.Add(coord1s);
            //pointList.Add(coord2s);

            return coord1s;
        }


        public static List<Vector2Int> GetRelatedHorizontalCoords(Vector2Int direct, Vector2Int targetCoord)
        {
            var coords = new List<Vector2Int>();
            if (direct == new Vector2Int(1, 1))
            {
                coords.Add(new Vector2Int(-1, 1));
                coords.Add(new Vector2Int(1, -1));
            }
            else if (direct == new Vector2Int(-1, 1))
            {
                coords.Add(new Vector2Int(-1, -1));
                coords.Add(new Vector2Int(1, 1));
            }
            else if (direct == new Vector2Int(-1, -1))
            {
                coords.Add(new Vector2Int(-1, 1));
                coords.Add(new Vector2Int(1, -1));
            }
            else if (direct == new Vector2Int(1, -1))
            {
                coords.Add(new Vector2Int(-1, -1));
                coords.Add(new Vector2Int(1, 1));
            }
            else if (direct == new Vector2Int(0, 1))
            {
                coords.Add(new Vector2Int(-1, 0));
                coords.Add(new Vector2Int(1, 0));
            }
            else if (direct == new Vector2Int(-1, 0))
            {
                coords.Add(new Vector2Int(0, -1));
                coords.Add(new Vector2Int(0, 1));
            }
            else if (direct == new Vector2Int(0, -1))
            {
                coords.Add(new Vector2Int(-1, 0));
                coords.Add(new Vector2Int(1, 0));
            }
            else if (direct == new Vector2Int(1, 0))
            {
                coords.Add(new Vector2Int(0, -1));
                coords.Add(new Vector2Int(0, 1));
            }

            return coords;

        }
        
        public static EGridType GetGridType(int gridPosIdx, bool isBattleData)
        {
            if (isBattleData)
            {
                if (BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GridTypes.ContainsKey(gridPosIdx))
                {
                    return BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GridTypes[gridPosIdx];
                }

            }
            else
            {
                if (BattleAreaManager.Instance.TmpUnitEntity != null &&
                    BattleAreaManager.Instance.TmpUnitEntity.GridPosIdx == gridPosIdx)
                {
                    return EGridType.TemporaryUnit;
                }
                else if (BattleManager.Instance.BattleData.GridTypes.ContainsKey(gridPosIdx))
                {
                    return BattleManager.Instance.BattleData.GridTypes[gridPosIdx];
                }
                
            }
            
            
            

            return EGridType.Empty;
        }
        
        // public static Vector2 WorldToUGUIPosition(Canvas canvas, Camera worldCamera, Camera uiCamera, Vector3 worldPosition)
        // {
        //     // 将世界坐标转换为屏幕坐标
        //     var screenPosition =
        //         worldCamera.WorldToScreenPoint(
        //             worldPosition); //RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
        //     // 将屏幕坐标转换为 UGUI 坐标
        //     //var uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        //     RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        //         screenPosition, uiCamera, out var localPoint);
        //     return localPoint;
        // }
    }
}