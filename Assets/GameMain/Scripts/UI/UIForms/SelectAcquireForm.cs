using System;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

using UnityGameFramework.Runtime;

namespace RoundHero
{
    
    
    public class SelectAcquireFormParams
    {
        public List<SelectAcquireItemData> SelectAcquireItemDatas = new();
        public Action<int> OnClick;

    }
    
    public class SelectAcquireForm : UGuiForm
    {
        [SerializeField]
        private LoopGridView itemView;
        
        private SelectAcquireFormParams selectAcquireFormParams;
        
        private void Awake()
        {
            itemView.InitGridView(0,  OnGetItemByRowColumn);
  
        }
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            selectAcquireFormParams = (SelectAcquireFormParams)userData;
            if (selectAcquireFormParams == null)
            {
                Log.Warning("SelectAcquireFormParams is null.");
                return;
            }
            
            itemView.SetListItemCount(selectAcquireFormParams.SelectAcquireItemDatas.Count);
            itemView.RefreshAllShownItem();
            
        }
        
        LoopGridViewItem OnGetItemByRowColumn(LoopGridView view, int itemIndex,int row,int column)
        {
            if (itemIndex < 0)
            {
                return null;
            }
            
            var item = view.NewListViewItem("SelectAcquireItem");
        
            var itemScript = item.GetComponent<SelectAcquireItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.SetItemData(selectAcquireFormParams.SelectAcquireItemDatas[itemIndex],
                selectAcquireFormParams.OnClick, itemIndex, row, column);
            
            return item;
        }
    }
}