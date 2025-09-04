using System;
using System.Collections.Generic;
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
        // [SerializeField]
        // private LoopGridView itemView;
        
        private SelectAcquireFormParams selectAcquireFormParams;

        [SerializeField] private List<SelectAcquireItem> selectAcquireItems;
        
        private void Awake()
        {
            //itemView.InitGridView(0,  OnGetItemByRowColumn);
  
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
            
            //itemView.SetListItemCount(selectAcquireFormParams.SelectAcquireItemDatas.Count);
            //itemView.RefreshAllShownItem();
            RefreshItems();
        }

        public void RefreshItems()
        {
            for (int i = 0; i < selectAcquireItems.Count; i++)
            {
                var selectAcquireItem = selectAcquireItems[i];
                if (i >= selectAcquireFormParams.SelectAcquireItemDatas.Count)
                {
                    selectAcquireItem.gameObject.SetActive(false);
                    continue;
                }
                
                selectAcquireItem.gameObject.SetActive(true);
                selectAcquireItem.SetItemData(selectAcquireFormParams.SelectAcquireItemDatas[i],
                    OnSelectItem, i);
            }

        }
        
        // LoopGridViewItem OnGetItemByRowColumn(LoopGridView view, int itemIndex,int row,int column)
        // {
        //     if (itemIndex < 0)
        //     {
        //         return null;
        //     }
        //     
        //     var item = view.NewListViewItem("SelectAcquireItem");
        //
        //     var itemScript = item.GetComponent<SelectAcquireItem>();
        //     if (item.IsInitHandlerCalled == false)
        //     {
        //         item.IsInitHandlerCalled = true;
        //         itemScript.Init();
        //     }
        //
        //     itemScript.SetItemData(selectAcquireFormParams.SelectAcquireItemDatas[itemIndex],
        //         OnSelectItem, itemIndex, row, column);
        //     
        //     return item;
        // }
        
        public void OnSelectItem(int selectIdx)
        {
            selectAcquireFormParams.OnClick.Invoke(selectIdx);
            Close();
        }

        public void ConfirmClose()
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmUnAcquire),
                OnConfirm = () =>
                {

                    Close();
                    
                }
            });    
        }
        
        
    }
}