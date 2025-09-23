
using System.Collections.Generic;
using UGFExtensions.Await;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace RoundHero
{
    public class BattleModeRewardForm : UGuiForm
    {
        private ProcedureStart procedureBattle;
        
        private List<SelectAcquireItemData> selectAcquireItemDatas = new();
        [SerializeField] private List<SelectAcquireItem> selectAcquireItems;
        private Dictionary<EItemType, int> RewardRatios = new Dictionary<EItemType, int>();
        private SceneEntity startSelectEntity;
        private PlayerInfoForm playerInfoForm;
        //private List<int> selectedIdxs = new List<int>();
        [SerializeField] private Text tips;
        [SerializeField] private Text heroHP;

        private Random random;
        
        protected override async void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureBattle = (ProcedureStart)userData;
            
            RewardRatios.Clear();
            selectAcquireItemDatas.Clear();
            
            startSelectEntity = await GameEntry.Entity.ShowSceneEntityAsync("StartSelect");
            // var formResult = await GameEntry.UI.OpenUIFormAsync(UIFormId.PlayerInfoForm, this);
            // playerInfoForm = formResult.Logic as PlayerInfoForm;

            var rewardRandomSeed = GamePlayManager.Instance.GamePlayData.BattleModeProduce.RewardRandomSeed;
            random = new Random(rewardRandomSeed);

            if (GamePlayManager.Instance.GamePlayData.BattleModeProduce.selectAcquireItemDatas.Count <= 0)
            {
                var ratio = 0;
                foreach (var kv in Constant.Battle.BattleModeRewardRatios)
                {
                    ratio += kv.Value;
                    RewardRatios.Add(kv.Key, ratio);
                }
                var drBlesses = GameEntry.DataTable.GetDataTable<DRBless>();
                var blessIDs = new List<int>();
                foreach (var drBless in drBlesses)
                {
                    bool isContinue = false;
                    foreach (var kv in PlayerManager.Instance.PlayerData.BlessDatas)
                    {
                        if (kv.Value.BlessID == drBless.BlessID)
                        {
                            isContinue = true;
                            break;
                        }
                            
                    }

                    if(isContinue)
                        continue;
                    
                    blessIDs.Add(drBless.Id);
                }
                
                var test = new SelectAcquireItemData()
                {
                    ItemType = EItemType.AddCardFuneSlot,
                    ItemID = 0,
                };
                selectAcquireItemDatas.Add(test);
                test = new SelectAcquireItemData()
                {
                    ItemType = EItemType.AddCardFuneSlot,
                    ItemID = 0,
                };
                selectAcquireItemDatas.Add(test);
                test = new SelectAcquireItemData()
                {
                    ItemType = EItemType.AddCardFuneSlot,
                    ItemID = 0,
                };
                selectAcquireItemDatas.Add(test);
                //
                // var test2 = new SelectAcquireItemData()
                // {
                //     ItemType = EItemType.RemoveCard,
                // };
                // selectAcquireItemDatas.Add(test2);
                //
                // var test3 = new SelectAcquireItemData()
                // {
                //     ItemType = EItemType.AddMaxHP,
                // };
                // selectAcquireItemDatas.Add(test3);
                
                for (int i = 0; i < 8; i++)
                {
                    var randomType = random.Next(0, 100);
                    foreach (var kv in RewardRatios)
                    {
                        if (randomType < kv.Value)
                        {
                            var selectAcquireItemData = new SelectAcquireItemData()
                            {
                                ItemType = kv.Key,
                            };
                            selectAcquireItemDatas.Add(selectAcquireItemData);
                            var randomIdx = 0;
                            switch (kv.Key)
                            {
                                case EItemType.UnitCard:
                                    var drUnitCards = GameEntry.DataTable.GetCards(ECardType.Unit);
                                    randomIdx = random.Next(0, drUnitCards.Length);
                                    selectAcquireItemData.ItemID = drUnitCards[randomIdx].Id;
                                    break;
                                case EItemType.TacticCard:
                                    var drTacticCards = GameEntry.DataTable.GetCards(ECardType.Tactic);
                                    randomIdx = random.Next(0, drTacticCards.Length);
                                    selectAcquireItemData.ItemID = drTacticCards[randomIdx].Id;
                                    break;
                                case EItemType.Bless:
                                    
                                    randomIdx = random.Next(0, blessIDs.Count);
                                    selectAcquireItemData.ItemID = drBlesses[randomIdx].Id;
                                    blessIDs.RemoveAt(randomIdx);
                                    break;
                                case EItemType.Fune:
                                    var drFunes = GameEntry.DataTable.GetBuffs(EBuffType.Fune);
                                    randomIdx = random.Next(0, drFunes.Count);
                                    selectAcquireItemData.ItemID = drFunes[randomIdx].Id;
                                    break;
                                case EItemType.AddMaxHP:
                                    break;
                                case EItemType.RemoveCard:
                                    break;
                                case EItemType.AddCardFuneSlot:
                                    break;

                            }

                            break;

                        }
                    }
                }
            }
            else
            {
                foreach (var selectAcquireItemData in GamePlayManager.Instance.GamePlayData.BattleModeProduce.selectAcquireItemDatas) 
                {
                    selectAcquireItemDatas.Add(selectAcquireItemData.Copy());
                }
            }

            RefreshItems();
            SetTips();
            RefreshHP();
        }
        
        public void RefreshItems()
        {
            for (int i = 0; i < selectAcquireItems.Count; i++)
            {
                var selectAcquireItem = selectAcquireItems[i];
                if (i >= selectAcquireItemDatas.Count)
                {
                    selectAcquireItem.gameObject.SetActive(false);
                    continue;
                }
                
                selectAcquireItem.gameObject.SetActive(true);
                // selectAcquireItemDatas[i].IsSelected =
                //     selectedIdxs.Contains(i);
                
                selectAcquireItem.SetItemData(selectAcquireItemDatas[i],
                    OnSelectItem, i);

                

            }

        }

        private void RefreshHP()
        {
            heroHP.text =
                BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" +
                BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP;

        }

        protected void SetTips()
        {
            tips.text = GameEntry.Localization.GetLocalizedString(Constant.Localization.Tips_BattleModeRewardSelectItem,
                selectAcquireItemDatas.FindAll(data => data.IsSelected).Count, Constant.BattleMode.MaxRewardCount);
        }

        public void OpenCardAndFuneForm()
        {
            GameEntry.UI.OpenUIForm(UIFormId.CardAndFuneForm);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Entity.HideEntity(startSelectEntity);
            //GameEntry.UI.CloseUIForm(playerInfoForm);
        }

        private void SelectItem(int selectIdx)
        {
            var selectAcquireItem = selectAcquireItems[selectIdx];
            selectAcquireItem.SetSelect();
            var selectAcquireItemData = selectAcquireItemDatas[selectIdx];
            selectAcquireItemData.IsSelected = true;
            SetTips();
            RefreshHP();
        }

        public void OnSelectItem(int selectIdx)
        {
            if (selectAcquireItemDatas.FindAll(data => data.IsSelected).Count >= Constant.BattleMode.MaxRewardCount)
            {
                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_RewardMaxCount,
                    Constant.BattleMode.MaxRewardCount);
                return;
            }
            var selectAcquireItemData = selectAcquireItemDatas[selectIdx];
            
           
            
            var idx = 0;
            switch (selectAcquireItemData.ItemType)
            {
                case EItemType.TacticCard:
                case EItemType.UnitCard:
                    idx = CardManager.Instance.GetIdx();
                    CardManager.Instance.CardDatas.Add(idx, new Data_Card(idx, selectAcquireItemData.ItemID));
                    SelectItem(selectIdx);
                    break;
                case EItemType.Bless:
                    idx = BlessManager.Instance.GetIdx();
                    var drBless = GameEntry.DataTable.GetBless(selectAcquireItemData.ItemID);
                    BlessManager.Instance.BlessDatas.Add(idx, new Data_Bless(idx, drBless.BlessID));
                    GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());
                    SelectItem(selectIdx);
                    break;
                case EItemType.Fune:
                    idx = FuneManager.Instance.GetIdx();
                    FuneManager.Instance.FuneDatas.Add(idx, new Data_Fune(idx, selectAcquireItemData.ItemID));
                    BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(idx);
                    SelectItem(selectIdx);
                    break;
                
                case EItemType.RemoveCard:
                    ShowRemoveCard(selectIdx);
                    break;
                case EItemType.AddCardFuneSlot:
                    ShowAddCardFuneSlot(selectIdx);
                    break;
                case EItemType.AddMaxHP:
                    HeroManager.Instance.BattleHeroData.BaseMaxHP += 1;
                    HeroManager.Instance.BattleHeroData.CurHP = HeroManager.Instance.BattleHeroData.MaxHP;
                    SelectItem(selectIdx);
                    break;
                default:
                    break;
            }

        }
        
        
        public void ConfirmReward()
        {
            // foreach (var selectAcquireItemData in selectAcquireItemDatas)
            // {
            //     if(!selectAcquireItemData.IsSelected)
            //         continue;
            //     
            // }
            
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.selectAcquireItemDatas.Clear();
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session += 1;
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage = BattleModeStage.Battle;
            
            GameEntry.UI.CloseUIForm(this);
            

            
            GameEntry.Event.Fire(null,
                GamePlayStartGameEventArgs.Create());
        }

        public void Back()
        {
            Close();

            GamePlayManager.Instance.GamePlayData.BattleModeProduce.selectAcquireItemDatas.Clear();
            foreach (var selectAcquireItemData in selectAcquireItemDatas) 
            {
                GamePlayManager.Instance.GamePlayData.BattleModeProduce.selectAcquireItemDatas.Add(selectAcquireItemData.Copy());
            }
            
            procedureBattle.Start();
        }
        
        private CardsForm cardsForm;

        private async void ShowRemoveCard(int selectIdx)
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                ClickParams = selectIdx,
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_RemoveCard),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction =  RemoveCard,
                IsShowAllFune = false,
                OnCloseAction = () =>
                {
                    CloseCardsFormConfirm(Constant.Localization.Message_ConfirmUnRemoveCard);

                }
                
            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }
        
        private void RemoveCard(int cardIdx, object clickParams)
        {
            var selectIdx = (int)clickParams;
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmRemoveCard),
                OnConfirm = () =>
                {
                    SelectItem(selectIdx);
        
                    CardManager.Instance.RemoveCard(cardIdx);
                    GameEntry.UI.CloseUIForm(cardsForm);

                }
            });
            
            
        }
        
        private void CloseCardsFormConfirm(string message)
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(message),
                OnConfirm = () =>
                {
                    GameEntry.UI.CloseUIForm(cardsForm);

                },
                
            });
        }
        
        
        private async void ShowAddCardFuneSlot(int selectIdx)
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                ClickParams = selectIdx,
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_AddCardFuneSlot),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction =  AddCardFuneSlot,
                IsShowAllFune = false,
                OnCloseAction = () =>
                {
                    CloseCardsFormConfirm(Constant.Localization.Message_ConfirmUnAddCardFuneSlot);

                }
                
            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }
        
        private void AddCardFuneSlot(int cardIdx, object clickParams)
        {
            var selectIdx = (int)clickParams;
            
            var cardData = CardManager.Instance.GetCard(cardIdx);
            if (cardData.MaxFuneCount >= Constant.Card.MaxFuneCount)
            {
                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_MaxFuneCount);
                return;
            }
            
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmAddCardFuneSlot),
                OnConfirm = () =>
                {
                    var cardData = CardManager.Instance.GetCard(cardIdx);
                    cardData.MaxFuneCount += 1;
                    SelectItem(selectIdx);
                    
                    GameEntry.UI.CloseUIForm(cardsForm);

                }
            });
            
            
        }
        
       
    }
}