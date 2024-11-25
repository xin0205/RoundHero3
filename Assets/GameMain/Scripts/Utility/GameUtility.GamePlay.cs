

using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

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
            foreach (var value in drCard.Values1)
            {
                var val = Mathf.Abs(BattleBuffManager.Instance.GetBuffValue(value));
                if (val != 0)
                {
                    values.Add(val);
                }
               
            }
            
            foreach (var value in drCard.Values2)
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
                fValues.Add(GetBuffValue(value));
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
        
        public static List<int> GetRange(int gridPosIdx, string actionType, string selfCampStr, string unitCampStr = null, bool isBattleData = true)
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
            
            EUnitCamp? selfUnitCamp = null;
            if (selfCampStr != null)
            {
                selfUnitCamp = GameUtility.GetEnum<EUnitCamp>(selfCampStr);
            }

            
            
            return GetRange(gridPosIdx, nActionType, selfUnitCamp, unitCamps, isBattleData);

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
                : BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx)?.BattleUnit;

            // if (unit != null)
            // {
            //     cachePosToUnits.Add(gridPosIdx, unit);
            // }

            return unit;
        }
        public static Data_BattleUnit GetUnitDataByID(int unitID, bool isBattleData = true)
        {
            // if (cachePosToUnits.ContainsKey(gridPosIdx))
            //     return cachePosToUnits[gridPosIdx];
            
            var unit = isBattleData
                ? BattleFightManager.Instance.GetUnitByID(unitID)
                : BattleUnitManager.Instance.GetUnitByID(unitID)?.BattleUnit;

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
                : BattleUnitManager.Instance.GetUnitByGridPosIdxMoreCamps(gridPosIdx, selfUnitCamp, unitCamps)?.BattleUnit;
        
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
                
                if (kv.Value.ID == unitID)
                {
                    return kv.Value;
                }
            }

            return null;

        }

        public static List<int> GetRange(int gridPosIdx, EActionType actionType,
            EUnitCamp? selfUnitCamp = null, List<ERelativeCamp>? unitCamps = null,
            bool isBattleData = true)
        {
            retGetRange.Clear();
            return GetRange(gridPosIdx, actionType, retGetRange,
                selfUnitCamp, unitCamps, isBattleData);
        }
        
        
        private static List<List<int>> rangeList = new (50);
        private static Dictionary<Vector2Int, Vector2Int> cacheCoords = new ();
        
        public static List<int> GetRange(int gridPosIdx, EActionType actionType, List<int> retGetRange, EUnitCamp? selfUnitCamp = null, List<ERelativeCamp>? unitCamps = null,
            bool isBattleData = true)
        {

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
                    
                    if(kv.Value.CurHP <= 0)
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Us) && !unitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != selfUnitCamp)
                        continue;
                    
                    if (unitCamps.Contains(ERelativeCamp.Enemy) && !unitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == selfUnitCamp)
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
                    
                    if(kv.Value.CurHP <= 0)
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
                
                var direct8RangeNest = GameUtility.GetRangeNest(gridPosIdx, EActionType.Direct8, false);
                
                foreach (var list in direct8RangeNest)
                {
                    var unitCount = 0;
                    foreach (var posIdx in list)
                    {
                        var unit = GetUnitByGridPosIdxMoreCamps(posIdx, isBattleData, selfUnitCamp, unitCamps);
                        
                        if(unit == null)
                            continue;
                        
                        if(unit.CurHP <= 0)
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
                
                rangeList.Add(direct8RangeNest[maxIdx]);
            }
            // else if (actionType == EActionType.UnitMaxXExtend)
            // {
            //     var maxCount = 0;
            //     var range = GameUtility.GetRange(gridPosIdx, actionType);
            //     foreach (var rangeGridPosIdx in range)
            //     {
            //         var unit = GetUnitByGridPosIdxMoreCamps(rangeGridPosIdx, isBattleData, selfUnitCamp, unitCamps);
            //             
            //         if(unit == null)
            //             continue;
            //         
            //         if(unit.CurHP <= 0)
            //             continue;
            //         
            //         //rangeList.Add();
            //     }
            // }
            // else if (actionType == EActionType.Direct)
            // {
            //     
            //     foreach (var list in direct8RangeNest)
            //     {
            //         foreach (var posIdx in list)
            //         {
            //             var unit = GetUnitByGridPosIdx(posIdx, isBattleData);
            //
            //             if (unit != null && unit.UnitRole == EUnitRole.Hero && heroInRangeTrigger)
            //             {
            //                 rangeList.Add(list);
            //                 break;
            //             }
            //         }
            //     }
            
                // foreach (var range in rangeList)
                // {
                //     for (int i = range.Count - 1; i >= 0; i--)
                //     {
                //         var unit = GetUnitByGridPosIdx(range[i], isBattleData);
                //
                //         if (unit == null || unit.CurHP <= 0)
                //         {
                //             range.RemoveAt(i);
                //         }
                //     }
                // }
            //}
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

                        if (unitCamps == null)
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
                            
                                 if (unit.CurHP <= 0)
                                     continue;
                                 
                                 if(unitCamps.Contains(ERelativeCamp.Us) && !unitCamps.Contains(ERelativeCamp.Enemy) && selfUnitCamp != unit.UnitCamp)
                                     continue;
                            
                                 if(unitCamps.Contains(ERelativeCamp.Enemy)  && !unitCamps.Contains(ERelativeCamp.Us) && selfUnitCamp == unit.UnitCamp)
                                     continue;
                                 
                                 retGetRange.Add(posIdx);
                             }
                             else
                             {
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
                                 
                                 if (unit != null && unit.CurHP > 0)
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
        public static int GetActionGridPosIdx(int actionGridPosIdx, EActionType attackType, bool isBattleData = true)
        {
            if (!ActionTypeMaps.ContainsKey(attackType))
            {
                ActionTypeMaps.Add(attackType, Enum.GetName(typeof(EActionType), attackType));
            }
        
            var isExtendActionType = ActionTypeMaps[attackType].Contains("Extend");
            var isOblique = ActionTypeMaps[attackType].Contains("Direct8");

            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionGridPosIdx);
            var heroCoord = GameUtility.GridPosIdxToCoord(HeroManager.Instance.BattleHeroData.GridPosIdx);

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
                    var unit = GetUnitByGridPosIdx(stepGridPosIdx);
                    if (unit != null && unit.CurHP > 0)
                    {
                        return -1;
                    }

                }

            }


        }
        
        public static List<int> GetActionGridPosIdxs(int gridPosIdx, EActionType moveType, EActionType attackType,
            List<int> moveRange, List<int> heroHurtRange, bool isBattleData = true)
        {
            
            GetRange(gridPosIdx, moveType, moveRange, null, null, isBattleData);
            
            GetRange(HeroManager.Instance.BattleHeroData.GridPosIdx, attackType, heroHurtRange, null, null,
                isBattleData);

            var intersectList = moveRange.Intersect(heroHurtRange).ToList();

            

            return intersectList;

        }

        private static List<List<int>> rangeNestList = new(8)
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
        public static List<List<int>> GetRangeNest(int gridPosIdx, EActionType actionType, bool inclueCenter = true)
        {
            foreach (var list in rangeNestList)
            {
                list.Clear();
            }
            
            
            var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
            var idx = 0;
            foreach (var points in Constant.Battle.ActionTypePoints[actionType])
            {
                if (rangeNestList.Count < idx + 1)
                {
                    rangeNestList.Add(new List<int>());
                }
                
                foreach (var point in points)
                {
                    if(!inclueCenter && point == Vector2Int.zero)
                        continue;
                    
                    var targetCoord = coord + point;
                    if (!GameUtility.InGridRange(targetCoord))
                        continue;
                    
                    rangeNestList[idx].Add(GridCoordToPosIdx(targetCoord));
                }

                idx++;

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
            if (!Constant.Battle.Coord2PosMap.ContainsKey(deltaCoord))
            {
                return null;
            }
            
            return Constant.Battle.Coord2PosMap[deltaCoord];
        }
        
        public static Vector3 GetMovePos(EUnitActionState unitActionState, List<int> moveGridPosIdxs, int i)
        {
            var nextMoveGridPosIdx = -1;
            var beforeMoveGridPosIdx = -1;
            
            var moveGridPosIdx = moveGridPosIdxs[i];
 
            var pos = GameUtility.GridPosIdxToPos(moveGridPosIdx);
            if (unitActionState == EUnitActionState.Fly)
                return pos;
            //var nextPos = pos;

            var relativePosLength = Enum.GetValues(typeof(ERelativePos)).Length;
            
            if (i > 0  && i < moveGridPosIdxs.Count - 1)
            {
                nextMoveGridPosIdx = moveGridPosIdxs[i + 1];
                beforeMoveGridPosIdx = moveGridPosIdxs[i - 1];
                var nextPos = GameUtility.GridPosIdxToPos(nextMoveGridPosIdx);
                var moveGrids = BattleAreaManager.Instance.GetUnits(moveGridPosIdx);
                if (moveGrids.Count > 0)
                {
                     var nextRelativePos = GameUtility.GetRelativePos(moveGridPosIdx, nextMoveGridPosIdx);
                     if (nextRelativePos != null)
                     {
                         var relativePos = GameUtility.GetRelativePos(moveGridPosIdx, beforeMoveGridPosIdx);
                         if (relativePos != null)
                         {

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
                         
                     }
                 
                }
                 
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

        public static bool ContainRoundState(Data_GamePlay gamePlay, EBuffID buffID)
        {
            foreach (var kv in gamePlay.BattleData.BattlePlayerDatas)
            {
                if (kv.Value.RoundBuffs.Contains(buffID))
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
        
        public static float GetBuffValue(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                return floatValue;
            }
            else if(Enum.TryParse(value, out EValueType valueType))
            {
                switch (valueType)
                {
                    case EValueType.EffectUnitAttack:
                        break;
                    case EValueType.Empty:
                        break;
                    case EValueType.HandCardCount:
                        break;
                    case EValueType.UnitCount:
                        break;
                    case EValueType.UsBuffCount:
                        break;
                    case EValueType.EnemyDeBuffCount:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return 1;
            }

            return 0;
        }

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
        
    }
}