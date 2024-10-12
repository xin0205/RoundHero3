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
        public List<BattleEventItemData> BattleEventItemDatas;

        [SerializeField]
        private BattleEventItem BattleEventItem;
        
        [SerializeField]
        private Text text;
        
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

            BattleEventItemDatas = TreasureManager.Instance.GenerateTreasureEvent(treasureFormData.RandomSeed);

            //text.text = battleEventData.BattleEventExpressionType + "-" + battleEventData.BattleEvent;

            BattleEventItem.Init(BattleEventItemDatas);
        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
        }
    }
}