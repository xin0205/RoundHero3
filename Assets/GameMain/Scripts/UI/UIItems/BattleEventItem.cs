using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class BattleEventItem : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        private List<BattleEventItemData> battleGameEventItemDatas;

        public void Init(List<BattleEventItemData> battleGameEventItemDatas)
        {
            this.battleGameEventItemDatas = battleGameEventItemDatas;
            
            text.text = "";
            foreach (var battleGameEventItemData in battleGameEventItemDatas)
            {
                text.text += battleGameEventItemData.EventType.ToString();

                foreach (var value in battleGameEventItemData.EventValues)
                {
                    text.text += "_" + value;
                }

                text.text += "-";
            }
            
        }

        public void OnClick()
        {
            foreach (var battleEventItemData in this.battleGameEventItemDatas)
            {
                BattleEventManager.Instance.AcquireEventItem(battleEventItemData);
            }
        }

    }
}