using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    
    public class FunesView : MonoBehaviour
    {
        [SerializeField]
        private LoopGridView funeView;

        private Dictionary<int, int>  funeDict = new ();
        private List<PlayerCommonItemData> funes = new ();

        private GameObject parentForm;

        //private List<int> cardIdxs = new List<int>();


        private void Awake()
        {
            funeView.InitGridView(0, OnGetCardItemByRowColumn);
        }

        public void Init(List<int> funeIdxs, GameObject parentForm)
        {
            this.parentForm = parentForm;
            
            //this.cardIdxs = cardIdxs;
            this.funes.Clear();
            this.funeDict.Clear();
            var idx = 0;
            foreach (var funeIdx in funeIdxs)
            {
                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                this.funes.Add(new PlayerCommonItemData()
                {
                    ItemIdx = funeIdx,
                    CommonItemData = new CommonItemData()
                    {
                        ItemType = EItemType.Fune,
                        FuneID = funeData.FuneID,

                    }
                });
                funeDict.Add(funeIdx, idx++);
            }

            funeView.SetListItemCount(this.funes.Count);
            funeView.RefreshAllShownItem();

        }

        public void Refresh()
        {
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
            
            
        
            itemScript.SetItemData(funes[itemIndex], OnPointDown, OnPointUp);
            
            return item;
        }
        
        public PlayerCommonItem TempPlayerCommonItem;
        [SerializeField]
        private PlayerCommonItem playerCommonItem;
        
        
        public void OnPointDown(int itemIdx)
        {
            TempPlayerCommonItem = GameObject.Instantiate(playerCommonItem, this.parentForm.transform);
            TempPlayerCommonItem.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            TempPlayerCommonItem.GetComponent<Image>().raycastTarget = false;
            TempPlayerCommonItem.gameObject.SetActive(true);
            TempPlayerCommonItem.SetItemData(funes[funeDict[itemIdx]], null, null);
        }
        
        public void OnPointUp()
        {
            //GameObject.DestroyImmediate(TempPlayerCommonItem.gameObject);
            if (TempPlayerCommonItem != null)
            {
                GameObject.DestroyImmediate(TempPlayerCommonItem.gameObject);
                TempPlayerCommonItem = null;
            }
        }
        
        private void Update()
        {
            if (TempPlayerCommonItem != null)
            {
                TempPlayerCommonItem.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
}