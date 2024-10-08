using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoundHero
{
    
    public class FunesView : MonoBehaviour
    {
        [SerializeField]
        private LoopGridView funeView;

        private Dictionary<int, int>  funeDict = new ();
        private List<PlayerCommonItemData> funes = new ();

        //private List<int> cardIdxs = new List<int>();


        private void Awake()
        {
            funeView.InitGridView(0, OnGetCardItemByRowColumn);
        }

        public void Init(List<int> funeIdxs)
        {
            //this.cardIdxs = cardIdxs;
            this.funes.Clear();
            this.funeDict.Clear();
            var idx = 0;
            foreach (var funeIdx in funeIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                this.funes.Add(new PlayerCommonItemData()
                {
                    ItemIdx = funeIdx,
                    CommonItemData = new CommonItemData()
                    {
                        ItemType = EItemType.Fune,
                        FuneID = drBuff.Id,

                    }
                });
                funeDict.Add(funeIdx, idx++);
            }

            funeView.SetListItemCount(this.funes.Count);
            funeView.RefreshAllShownItem();

        }
        
        LoopGridViewItem OnGetCardItemByRowColumn(LoopGridView view, int itemIndex, int row, int column)
        {
            if (itemIndex < 0)
            {
                return null;
            }
            
            var item = view.NewListViewItem("PlayerCommonItem");
        
            var itemScript = item.GetComponent<PlayerCommonItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            
        
            itemScript.SetItemData(funes[itemIndex]);
            
            return item;
        }
    }
}