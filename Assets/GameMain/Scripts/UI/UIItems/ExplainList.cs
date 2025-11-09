using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public class ExplainList : MonoBehaviour
    {
        public List<ExplainItem> explainItems = new List<ExplainItem>();

        private void Awake()
        {
            var explainItems = GetComponentsInChildren<ExplainItem>(true);

            foreach (var explainItem in explainItems)
            {
                explainItem.gameObject.SetActive(false);
                this.explainItems.Add(explainItem);
            }
            
        }

        public void SetData(List<CommonItemData> commonItemDatas)
        {
            foreach (var explainItem in explainItems)
            {
                explainItem.gameObject.SetActive(false);
            }
            
            var idx = 0;
            foreach (var commonItemData in commonItemDatas)
            {
                explainItems[idx].gameObject.SetActive(true);
                explainItems[idx].SetItemData(commonItemData);
                idx++;
            }
        }
    }
}