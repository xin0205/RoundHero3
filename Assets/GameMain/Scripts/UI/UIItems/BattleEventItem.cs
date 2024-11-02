using System;
using System.Collections.Generic;
using GameFramework;
using TMPro;
using UGFExtensions.Await;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoundHero
{
    public class BattleEventItem : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        private List<BattleEventItemData> battleGameEventItemDatas;

        private Action<int> onClickAction;

        private int idx;

        public void Init(int idx, List<BattleEventItemData> battleGameEventItemDatas, Action<int> OnClickAction)
        {
            this.battleGameEventItemDatas = battleGameEventItemDatas;
            this.onClickAction = OnClickAction;
            this.idx = idx;
            
            text.text = "";
            var eventValues = new List<string>();
            
            foreach (var battleGameEventItemData in battleGameEventItemDatas)
            {
                eventValues.Clear();
                var eventTypeStr = GameEntry.Localization.GetString(battleGameEventItemData.EventType.ToString());

                foreach (var val in battleGameEventItemData.EventValues)
                {
                    eventValues.Add(val.ToString());
                }
                

                var changeValue = false;

                if (Constant.BattleEvent.AppointEventTypeItemTypeMap.ContainsKey(battleGameEventItemData.EventType))
                {
                    var name = "";
                    var desc = "";
                    GameUtility.GetItemText(Constant.BattleEvent.AppointEventTypeItemTypeMap[battleGameEventItemData.EventType], battleGameEventItemData.EventValues[0], ref name, ref desc);
                    eventValues[0] = name;
                    changeValue = true;
                }
                
                
                text.text += GameEntry.Localization.GetLocalizedStrings(eventTypeStr, eventValues);

                // var names = new List<string>();
                // foreach (var value in battleGameEventItemData.EventValues)
                // {
                //     GameUtility.GetItemText();
                // }
                //
                // text.text += "-";
            }
            
            
        }

        public void OnClick()
        {
            this.onClickAction.Invoke(idx);
        }
        
        
        

    }
}