using System.Collections.Generic;
using System.Linq;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public interface ITriggerActionData
    {
    }

    public class TriggerActionTriggerData : ITriggerActionData
    {
        public TriggerData TriggerData;

    }

    public class TriggerActionMoveData : ITriggerActionData
    {

        public MoveUnitData MoveUnitData;
    }


    public class BattleBulletManager : Singleton<BattleBulletManager>
    {
        // public Dictionary<int, GameFrameworkMultiDictionary<int, ITriggerActionData>> TriggerActionDatas2 =
        //     new();

        // public Dictionary<int, ActionData> TriggerActionDatas =
        //     new();
        
        public Dictionary<int, Dictionary<int, TriggerCollection>> TriggerCollections =
            new();
        
        public void AddTriggerCollection(TriggerCollection triggerCollection)
        {

            if (!TriggerCollections.ContainsKey(triggerCollection.ActionUnitIdx))
            {
                TriggerCollections.Add(triggerCollection.ActionUnitIdx, new Dictionary<int, TriggerCollection>());

            }
            
            TriggerCollections[triggerCollection.ActionUnitIdx].Add(triggerCollection.EffectTagIdx,
                triggerCollection.Copy());

        }
        
        public void AddTriggerCollection(ActionData actionData)
        {

            foreach (var kv in actionData.TriggerDataDict)
            {
                if (!TriggerCollections.ContainsKey(kv.Value.ActionUnitIdx))
                {
                    TriggerCollections.Add(kv.Value.ActionUnitIdx, new Dictionary<int, TriggerCollection>());

                }
            
                TriggerCollections[kv.Value.ActionUnitIdx].Add(kv.Value.EffectTagIdx,
                    kv.Value.Copy());
            }
            
            

        }
        
        public Dictionary<int, TriggerCollection> GetTriggerCollectionDict(int actionUnitIdx)
        {

            if (TriggerCollections.ContainsKey(actionUnitIdx))
            {
                return TriggerCollections[actionUnitIdx];
            }

            return null;
        }
        
        public TriggerCollection GetTriggerCollection(int actionUnitIdx, int effectUnitIdx)
        {

            if (TriggerCollections.ContainsKey(actionUnitIdx))
            {
                if (TriggerCollections[actionUnitIdx].ContainsKey(effectUnitIdx))
                {

                    return TriggerCollections[actionUnitIdx][effectUnitIdx];
                }
            }

            return null;
        }
        
        public void UseTriggerCollection(int actionUnitIdx, int effectUnitIdx)
        {
            if (!TriggerCollections.ContainsKey(actionUnitIdx))
            {
                return;
            }
            
            if (!TriggerCollections[actionUnitIdx].ContainsKey(effectUnitIdx))
            {

                return;
            }

            TriggerCollections[actionUnitIdx][effectUnitIdx].IsTrigger = true;

            foreach (var triggerData in TriggerCollections[actionUnitIdx][effectUnitIdx].TriggerDatas)
            {
                UseTriggerData(triggerData);
            }
            
            foreach (var kv in TriggerCollections[actionUnitIdx][effectUnitIdx].MoveData.MoveUnitDatas)
            {
                UseMoveActionData(kv.Value);
            }
            
            ClearTriggerCollection();
        }
        
        public void UseTriggerCollection(int actionUnitIdx)
        {
            if (!TriggerCollections.ContainsKey(actionUnitIdx))
            {
                return;
            }

            foreach (var kv in TriggerCollections[actionUnitIdx])
            {
                kv.Value.IsTrigger = true;
                foreach (var triggerData in kv.Value.TriggerDatas)
                {
                    UseTriggerData(triggerData);
                }
            }
            
            foreach (var kv in TriggerCollections[actionUnitIdx])
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    UseMoveActionData(kv2.Value);
                }
                
                
            }

            ClearTriggerCollection();
        }
        
        public void UseTriggerData(TriggerData triggerData)
        {
            if (triggerData.IsTrigger)
            {
                return;
            }
            
            triggerData.IsTrigger = true;
            BattleFightManager.Instance.TriggerAction(triggerData.Copy());
            //ClearTriggerData();
        }
        
        public void UseMoveActionData(MoveUnitData moveUnitData)
        {
            if(moveUnitData.IsTrigger)
                return;
            
            moveUnitData.IsTrigger = true;
            var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(moveUnitData.UnitIdx);

            if (moveUnitData.UnitActionState == EUnitActionState.Fly)
            {
                effectUnitEntity.Fly(moveUnitData.MoveActionData.Copy());
            }
            else if (moveUnitData.UnitActionState == EUnitActionState.Run)
            {
                effectUnitEntity.Run(moveUnitData.MoveActionData.Copy());
            }
            else if (moveUnitData.UnitActionState == EUnitActionState.Rush)
            {
                effectUnitEntity.Rush(moveUnitData.MoveActionData.Copy());
            }
            else if (moveUnitData.UnitActionState == EUnitActionState.Throw)
            {
                effectUnitEntity.Rush(moveUnitData.MoveActionData.Copy());
            }

            //moveUnitData.MoveActionData.Clear();
            //ClearTriggerData();
        }
        
        public void ClearTriggerCollection()
        {
            var keys = TriggerCollections.Keys.ToList();
            var values = TriggerCollections.Values.ToList();
            for (int i = TriggerCollections.Count - 1; i >= 0; i--)
            {
                var keys2 = values[i].Keys.ToList();
                var values2 = values[i].Values.ToList();
                for (int j = values2.Count - 1; j >= 0; j--)
                {
                    var triggerCollection = values2[j];
                    if (triggerCollection.IsTrigger)
                    {
                        TriggerCollections[keys[i]].Remove(keys2[j]);
                    }
                }

                if (TriggerCollections[keys[i]].Count <= 0)
                {
                    TriggerCollections.Remove(keys[i]);
                }
            }
        }
        
        // public void ClearTriggerData()
        // {
        //     
        //     // var keys = TriggerActionDatas2.Keys.ToList();
        //     // var values = TriggerActionDatas2.Values.ToList();
        //     //
        //     // for (int i = values.Count - 1; i >= 0; i--)
        //     // {
        //     //     var list = values[i].ToList();
        //     //     
        //     //     for (int j = list.Count - 1; j >= 0; j--)
        //     //     {
        //     //         var list2 = list[j].Value.ToList();
        //     //         
        //     //         for (int k = list2.Count - 1; k >= 0; k--)
        //     //         {
        //     //             
        //     //             if (list2[k] is TriggerActionTriggerData triggerActionTriggerData)
        //     //             {
        //     //                 if (triggerActionTriggerData.TriggerData.IsTrigger)
        //     //                 {
        //     //                     TriggerActionDatas2[keys[i]].Remove(list[j].Key, triggerActionTriggerData);
        //     //                 }
        //     //             }
        //     //             
        //     //             if (list2[k] is TriggerActionMoveData triggerActionMoveData)
        //     //             {
        //     //                 if (triggerActionMoveData.MoveUnitData.IsTrigger)
        //     //                 {
        //     //                     TriggerActionDatas2[keys[i]].Remove(list[j].Key, triggerActionMoveData);
        //     //                 }
        //     //             }
        //     //         }
        //     //         
        //     //     }
        //     // }
        //     
        //     // foreach (var kv in TriggerActionDatas)
        //     // {
        //     //     var list = kv.Value.ToList();
        //     //     for (int i = list.Count - 1; i >= 0; i--)
        //     //     {
        //     //         var list2 = list[i].Value.ToList();
        //     //         for (int j = list2.Count - 1; j >= 0; j--)
        //     //         {
        //     //             var triggerActionData = list2[j];
        //     //
        //     //             if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
        //     //             {
        //     //                 if (triggerActionTriggerData.TriggerData.IsTrigger)
        //     //                 {
        //     //                     TriggerActionDatas[kv.Key].Remove(list[i].Key, triggerActionTriggerData);
        //     //                 }
        //     //             }
        //     //     
        //     //         }
        //     //     }
        //     //
        //     // }
        // }
        //
        // public void ClearMoveData(int actionUnitIdx)
        // {
        //     // if (TriggerActionDatas2.ContainsKey(actionUnitIdx))
        //     // {
        //     //     TriggerActionDatas2[actionUnitIdx].Clear();
        //     // }
        // }

        public void Destory()
        {
            TriggerCollections.Clear();
            //TriggerActionDatas2.Clear();
        }
        
        // public void AddActionData(ActionData actionData)
        // {
        //
        //     if (!TriggerActionDatas.ContainsKey(actionData.ActionUnitIdx))
        //     {
        //         TriggerActionDatas.Add(actionData.ActionUnitIdx,
        //             actionData);
        //     }
        //
        // }
        //
        //
        //
        // public ActionData GetActionData(int actionUnitIdx)
        // {
        //
        //     if (TriggerActionDatas.ContainsKey(actionUnitIdx))
        //     {
        //         return TriggerActionDatas[actionUnitIdx];
        //     }
        //
        //     return null;
        // }
        //
        // public void UseActionData(int actionUnitIdx)
        // {
        //     if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
        //     {
        //         return;
        //     }
        //
        //     foreach (var kv in TriggerActionDatas[actionUnitIdx].TriggerDataDict)
        //     {
        //         foreach (var triggerData in kv.Value.TriggerDatas)
        //         {
        //             UseTriggerData(triggerData);
        //         }
        //     }
        //     
        //     foreach (var kv in TriggerActionDatas[actionUnitIdx].TriggerDataDict)
        //     {
        //         foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
        //         {
        //             UseMoveActionData(kv2.Value);
        //         }
        //     }
        // }
        //
        // public void UseActionData(int actionUnitIdx, int effectUnitIdx)
        // {
        //
        //     if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
        //     {
        //         return;
        //     }
        //     
        //     if (!TriggerActionDatas[actionUnitIdx].TriggerDataDict.ContainsKey(effectUnitIdx))
        //     {
        //         return;
        //     }
        //     
        //     foreach (var triggerData in TriggerActionDatas[actionUnitIdx].TriggerDataDict[effectUnitIdx].TriggerDatas)
        //     {
        //         UseTriggerData(triggerData);
        //         
        //     }
        //     
        //     foreach (var kv in TriggerActionDatas[actionUnitIdx].TriggerDataDict[effectUnitIdx].MoveData.MoveUnitDatas)
        //     {
        //         UseMoveActionData(kv.Value);
        //     }
        //
        //
        // }
        
        // public void AddTriggerData(TriggerData triggerData)
        // {
        //     //var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
        //     //effectUnit.ActionUnitID = triggerData.ActionUnitID;
        //
        //
        //     if (!TriggerActionDatas2.ContainsKey(triggerData.ActionUnitIdx))
        //     {
        //         TriggerActionDatas2.Add(triggerData.ActionUnitIdx,
        //             new GameFrameworkMultiDictionary<int, ITriggerActionData>());
        //     }
        //
        //
        //     // if (!TriggerActionDatas[triggerData.ActionUnitID].Contains(triggerData.EffectUnitID))
        //     // {
        //     //     TriggerActionDatas[triggerData.ActionUnitID].Add(triggerData.EffectUnitID, triggerActionDatas);
        //     // }
        //     // else
        //     // {
        //     //     triggerActionData = TriggerActionDatas[triggerData.ActionUnitID][triggerData.EffectUnitID];
        //     // }
        //
        //     var triggerActionData = new TriggerActionTriggerData();
        //     triggerActionData.TriggerData = triggerData;//.Copy();
        //     TriggerActionDatas2[triggerData.ActionUnitIdx].Add(triggerData.EffectUnitIdx, triggerActionData);
        //
        // }
        
        // public void AddTriggerDataByEffect(TriggerData triggerData)
        // {
        //
        //     if (!TriggerActionDatas.ContainsKey(triggerData.EffectUnitIdx))
        //     {
        //         TriggerActionDatas.Add(triggerData.EffectUnitIdx,
        //             new GameFrameworkMultiDictionary<int, ITriggerActionData>());
        //     }
        //
        //     var triggerActionData = new TriggerActionTriggerData();
        //     triggerActionData.TriggerData = triggerData.Copy();
        //     TriggerActionDatas[triggerData.EffectUnitIdx].Add(triggerData.EffectUnitIdx, triggerActionData);
        //
        // }


        // public void AddMoveActionData(int actionUnitIdx, MoveData moveData)
        // {
        //     //var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(moveActionData.ActionUnitID);
        //     //effectUnit.ActionUnitID = triggerData.ActionUnitID;
        //
        //     foreach (var kv in moveData.MoveUnitDatas)
        //     {
        //         if (!TriggerActionDatas2.ContainsKey(actionUnitIdx))
        //         {
        //
        //             TriggerActionDatas2.Add(actionUnitIdx,
        //                 new GameFrameworkMultiDictionary<int, ITriggerActionData>());
        //         }
        //
        //         var triggerActionData = new TriggerActionMoveData();
        //         // if (!TriggerActionDatas[actionUnitID].Contains(kv.Key))
        //         // {
        //         //     triggerActionData = new TriggerActionData();
        //         //     TriggerActionDatas[actionUnitID].Add(kv.Key, triggerActionData);
        //         // }
        //         // else
        //         // {
        //         //     triggerActionData = TriggerActionDatas[actionUnitID][kv.Key];
        //         // }
        //
        //         triggerActionData.MoveUnitData = kv.Value;
        //         TriggerActionDatas2[actionUnitIdx].Add(kv.Key, triggerActionData);
        //     }
        //
        // }
        //
        // public void UseTriggerData(int actionUnitIdx, int effectUnitIdx)
        // {
        //
        //
        //     // if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
        //     // {
        //     //     return;
        //     // }
        //     //
        //     // if (effectUnitIdx != -1 && !TriggerActionDatas[actionUnitIdx].Contains(effectUnitIdx))
        //     // {
        //     //     return;
        //     // }
        //
        //     foreach (var kv in TriggerActionDatas2)
        //     {
        //         var list = kv.Value.Reverse().ToList();
        //         for (int i = list.Count - 1; i >= 0; i--)
        //         {
        //             var list2 = list[i].Value.Reverse().ToList();
        //             for (int j = list2.Count - 1; j >= 0; j--)
        //             {
        //                 var triggerActionData = list2[j];
        //                 if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
        //                 {
        //                     //反击会失效
        //                     // //教程cores位置 randomseed 91408126 抛射尚未到达，被攻击单位就开始攻击，重复触发抛射TriggerData
        //                     // if(effectUnitIdx != -1 && effectUnitIdx != triggerActionTriggerData.TriggerData.EffectUnitIdx)
        //                     //     continue;
        //                     //     
        //                     // if(actionUnitIdx != -1 && actionUnitIdx != triggerActionTriggerData.TriggerData.ActionUnitIdx)
        //                     //     continue;
        //
        //                     UseTriggerData(triggerActionTriggerData.TriggerData);
        //                 }
        //             }
        //         }
        //         
        //     }
        //
        //     // var triggerActionDatas = TriggerActionDatas[actionUnitIdx][effectUnitIdx];
        //     //
        //     // foreach (var triggerActionData in triggerActionDatas)
        //     // {
        //     //     if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
        //     //     {
        //     //         UseTriggerData(triggerActionTriggerData.TriggerData);
        //     //     }
        //     //
        //     // }
        //
        //
        //
        //
        // }

        

        // public void UseMoveActionData(int actionUnitIdx, int effectUnitIdx)
        // {
        //     if (!TriggerActionDatas2.ContainsKey(actionUnitIdx))
        //     {
        //         return;
        //     }
        //
        //     if (!TriggerActionDatas2[actionUnitIdx].Contains(effectUnitIdx))
        //     {
        //         return;
        //     }
        //
        //     var moveActionDatas = TriggerActionDatas2[actionUnitIdx][effectUnitIdx];
        //
        //     foreach (var moveActionData in moveActionDatas)
        //     {
        //
        //         if (moveActionData is TriggerActionMoveData triggerActionMoveData)
        //         {
        //             if (triggerActionMoveData.MoveUnitData == null)
        //             {
        //                 continue;
        //             }
        //
        //             UseMoveActionData(triggerActionMoveData.MoveUnitData);
        //             ClearMoveData();
        //         }
        //
        //     }
        //
        //
        //
        //
        // }

        

        // public void ActionUnitTrigger(int actionUnitIdx, int effectUnitIdx = -1)
        // {
        //     if (!TriggerActionDatas2.ContainsKey(actionUnitIdx))
        //     {
        //         return;
        //     }
        //
        //     var triggerActionDataList = TriggerActionDatas2[actionUnitIdx].ToList();
        //     
        //     //for (int i = triggerActionDataList.Count - 1; i >= 0; i--)
        //     for (int i = 0; i < triggerActionDataList.Count; i++)
        //     {
        //
        //         var triggerActionData = triggerActionDataList[i];
        //         var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerActionData.Key);
        //         if(effectUnitIdx != -1 && effectUnit != null && effectUnit.UnitIdx != effectUnitIdx)
        //             continue;
        //
        //         UseTriggerData(actionUnitIdx, effectUnit == null ? -1 : effectUnit.UnitIdx);
        //         UseMoveActionData(actionUnitIdx, effectUnit == null ? -1 : effectUnit.UnitIdx);
        //     }
        //     
        //     // foreach (var kv in TriggerActionDatas[actionUnitIdx])
        //     // {
        //     //     var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
        //     //
        //     //     UseTriggerData(actionUnitIdx, effectUnit.UnitIdx);
        //     //     UseMoveActionData(actionUnitIdx, effectUnit.UnitIdx);
        //     // }
        //
        //     //TriggerActionDatas[actionUnitIdx].Clear();
        //     BattleManager.Instance.RefreshView();
        // }

        // public GameFrameworkMultiDictionary<int, ITriggerActionData> GetTriggerActionDatas(int actionUnitIdx)
        // {
        //     if (!TriggerActionDatas2.ContainsKey(actionUnitIdx))
        //     {
        //         return null;
        //     }
        //
        //     return TriggerActionDatas2[actionUnitIdx];
        // }

        // public List<ITriggerActionData> GetTriggerActionDatas(int actionUnitIdx, int effectUnitIdx = -1)
        // {
        //     var triggerDatas = GetTriggerActionDatas(actionUnitIdx);
        //     if (triggerDatas == null)
        //     {
        //         return null;
        //     }
        //
        //     var list = new List<ITriggerActionData>();
        //     if (effectUnitIdx == -1)
        //     {
        //         foreach (var kv in triggerDatas)
        //         {
        //             list.AddRange(kv.Value.ToList());
        //         }
        //     }
        //     else
        //     {
        //
        //         foreach (var kv in triggerDatas)
        //         {
        //             foreach (var triggerActionData in kv.Value)
        //             {
        //                 if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData &&
        //                     (triggerActionTriggerData.TriggerData.EffectUnitIdx == effectUnitIdx ||
        //                     triggerActionTriggerData.TriggerData.InterrelatedEffectUnitIdx == effectUnitIdx))
        //                 {
        //                     list.Add(triggerActionData);
        //                 }
        //                 else if (triggerActionData is TriggerActionMoveData triggerActionMoveData)
        //                 {
        //                     var isAdd = false;
        //                     if (triggerActionMoveData.MoveUnitData.MoveActionData.MoveUnitIdx == effectUnitIdx ||
        //                         triggerActionMoveData.MoveUnitData.MoveActionData.MoveUnitIdx == actionUnitIdx)
        //                     {
        //                         isAdd = true;
        //                     }
        //                     foreach (var kv2 in triggerActionMoveData.MoveUnitData.MoveActionData.TriggerDataDict)
        //                     {
        //                         foreach (var triggerData in kv2.Value)
        //                         {
        //                             if (triggerData.EffectUnitIdx == effectUnitIdx ||
        //                                 triggerData.InterrelatedEffectUnitIdx == effectUnitIdx)
        //                             {
        //                                 isAdd = true;
        //                                 
        //                                 break;
        //                             }
        //                         }
        //                     }
        //
        //                     if (isAdd)
        //                     {
        //                         list.Add(triggerActionData);
        //                     }
        //                     
        //                 }
        //             }
        //         }
        //         
        //         // if (triggerDatas.TryGetValue(effectUnitIdx, out var triggerActionDatas))
        //         // {
        //         //     list = triggerActionDatas.ToList();
        //         // }
        //     }
        //
        //     
        //
        //     return list;
        // }
        //
        // public ITriggerActionData GetTriggerActionData(int triggerDataIdx)
        // {
        //     foreach (var kv in TriggerActionDatas2)
        //     {
        //         foreach (var kv2 in kv.Value)
        //         {
        //             foreach (var data in kv2.Value)
        //             {
        //                 if (data is TriggerActionTriggerData triggerActionTriggerData)
        //                 {
        //                     if (triggerActionTriggerData.TriggerData.Idx == triggerDataIdx)
        //                     {
        //                         return data;
        //                     }
        //                         
        //                 }
        //             }
        //         }
        //     }
        //
        //     return null;
        //     
        // }

        
        // public List<TriggerActionData> GetTriggerActionDatasA(int actionUnitID)
        // {
        //     var triggerDatas = GetTriggerActionDatas(actionUnitID);
        //     if ( triggerDatas == null)
        //     {
        //         return null;
        //     }
        //
        //     var list = triggerDatas.ke
        //
        //     return list;
        // }

        // public void ClearData(int actionUnitIdx)
        // {
        //     if (TriggerActionDatas.ContainsKey(actionUnitIdx))
        //     {
        //         TriggerActionDatas[actionUnitIdx].Clear();
        //     }
        // }
        //
        // public void ClearMoveData(int actionUnitIdx, int moveUnitIdx)
        // {
        //     if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
        //     {
        //         return;
        //     }
        //
        //     if (!TriggerActionDatas[actionUnitIdx].Contains(moveUnitIdx))
        //     {
        //         return;
        //     }
        //
        //     var list = TriggerActionDatas[actionUnitIdx][moveUnitIdx].ToList();
        //     for (int i = list.Count - 1; i >= 0; i--)
        //     {
        //         var triggerActionData = list[i];
        //
        //         if (triggerActionData is TriggerActionMoveData triggerActioMoveData)
        //         {
        //             TriggerActionDatas[actionUnitIdx].Remove(moveUnitIdx, triggerActioMoveData);
        //         }
        //         
        //     }
        //     
        // }
        
        public void ClearMoveData()
        {
            // var keys = TriggerActionDatas2.Keys.ToList();
            // var values = TriggerActionDatas2.Values.ToList();
            //
            // for (int i = values.Count - 1; i >= 0; i--)
            // {
            //     var list = values[i].ToList();
            //     
            //     for (int j = list.Count - 1; j >= 0; j--)
            //     {
            //         var list2 = list[j].Value.ToList();
            //         
            //         
            //         for (int k = list2.Count - 1; k >= 0; k--)
            //         {
            //             
            //             if (list2[k] is TriggerActionMoveData triggerActioMoveData)
            //             {
            //                 if (triggerActioMoveData.MoveUnitData.IsTrigger)
            //                 {
            //                     TriggerActionDatas2[keys[i]].Remove(list[j].Key, triggerActioMoveData);
            //                 }
            //             }
            //         }
            //         
            //     }
            // }
   
        }
        

        
    }

}
