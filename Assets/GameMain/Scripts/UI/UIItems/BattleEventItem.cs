using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RoundHero
{
    public class BattleEventItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        public void Init(List<BattleEventItemData> battleGameEventItemDatas)
        {
            text.text = "";
            foreach (var battleGameEventItemData in battleGameEventItemDatas)
            {
                text.text += battleGameEventItemData.EventType.ToString();

                foreach (var itemID in battleGameEventItemData.RandomItemIDs)
                {
                    text.text += "_" + itemID;
                }

                text.text += "-";
            }
            
        }

    }
}