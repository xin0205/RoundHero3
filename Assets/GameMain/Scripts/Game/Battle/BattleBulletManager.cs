using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    
    
    
    public class BattleBulletManager : Singleton<BattleBulletManager>
    {
        public Dictionary<int, Dictionary<int, TriggerData>> TriggerDatas =
            new ();


        public void AddTriggerData(TriggerData triggerData)
        {
            var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            //effectUnit.ActionUnitID = triggerData.ActionUnitID;
            
            if (!TriggerDatas.ContainsKey(triggerData.ActionUnitID))
            {
                TriggerDatas.Add(triggerData.ActionUnitID, new Dictionary<int, TriggerData>());
            } 
            
            TriggerDatas[triggerData.ActionUnitID].Add(triggerData.EffectUnitID, triggerData.Copy());
        }

        public void UseTriggerData(int actionUnitID, int effectUnitID)
        {
            if (!TriggerDatas.ContainsKey(actionUnitID))
            {
                return;
            }
            
            if (!TriggerDatas[actionUnitID].ContainsKey(effectUnitID))
            {
                return;
            }

            BattleFightManager.Instance.TriggerAction(TriggerDatas[actionUnitID][effectUnitID]);
            

        }

        public void ActionUnitTrigger(int actionUnitID)
        {
            if (!TriggerDatas.ContainsKey(actionUnitID))
            {
                return;
            }

            foreach (var kv in TriggerDatas[actionUnitID])
            {
                var effectUnit = BattleUnitManager.Instance.GetUnitByID(kv.Key);
                
                UseTriggerData(actionUnitID, effectUnit.ID);
            }
            TriggerDatas[actionUnitID].Clear();
        }

        public Dictionary<int, TriggerData> GetTriggerDatas(int actionUnitID)
        {
            if (!TriggerDatas.ContainsKey(actionUnitID))
            {
                return null;
            }

            return TriggerDatas[actionUnitID];
        }
        
        
    }
}