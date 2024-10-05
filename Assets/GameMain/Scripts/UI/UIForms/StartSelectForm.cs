using System.Collections.Generic;
using GameFramework.Event;
using SuperScrollView;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class StartSelectForm : UGuiForm
    {
        private ProcedureStart procedureStart;
        public LoopGridView heroIconGridView;
        public LoopGridView selectCardGridView;
        public LoopGridView inBattleGridView;
        private List<int> selectInitCards = new List<int>();
        
        private SceneEntity roleEntity;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            heroIconGridView.InitGridView(0, OnHeroIconGetItemByRowColumn);
            selectCardGridView.InitGridView(0, OnSelectCardGetItemByRowColumn);
            inBattleGridView.InitGridView(0, OnInBattleCardGetItemByRowColumn);
        }

        protected override async void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureStart = (ProcedureStart)userData;
            if (procedureStart == null)
            {
                Log.Warning("ProcedureStart is null.");
                return;
            }
            
            roleEntity = await GameEntry.Entity.ShowSceneEntityAsync("Role");
            
            GameEntry.Event.Subscribe(StartSelect_SelectHeroEventArgs.EventId, OnSelectHero);
            
            GameManager.Instance.StartSelect_HeroID = EHeroID.Normal;
            
            var drHero = GameEntry.DataTable.GetDataTable<DRHero>();
            heroIconGridView.SetListItemCount(drHero.Count);
            heroIconGridView.RefreshAllShownItem();
 
            var drCards = GameEntry.DataTable.GetDataTable<DRCard>().GetDataRows((t) =>
            {
                return t.InitCard;
            });

            foreach (var drCard in drCards)
            {
                selectInitCards.Add(drCard.Id);
            }
            
            selectCardGridView.SetListItemCount(selectInitCards.Count);
            selectCardGridView.RefreshAllShownItem();
            
            inBattleGridView.SetListItemCount(GameManager.Instance.Cards.Count);
            selectCardGridView.RefreshAllShownItem();
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Entity.HideEntity(roleEntity);
            
            GameEntry.Event.Unsubscribe(StartSelect_SelectHeroEventArgs.EventId, OnSelectHero);
        }
        
        private void OnSelectHero(object sender, GameEventArgs e)
        {
            var ne = (StartSelect_SelectHeroEventArgs)e;
            GameManager.Instance.StartSelect_HeroID = ne.HeroID;
            heroIconGridView.RefreshAllShownItem();

        }
        
        public void PVEStartGame()
        {
            GameEntry.Entity.HideEntity(procedureStart.StartSelectEntity);
            GameEntry.UI.CloseUIForm(this);
            
            var randomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            randomSeed = 94204398;//2198030
            Log.Debug("randomSeed:" + randomSeed);
            GamePlayManager.Instance.GamePlayData.RandomSeed = randomSeed;
            GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(randomSeed, EEnemyType.Normal));
        }

        LoopGridViewItem OnHeroIconGetItemByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {

            var drHero = GameEntry.DataTable.GetHero(itemIndex);
            if (drHero == null)
            {
                return null;
            }

            var item = gridView.NewListViewItem("HeroIconItem");

            var itemScript = item.GetComponent<HeroIconItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            itemScript.SetItemData(drHero, itemIndex, row, column);
            return item;
        }
        
        LoopGridViewItem OnSelectCardGetItemByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {

            var drHero = GameEntry.DataTable.GetHero(itemIndex);
            if (drHero == null)
            {
                return null;
            }

            var item = gridView.NewListViewItem("SelectCardItem");

            var itemScript = item.GetComponent<SelectCardItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
                itemScript.ClickAction = CardAddBattle;
            }
            
            itemScript.SetItemData(selectInitCards[itemIndex], itemIndex, row, column);
            return item;
        }
        
        LoopGridViewItem OnInBattleCardGetItemByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {

            var drHero = GameEntry.DataTable.GetHero(itemIndex);
            if (drHero == null)
            {
                return null;
            }

            var item = gridView.NewListViewItem("HalfCardItem");

            var itemScript = item.GetComponent<HalfCardItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
                itemScript.ClickAction = CardRemoveBattle;
            }
            
            itemScript.SetItemData(GameManager.Instance.Cards[itemIndex], itemIndex, row, column);
            return item;
        }

        public void CardAddBattle(int cardID)
        {
            if (GameManager.Instance.Cards.Contains(cardID))
            {
                return;
            }
            
            for (int i = 0; i < 3; i++)
            {
                GameManager.Instance.Cards.Add(cardID);
            }
            
            inBattleGridView.SetListItemCount(GameManager.Instance.Cards.Count);
            inBattleGridView.RefreshAllShownItem();
        }

        public void CardRemoveBattle(int cardID)
        {
            GameManager.Instance.Cards.RemoveAll((BattleCardID) => BattleCardID == cardID);
            
            inBattleGridView.SetListItemCount(GameManager.Instance.Cards.Count);
            inBattleGridView.RefreshAllShownItem();
        }
        
        // //public Material[] TextMaterials;//所有FontAsset的材质球
        // public Texture2D TextTexture2D;//复制出的图集
        // public Texture2D TextPng;//保存的png
        // public void ReplaceTextTexture()
        // {
        //     //创建用于压缩的png
        //     byte[] bytes = TextTexture2D.EncodeToPNG();
        //     System.IO.File.WriteAllBytes("Assets/font.png", bytes);
        //     // TextPng = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/font.png");
        //     // //批量赋值，或者只用上面的代码创建png，手动赋值
        //     // foreach (var material in TextMaterials)
        //     // {
        //     //     material.SetTexture("_MainTex",TextPng);
        //     // }
        //     AssetDatabase.Refresh();
        // }

        
    }
}