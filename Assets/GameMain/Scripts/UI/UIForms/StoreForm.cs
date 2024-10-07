using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SuperScrollView;
using UnityGameFramework.Runtime;
using Random = System.Random;

namespace RoundHero
{
    public class StoreFormData
    {
        public int RandomSeed;
    }
    
    public class StoreForm : UGuiForm
    {
        [SerializeField]
        private LoopGridView cardView;
        [SerializeField]
        private LoopGridView funeView;
        [SerializeField]
        private LoopGridView blessView;

        private List<StoreItemData> storeCards = new ();
        private List<StoreItemData> storeFunes = new ();
        private List<StoreItemData> storeBlesses = new ();

        private StoreFormData storeFormData;

        private System.Random random;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            cardView.InitGridView(0, OnGetStoreCardItemByRowColumn);
            blessView.InitGridView(0, OnGetStoreBlessItemByRowColumn);
            funeView.InitGridView(0, OnGetStoreFuneItemByRowColumn);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            storeFormData = (StoreFormData)userData;
            if (storeFormData == null)
            {
                Log.Warning("StoreFormData is null.");
                return;
            }

            random = new Random(storeFormData.RandomSeed);



            Refresh();
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
        }

        public void Refresh()
        {
            storeCards.Clear();
            storeBlesses.Clear();
            storeFunes.Clear();
            
            var drCards = GameEntry.DataTable.GetDataTable<DRCard>();
            var cardIDs = drCards.Select(t => t.Id).ToList();
            var randomCardSequence = MathUtility.GetRandomNum(6, 0, cardIDs.Count, random);
            var idx = 0;
            foreach (var sequence in randomCardSequence)
            {
                storeCards.Add(new StoreItemData()
                {
                    ItemData = new ItemData()
                    {
                        CardID = cardIDs[sequence],
                        ItemType = EItemType.Card,
                    },
                    Price = random.Next(Constant.Store.CardPriceRange.x, Constant.Store.CardPriceRange.y),
                    StoreIdx = idx++,
                });
            }

            cardView.SetListItemCount(storeCards.Count);
            cardView.RefreshAllShownItem();
            
            var drBlesses = GameEntry.DataTable.GetDataTable<DRBless>();
            var blessIDs = drBlesses.Select(t => t.BlessID).ToList();
            var randomBlessSequence = MathUtility.GetRandomNum(3, 0, blessIDs.Count, random);
            foreach (var sequence in randomBlessSequence)
            {
                storeBlesses.Add(new StoreItemData()
                {
                    ItemData = new ItemData()
                    {
                        BlessID = blessIDs[sequence],
                        ItemType = EItemType.Bless,
                    },
                    Price = random.Next(Constant.Store.BlessPriceRange.x, Constant.Store.BlessPriceRange.y),
                });
            }

            blessView.SetListItemCount(storeBlesses.Count);
            blessView.RefreshAllShownItem();
            
            var drFunes = GameEntry.DataTable.GetBuffs(EBuffType.Fune);
            var funeIDs = drFunes.Select(t => t.Id).ToList();
            var randomFuneSequence = MathUtility.GetRandomNum(3, 0, funeIDs.Count, random);
            foreach (var sequence in randomFuneSequence)
            {
                storeFunes.Add(new StoreItemData()
                {
                    ItemData = new ItemData()
                    {
                        FuneID = funeIDs[sequence],
                        ItemType = EItemType.Fune,
                    },
                    Price = random.Next(Constant.Store.FunePriceRange.x, Constant.Store.FunePriceRange.y),
                });
            }

            funeView.SetListItemCount(storeFunes.Count);
            funeView.RefreshAllShownItem();
        }

        public void PurseCard(int cardStoreIdx, int price)
        {
            storeCards[cardStoreIdx].IsSale = true;
            
        }
        
        LoopGridViewItem OnGetStoreCardItemByRowColumn(LoopGridView view, int itemIndex,int row,int column)
        {
            if (itemIndex < 0)
            {
                return null;
            }
            
            var item = view.NewListViewItem("StoreCardItem");
        
            var itemScript = item.GetComponent<StoreCardItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            
        
            itemScript.SetItemData(storeCards[itemIndex], PurseCard());
            
            return item;
        }
        
        
        LoopGridViewItem OnGetStoreFuneItemByRowColumn(LoopGridView view, int itemIndex,int row,int column)
        {
            if (itemIndex < 0)
            {
                return null;
            }
            
            var item = view.NewListViewItem("CommonStoreItem");
        
            var itemScript = item.GetComponent<CommonStoreItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.SetItemData(storeFunes[itemIndex]);
            
            return item;
        }
        
        LoopGridViewItem OnGetStoreBlessItemByRowColumn(LoopGridView view, int itemIndex,int row,int column)
        {
            if (itemIndex < 0)
            {
                return null;
            }
            
            var item = view.NewListViewItem("CommonStoreItem");
        
            var itemScript = item.GetComponent<CommonStoreItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.SetItemData(storeBlesses[itemIndex]);
            
            return item;
        }
    }
}