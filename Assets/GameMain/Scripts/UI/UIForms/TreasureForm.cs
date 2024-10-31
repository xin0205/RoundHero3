using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class TreasureFormData
    {
        public int RandomSeed;
    }
    
    public class TreasureForm : UGuiForm
    {
        private BattleEventData battleEventData;

        [SerializeField]
        private List<BattleEventItem> BattleEventItems;
        
        private TreasureFormData treasureFormData;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            treasureFormData = (TreasureFormData)userData;
            if (treasureFormData == null)
            {
                Log.Warning("TreasureFormData is null.");
                return;
            }

            battleEventData = BattleEventManager.Instance.GenerateTreasureEvent(treasureFormData.RandomSeed);

            //text.text = battleEventData.BattleEventExpressionType + "-" + battleEventData.BattleEvent;

            for (int i = 0; i <  BattleEventItems.Count; i++)
            {
                BattleEventItems[i].Init(battleEventData.BattleGameEventItemDatas[i]);
            }

            
        }

        private void OnClickItem(int itemIdx)
        {
            foreach (var VARIABLE in )
            {
                
            }
        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
        }
    }
}