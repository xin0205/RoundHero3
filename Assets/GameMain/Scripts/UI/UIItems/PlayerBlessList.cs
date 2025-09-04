using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Event;
using SuperScrollView;
using UnityEngine;

namespace RoundHero
{
    public class PlayerBlessList : MonoBehaviour
    {
        public LoopGridView blessGridView;
        private List<int> blessList = new ();


        private void Awake()
        {
            blessGridView.InitGridView(0, OnBlessIconGetItemByRowColumn);
        }

        private void OnEnable()
        {
            GameEntry.Event.Subscribe(RefreshPlayerInfoEventArgs.EventId, OnRefreshPlayerInfo);
            GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());
        }

        private void OnDisable()
        {
            GameEntry.Event.Unsubscribe(RefreshPlayerInfoEventArgs.EventId, OnRefreshPlayerInfo);
        }
        private void OnRefreshPlayerInfo(object sender, GameEventArgs e)
        {
            Refresh();
        }
        
        private async void Refresh()
        {
            blessList = BlessManager.Instance.BlessDatas.Keys.ToList();
            blessGridView.SetListItemCount(blessList.Count);
            blessGridView.RefreshAllShownItem();
        }

        LoopGridViewItem OnBlessIconGetItemByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {
            
            var blessData = BlessManager.Instance.GetBless(blessList[itemIndex]);
            if (blessData == null)
            {
                return null;
            }

            var item = gridView.NewListViewItem("BlessIconItem");

            var itemScript = item.GetComponent<BlessIconItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            itemScript.SetItemData(blessData, itemIndex, row, column);
            return item;
        }

    }
}