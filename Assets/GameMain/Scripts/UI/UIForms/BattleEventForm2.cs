using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleEventFormData2
    {
        public int RandomSeed;
    }
    
    public class BattleEventForm2 : UGuiForm
    {
        public BattleEventData battleEventData;

        [SerializeField]
        private List<BattleEventItem> BattleEventItems;
        
        [SerializeField]
        private TextMeshProUGUI text;
        
        private BattleEventFormData2 _battleEventFormData2;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            _battleEventFormData2 = (BattleEventFormData2)userData;
            if (_battleEventFormData2 == null)
            {
                Log.Warning("BattleEventFormData is null.");
                return;
            }

            battleEventData = BattleEventManager.Instance.GenerateRandomEvent(_battleEventFormData2.RandomSeed);

            text.text = battleEventData.BattleEventExpressionType + "-" + battleEventData.BattleEvent;

            // for (int i = 0; i < BattleEventItems.Count; i++)
            // {
            //     if (i < battleEventData.BattleEventItemDatas.Count)
            //     {
            //         BattleEventItems[i].Init(battleEventData.BattleEventItemDatas[i]);
            //     }
            //     
            // }
        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
        }
    }
}