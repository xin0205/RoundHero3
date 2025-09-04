using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    // public class SelectCardData
    // {
    //     public int CardIdx;
    //     public int CardID;
    // }
    
    public class StartSelectForm : UGuiForm
    {
        private ProcedureStart procedureStart;
        //public LoopGridView heroIconGridView;
        public LoopGridView selectCardGridView;
        public LoopGridView inBattleGridView;
        private List<int> selectInitCards = new List<int>();

        [SerializeField] private Text heroDesc;
        
        [SerializeField] private Text energy;
        
        [SerializeField] private Text heroName;
        
        [SerializeField] private List<GameObject> heroHPs;
        
        private HeroSceneEntity heroSceneEntity;

        private int startGameRandomSeed;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            //heroIconGridView.InitGridView(0, OnHeroIconGetItemByRowColumn);
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
            
            selectInitCards.Clear();
            GameManager.Instance.TmpHeroID = 0;
            GameManager.Instance.TmpInitCards.Clear();
            
            
            //heroSceneEntity = await GameEntry.Entity.ShowHeroSceneEntityAsync();

            //GameEntry.Event.Subscribe(StartSelect_SelectHeroEventArgs.EventId, OnSelectHero);

            
            // var drHero = GameEntry.DataTable.GetDataTable<DRHero>();
            // heroIconGridView.SetListItemCount(drHero.Count);
            // heroIconGridView.RefreshAllShownItem();
 
            var drCards = GameEntry.DataTable.GetDataTable<DRCard>().GetDataRows((t) =>
            {
                return t.InitCard;
            });

            foreach (var drCard in drCards)
            {
                selectInitCards.Add(drCard.Id);
            }
            
            GameManager.Instance.TmpInitCards = DataManager.Instance.DataGame.User.DefaultInitSelectCards;
            
            selectCardGridView.SetListItemCount(selectInitCards.Count);
            selectCardGridView.RefreshAllShownItem();

            
            inBattleGridView.SetListItemCount(GameManager.Instance.TmpInitCards.Count);
            selectCardGridView.RefreshAllShownItem();
            
            GameEntry.Event.Fire(null, StartSelect_SelectHeroEventArgs.Create(0));
            
            
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            //GameEntry.Entity.HideEntity(heroSceneEntity);
            GameEntry.Entity.HideEntity(procedureStart.StartSelectEntity);
            
            //GameEntry.Event.Unsubscribe(StartSelect_SelectHeroEventArgs.EventId, OnSelectHero);
        }


        
        private void OnSelectHero(object sender, GameEventArgs e)
        {
            var ne = (StartSelect_SelectHeroEventArgs)e;
            var heroID = ne.HeroID;
            //GameManager.Instance.TmpHeroID = ne.HeroID;
            //heroIconGridView.RefreshAllShownItem();
            
            //heroSceneEntity.ShowDisplayHeroEntity(heroID);

            var drHero = GameEntry.DataTable.GetHero(heroID);
            
            var heroDescStr =
                Utility.Text.Format(Constant.Localization.HeroDesc, heroID); 
            
            heroDescStr = GameEntry.Localization.GetString(heroDescStr);

            heroDesc.text = GameUtility.GetStrByValues(heroDescStr, drHero.Values1, true);

            energy.text = drHero.HP.ToString();
            
            var heroNameStr =
                Utility.Text.Format(Constant.Localization.HeroName, heroID); 
            
            heroName.text = GameEntry.Localization.GetString(heroNameStr);

            for (int i = 0; i < heroHPs.Count; i++)
            {
                if (i < drHero.Heart)
                {
                    heroHPs[i].SetActive(true);
                }
                else
                {
                    heroHPs[i].SetActive(false);
                }
            }


        }
        
        public void PVEStartGame()
        {
            // if (GameManager.Instance.TmpInitCards.Count < Constant.Battle.InitCardMaxCount)
            // {
            //     GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_InitCardCount,
            //         Constant.Battle.InitCardMaxCount);
            //     return;
            // }
            //
            //
            // GameEntry.UI.CloseUIForm(this);
            //
            //
            // startGameRandomSeed = 94204398;//2198030
            // Log.Debug("randomSeed:" + startGameRandomSeed);
            // //GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            //
            // GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            //     
            // GamePlayManager.Instance.GamePlayData.GameMode = EGamMode.PVE;
            // GamePlayManager.Instance.GamePlayData.BattleData.GameDifficulty = EGameDifficulty.Difficulty1;
            //     
            // GamePlayManager.Instance.GamePlayData.PVEType = EPVEType.Battle;
            // GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session = 0;
            // GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage = BattleModeStage.Battle;
            //
            // GameEntry.Event.Fire(null,
            //     GamePlayInitGameEventArgs.Create());


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

            var item = gridView.NewListViewItem("SelectCardItem");

            var itemScript = item.GetComponent<SelectCardItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
                //CardAddOrRemoveBattle;
                itemScript.ClickAction = CardAddBattle;
            }
            
            itemScript.SetItemData(selectInitCards[itemIndex], itemIndex, row, column);
            
            return item;
        }
        
        LoopGridViewItem OnInBattleCardGetItemByRowColumn(LoopGridView gridView, int itemIndex,int row,int column)
        {
            var item = gridView.NewListViewItem("HalfCardItem");

            var itemScript = item.GetComponent<HalfCardItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
                itemScript.ClickAction = CardRemoveBattle;
            }
            
            itemScript.SetItemData(GameManager.Instance.TmpInitCards[itemIndex], itemIndex, row, column);
            return item;
        }

        public void CardAddOrRemoveBattle(int cardID)
        {
            if (GameManager.Instance.TmpInitCards.Contains(cardID))
            {
                CardRemoveBattle(cardID);
            }
            else
            {
                CardAddBattle(cardID);
            }
        }

        public void CardAddBattle(int cardID)
        {
            // if (GameManager.Instance.TmpInitCards.Contains(cardID))
            // {
            //     return;
            // }

            if (GameManager.Instance.TmpInitCards.Count >= Constant.Battle.InitCardMaxCount)
            {

                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_InitCardMaxCount,
                    Constant.Battle.InitCardMaxCount);
                return;
            }
            
            for (int i = 0; i < Constant.Battle.SelectInitCardEachCount; i++)
            {
                GameManager.Instance.TmpInitCards.Add(cardID);
            }
            
            inBattleGridView.SetListItemCount(GameManager.Instance.TmpInitCards.Count);
            inBattleGridView.RefreshAllShownItem();
            inBattleGridView.GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
        }

        public void CardRemoveBattle(int cardSortIdx)
        {
            GameManager.Instance.TmpInitCards.RemoveAt(cardSortIdx);
            
            //GameManager.Instance.TmpInitCards.RemoveAll((BattleCardID) => BattleCardID == cardID);
            
            inBattleGridView.SetListItemCount(GameManager.Instance.TmpInitCards.Count);
            inBattleGridView.RefreshAllShownItem();
            inBattleGridView.GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
            
            selectCardGridView.RefreshAllShownItem();
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

        public void Back()
        {
            
            GameEntry.UI.CloseUIForm(this);

            procedureStart.Start();
        }

        public void StartTest()
        {
            if (GameManager.Instance.TmpInitCards.Count < Constant.Battle.InitCardMaxCount)
            {
                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_InitCardCount,
                    Constant.Battle.InitCardMaxCount);
                return;
            }
            
            
            GameEntry.UI.CloseUIForm(this);
            
            //startGameRandomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            startGameRandomSeed = 6036588;//94204398;//2198030
            // Log.Debug("randomSeed:" + startGameRandomSeed);
            // GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            // GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(startGameRandomSeed, EGameDifficulty.Difficulty1));
            
            GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            GamePlayManager.Instance.GamePlayData.GameMode = EGamMode.PVE;
            GamePlayManager.Instance.GamePlayData.PVEType = EPVEType.Test;


            GameEntry.Event.Fire(null,
                GamePlayInitGameEventArgs.Create());

        }
    }
}