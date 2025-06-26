﻿using System.Collections.Generic;
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
        public Dictionary<int, GameFrameworkMultiDictionary<int, ITriggerActionData>> TriggerActionDatas =
            new();


        public void AddTriggerData(TriggerData triggerData)
        {
            //var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;


            if (!TriggerActionDatas.ContainsKey(triggerData.ActionUnitIdx))
            {
                TriggerActionDatas.Add(triggerData.ActionUnitIdx,
                    new GameFrameworkMultiDictionary<int, ITriggerActionData>());
            }


            // if (!TriggerActionDatas[triggerData.ActionUnitID].Contains(triggerData.EffectUnitID))
            // {
            //     TriggerActionDatas[triggerData.ActionUnitID].Add(triggerData.EffectUnitID, triggerActionDatas);
            // }
            // else
            // {
            //     triggerActionData = TriggerActionDatas[triggerData.ActionUnitID][triggerData.EffectUnitID];
            // }

            var triggerActionData = new TriggerActionTriggerData();
            triggerActionData.TriggerData = triggerData.Copy();
            TriggerActionDatas[triggerData.ActionUnitIdx].Add(triggerData.EffectUnitIdx, triggerActionData);

        }

        public void AddMoveActionData(int actionUnitIdx, MoveData moveData)
        {
            //var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(moveActionData.ActionUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;

            foreach (var kv in moveData.MoveUnitDatas)
            {
                if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
                {

                    TriggerActionDatas.Add(actionUnitIdx,
                        new GameFrameworkMultiDictionary<int, ITriggerActionData>());
                }

                var triggerActionData = new TriggerActionMoveData();
                // if (!TriggerActionDatas[actionUnitID].Contains(kv.Key))
                // {
                //     triggerActionData = new TriggerActionData();
                //     TriggerActionDatas[actionUnitID].Add(kv.Key, triggerActionData);
                // }
                // else
                // {
                //     triggerActionData = TriggerActionDatas[actionUnitID][kv.Key];
                // }

                triggerActionData.MoveUnitData = kv.Value.Copy();
                TriggerActionDatas[actionUnitIdx].Add(kv.Key, triggerActionData);
            }

        }

        public void UseTriggerData(int actionUnitID, int effectUnitID)
        {


            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return;
            }

            if (!TriggerActionDatas[actionUnitID].Contains(effectUnitID))
            {
                return;
            }

            var triggerActionDatas = TriggerActionDatas[actionUnitID][effectUnitID];

            foreach (var triggerActionData in triggerActionDatas)
            {
                if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
                {
                    UseTriggerData(triggerActionTriggerData.TriggerData);
                }

            }




        }

        public void UseTriggerData(TriggerData triggerData)
        {
            triggerData.IsTrigger = true;
            BattleFightManager.Instance.TriggerAction(triggerData.Copy());
            ClearTriggerData();
        }

        public void UseMoveActionData(int actionUnitIdx, int effectUnitIdx)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
            {
                return;
            }

            if (!TriggerActionDatas[actionUnitIdx].Contains(effectUnitIdx))
            {
                return;
            }

            var moveActionDatas = TriggerActionDatas[actionUnitIdx][effectUnitIdx];

            foreach (var moveActionData in moveActionDatas)
            {

                if (moveActionData is TriggerActionMoveData triggerActionMoveData)
                {
                    if (triggerActionMoveData.MoveUnitData == null)
                    {
                        continue;
                    }

                    UseMoveActionData(triggerActionMoveData.MoveUnitData);
                    ClearMoveData();
                }

            }




        }

        public void UseMoveActionData(MoveUnitData moveUnitData)
        {
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

            moveUnitData.MoveActionData.Clear();

        }

        public void ActionUnitTrigger(int actionUnitIdx, int effectUnitIdx = -1)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
            {
                return;
            }


            var triggerActionDataList = TriggerActionDatas[actionUnitIdx].ToList();

            for (int i = triggerActionDataList.Count - 1; i >= 0; i--)
            {

                var triggerActionData = triggerActionDataList[i];
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerActionData.Key);
                if(effectUnitIdx != -1 && effectUnit != null && effectUnit.UnitIdx != effectUnitIdx)
                    continue;

                UseTriggerData(actionUnitIdx, effectUnit.UnitIdx);
                UseMoveActionData(actionUnitIdx, effectUnit.UnitIdx);
            }
            
            // foreach (var kv in TriggerActionDatas[actionUnitIdx])
            // {
            //     var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
            //
            //     UseTriggerData(actionUnitIdx, effectUnit.UnitIdx);
            //     UseMoveActionData(actionUnitIdx, effectUnit.UnitIdx);
            // }

            //TriggerActionDatas[actionUnitIdx].Clear();
            BattleManager.Instance.RefreshView();
        }

        public GameFrameworkMultiDictionary<int, ITriggerActionData> GetTriggerActionDatas(int actionUnitIdx)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitIdx))
            {
                return null;
            }

            return TriggerActionDatas[actionUnitIdx];
        }

        public List<ITriggerActionData> GetTriggerActionDatas(int actionUnitIdx, int effectUnitIdx)
        {
            var triggerDatas = GetTriggerActionDatas(actionUnitIdx);
            if (triggerDatas == null)
            {
                return null;
            }

            if (triggerDatas.TryGetValue(effectUnitIdx, out var triggerActionDatas))
            {
                return triggerActionDatas.ToList();
            }

            return null;
        }

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
            var keys = TriggerActionDatas.Keys.ToList();
            var values = TriggerActionDatas.Values.ToList();

            for (int i = values.Count - 1; i >= 0; i--)
            {
                var list = values[i].ToList();
                
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    var list2 = list[j].Value.ToList();
                    
                    
                    for (int k = list2.Count - 1; k >= 0; k--)
                    {
                        
                        if (list2[k] is TriggerActionMoveData triggerActioMoveData)
                        {
                            if (triggerActioMoveData.MoveUnitData.IsTrigger)
                            {
                                TriggerActionDatas[keys[i]].Remove(list[j].Key, triggerActioMoveData);
                            }
                        }
                    }
                    
                }
            }
   
        }
        

        public void ClearTriggerData()
        {
            
            var keys = TriggerActionDatas.Keys.ToList();
            var values = TriggerActionDatas.Values.ToList();

            for (int i = values.Count - 1; i >= 0; i--)
            {
                var list = values[i].ToList();
                
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    var list2 = list[j].Value.ToList();
                    
                    for (int k = list2.Count - 1; k >= 0; k--)
                    {
                        
                        if (list2[k] is TriggerActionTriggerData triggerActionTriggerData)
                        {
                            if (triggerActionTriggerData.TriggerData.IsTrigger)
                            {
                                TriggerActionDatas[keys[i]].Remove(list[j].Key, triggerActionTriggerData);
                            }
                        }
                    }
                    
                }
            }
            
            // foreach (var kv in TriggerActionDatas)
            // {
            //     var list = kv.Value.ToList();
            //     for (int i = list.Count - 1; i >= 0; i--)
            //     {
            //         var list2 = list[i].Value.ToList();
            //         for (int j = list2.Count - 1; j >= 0; j--)
            //         {
            //             var triggerActionData = list2[j];
            //
            //             if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
            //             {
            //                 if (triggerActionTriggerData.TriggerData.IsTrigger)
            //                 {
            //                     TriggerActionDatas[kv.Key].Remove(list[i].Key, triggerActionTriggerData);
            //                 }
            //             }
            //     
            //         }
            //     }
            //
            // }
        }

        public void ClearMoveData(int actionUnitIdx)
        {
            if (TriggerActionDatas.ContainsKey(actionUnitIdx))
            {
                TriggerActionDatas[actionUnitIdx].Clear();
            }
        }
    }

}
