using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public class TriggerActionData
    {
        public TriggerData TriggerData;
        public MoveActionData MoveActionData;
    }
    
    
    public class BattleBulletManager : Singleton<BattleBulletManager>
    {
        public Dictionary<int, Dictionary<int, TriggerActionData>> TriggerActionDatas =
            new ();


        public void AddTriggerData(TriggerData triggerData)
        {
            //var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;

            
            if (!TriggerActionDatas.ContainsKey(triggerData.ActionUnitID))
            {
                
                TriggerActionDatas.Add(triggerData.ActionUnitID, new Dictionary<int, TriggerActionData>());
            } 
            var triggerActionData = new TriggerActionData();
            triggerActionData.TriggerData = triggerData.Copy();
            TriggerActionDatas[triggerData.ActionUnitID].Add(triggerData.EffectUnitID, triggerActionData);
        }
        
        public void AddMoveActionData(int actionUnitID, MoveActionData moveActionData)
        {
            //var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(moveActionData.ActionUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;

            
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                
                TriggerActionDatas.Add(moveActionData.ActionUnitID, new Dictionary<int, TriggerActionData>());
            } 
            var triggerActionData = new TriggerActionData();
            triggerActionData.MoveActionData = moveActionData.Copy();
            TriggerActionDatas[moveActionData.ActionUnitID].Add(moveActionData.ActionUnitID, triggerActionData);
        }

        public void UseTriggerData(int actionUnitID, int effectUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return;
            }
            
            if (!TriggerActionDatas[actionUnitID].ContainsKey(effectUnitID))
            {
                return;
            }

            BattleFightManager.Instance.TriggerAction(TriggerActionDatas[actionUnitID][effectUnitID].TriggerData);
            

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
            }
            TriggerActionDatas[actionUnitID].Clear();
            BattleManager.Instance.Refresh();
        }

        public Dictionary<int, TriggerActionData> GetTriggerDatas(int actionUnitID)
        {
            if (!TriggerActionDatas.ContainsKey(actionUnitID))
            {
                return null;
            }

            return TriggerActionDatas[actionUnitID];
        }
        
        
    }
}