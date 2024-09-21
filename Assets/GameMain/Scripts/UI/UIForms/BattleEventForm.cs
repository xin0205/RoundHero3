using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RoundHero
{
    public class BattleEventForm : UGuiForm
    {
        public BattleEventData battleEventData;

        [SerializeField]
        private List<BattleEventItem> BattleEventItems;
        
        [SerializeField]
        private TextMeshProUGUI text;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            battleEventData = BattleEventManager.Instance.GenerateEvent();

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