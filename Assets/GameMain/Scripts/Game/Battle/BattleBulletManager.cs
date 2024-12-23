using System.Collections.Generic;
using System.Linq;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public class TriggerActionData
    {
        public TriggerData TriggerData;
        public MoveUnitData MoveUnitData;
    }
    
    
    public class BattleBulletManager : Singleton<BattleBulletManager>
    {
        public Dictionary<int, GameFrameworkMultiDictionary<int, TriggerActionData>> TriggerActionDatas =
            new ();


        public void AddTriggerData(TriggerData triggerData)
        {
            //var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;

            
            if (!TriggerActionDatas.ContainsKey(triggerData.ActionUnitID))
            {
                TriggerActionDatas.Add(triggerData.ActionUnitID, new GameFrameworkMultiDictionary<int, TriggerActionData>());
            } 
            
            
            // if (!TriggerActionDatas[triggerData.ActionUnitID].Contains(triggerData.EffectUnitID))
            // {
            //     TriggerActionDatas[triggerData.ActionUnitID].Add(triggerData.EffectUnitID, triggerActionDatas);
            // }
            // else
            // {
            //     triggerActionData = TriggerActionDatas[triggerData.ActionUnitID][triggerData.EffectUnitID];
            // }
            
            var triggerActionData = new TriggerActionData();
            triggerActionData.TriggerData = triggerData.Copy();
            TriggerActionDatas[triggerData.ActionUnitID].Add(triggerData.EffectUnitID, triggerActionData);
        }
        
        public void AddMoveActionData(int actionUnitID, MoveData moveData)
        {
            //var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(moveActionData.ActionUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;

            foreach (var kv in moveData.MoveUnitDatas)
            {
                if (!TriggerActionDatas.ContainsKey(actionUnitID))
                {
                
                    TriggerActionDatas.Add(actionUnitID, new GameFrameworkMultiDictionary<int, TriggerActionData>());
                }
                
                var triggerActionData = new TriggerActionData();
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
                TriggerActionDatas[actionUnitID].Add(kv.Key, triggerActionData);
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
            
            var moveActionDatas = TriggerActionDatas[actionUnitID][effectUnitID];
            
            foreach (var moveActionData in moveActionDatas)
            {
                
                BattleFightManager.Instance.TriggerAction(moveActionData.TriggerData);
            }

            
            

        }
        
        public void UseMoveActionData(int actionUnitID, int effectUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return;
            }
            
            if (!TriggerActionDatas[actionUnitID].Contains(effectUnitID))
            {
                return;
            }
            
            var moveActionDatas = TriggerActionDatas[actionUnitID][effectUnitID];

            foreach (var moveActionData in moveActionDatas)
            {
                if (moveActionData.MoveUnitData == null)
                {
                    continue;
                }
                
                UseMoveActionData(moveActionData.MoveUnitData);

            }
            
            

            
        }
        
        public void UseMoveActionData(MoveUnitData moveUnitData)
        {
            
            var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(moveUnitData.UnitID);

            if (moveUnitData.UnitActionState == EUnitActionState.Fly)
            {
                effectUnitEntity.Fly(moveUnitData.MoveActionData);
            }
            else if (moveUnitData.UnitActionState == EUnitActionState.Run)
            {
                effectUnitEntity.Run(moveUnitData.MoveActionData);
            }
            
           
        }

        public void ActionUnitTrigger(int actionUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return;
            }

            foreach (var kv in TriggerActionDatas[actionUnitID])
            {
                var effectUnit = BattleUnitManager.Instance.GetUnitByID(kv.Key);
                
                UseTriggerData(actionUnitID, effectUnit.ID);
                UseMoveActionData(actionUnitID, effectUnit.ID);
            }
            TriggerActionDatas[actionUnitID].Clear();
            BattleManager.Instance.RefreshView();
        }

        public GameFrameworkMultiDictionary<int, TriggerActionData> GetTriggerActionDatas(int actionUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return null;
            }

            return TriggerActionDatas[actionUnitID];
        }
        
        public List<TriggerActionData> GetTriggerActionDatas(int actionUnitID, int effectUnitID)
        {
            var triggerDatas = GetTriggerActionDatas(actionUnitID);
            if ( triggerDatas == null)
            {
                return null;
            }

            if (triggerDatas.TryGetValue(effectUnitID, out var triggerActionDatas))
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
        
        public void ClearData(int actionUnitID)
        {
            if (TriggerActionDatas.ContainsKey(actionUnitID))
            {
                TriggerActionDatas[actionUnitID].Clear();
            }
        }
    }
}