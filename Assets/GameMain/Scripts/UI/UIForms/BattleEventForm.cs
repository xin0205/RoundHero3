using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleEventFormData
    {
        public int RandomSeed;
    }
    
    public class BattleEventForm : UGuiForm
    {
        public BattleEventData battleEventData;

        [SerializeField]
        private List<BattleEventItem> BattleEventItems;
        
        [SerializeField]
        private TextMeshProUGUI text;
        
        private BattleEventFormData battleEventFormData;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            battleEventFormData = (BattleEventFormData)userData;
            if (battleEventFormData == null)
            {
                Log.Warning("BattleEventFormData is null.");
                return;
            }

            battleEventData = BattleEventManager.Instance.GenerateEvent(battleEventFormData.RandomSeed);

            text.text = battleEventData.BattleEventExpressionType + "-" + battleEventData.BattleEvent;

            for (int i = 0; i < BattleEventItems.Count; i++)
            {
                if (i < battleEventData.BattleGameEventItemDatas.Count)
                {
                    BattleEventItems[i].Init(battleEventData.BattleGameEventItemDatas[i]);
                }
                
            }
        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
        }
    }
}