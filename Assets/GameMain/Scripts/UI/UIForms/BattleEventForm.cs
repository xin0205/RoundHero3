using System;
using System.Collections.Generic;
using UGFExtensions.Await;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleEventFormData
    {
        public int RandomSeed;
        public BattleEventData BattleEventData;
    }
    
    public class BattleEventForm : UGuiForm
    {
        //private BattleEventData battleEventData;

        [SerializeField]
        private List<BattleEventItem> BattleEventItems;
        
        private BattleEventFormData battleEventFormData;
        private SceneEntity sceneEntity;

        private int curAcquireIdx;
        private int curItemIdx;
        private int acquireIdx;
        
        protected async override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            battleEventFormData = (BattleEventFormData)userData;
            if (battleEventFormData == null)
            {
                Log.Warning("TreasureFormData is null.");
                return;
            }

            //battleEventData = BattleEventManager.Instance.GenerateTreasureEvent(_battleEventFormData.RandomSeed);

            //text.text = battleEventData.BattleEventExpressionType + "-" + battleEventData.BattleEvent;

            for (int i = 0; i <  BattleEventItems.Count; i++)
            {
                BattleEventItems[i].Init(i, battleEventFormData.BattleEventData.BattleEventItemDatas[i], OnClickItem);
            }

            sceneEntity = await GameEntry.Entity.ShowSceneEntityAsync("Event");
        }

        private void NextAcquireItem()
        {
            curAcquireIdx += 1;
            if (curAcquireIdx >= acquireIdx)
            {
                
                return;
            }
            
            var battleEventItemData = GetCurrentBattleEventItemData();

            switch (battleEventItemData.EventType)
            {
                case EEventType.Card_Remove:
                    RemoveCard();
                    break;
                case EEventType.Card_Change:
                    ChangeCard();
                    break;
                case EEventType.Card_Copy:
                    CopyCard();
                    break;
                case EEventType.Random_UnitCard:
                case EEventType.Random_TacticCard:
                case EEventType.Random_Fune:
                case EEventType.Random_Bless:
                    ShowSelectAcquireForm();
                    break;
                case EEventType.Appoint_UnitCard:
                case EEventType.Appoint_TacticCard:
                case EEventType.Appoint_Fune:
                case EEventType.Appoint_Bless:
                case EEventType.AddCoin:
                case EEventType.AddHeroMaxHP:
                case EEventType.AddHeroCurHP:
                case EEventType.NegativeCard:
                case EEventType.SubCoin:
                case EEventType.SubHeroMaxHP:
                case EEventType.SubHeroCurHP:
                    AcquireItem();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnClickItem(int itemIdx)
        {
            Close();
            curItemIdx = itemIdx;
            var battleEventItemDatas = battleEventFormData.BattleEventData.BattleEventItemDatas[curItemIdx];
            acquireIdx = battleEventItemDatas.Count;

            curAcquireIdx = -1;
            NextAcquireItem();

        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
            
            GameEntry.Entity.HideEntity(sceneEntity);
        }

        private void AcquireItem(int selectIdx = 0)
        {
            var battleEventItemData = GetCurrentBattleEventItemData();
            BattleEventManager.Instance.AcquireEventItem(battleEventItemData, selectIdx);
            
            if (Constant.BattleEvent.EventTypeItemTypeMap.ContainsKey(battleEventItemData.EventType))
            {
                var acquireFormData = new AcquireFormData()
                {
                    
                    ItemType = Constant.BattleEvent.EventTypeItemTypeMap[battleEventItemData.EventType],
                    ClickCloseAction = CheckClose,

                };

                if (Constant.Hero.AttributeItemTypes.Contains(acquireFormData.ItemType))
                {
                    var value = battleEventItemData.EventValues[0];
                    acquireFormData.Value = value > 0 ? "+" + value : value.ToString();
                }
                else
                {
                    acquireFormData.ItemID = battleEventItemData.EventValues[0];
                }
                
                GameEntry.UI.OpenUIForm(UIFormId.AcquireForm, acquireFormData);
            }

            
        }

        private void ShowSelectAcquireForm()
        {
            var battleEventItemData = GetCurrentBattleEventItemData();
            
            var selectAcquireFormParams = new SelectAcquireFormParams();

            foreach (var value in battleEventItemData.EventValues)
            {
                var selectAcquireItemData = new SelectAcquireItemData()
                {
                    ItemType = Constant.BattleEvent.EventTypeItemTypeMap[battleEventItemData.EventType],
                    ItemID = value,
                };
                selectAcquireFormParams.SelectAcquireItemDatas.Add(selectAcquireItemData);
                
            }

            selectAcquireFormParams.OnClick = OnSelectAcquireClick;
            
            
            GameEntry.UI.OpenUIForm(UIFormId.SelectAcquireForm, selectAcquireFormParams);
            
        }

        private void OnSelectAcquireClick(int selectIdx)
        {
            AcquireItem(selectIdx);
            // var battleEventItemDatas = battleEventData.BattleEventItemDatas[curItemIdx];
            // var battleEventItemData = battleEventItemDatas[curAcquireIdx];
            // BattleEventManager.Instance.AcquireEventItem(battleEventItemData, selectIdx);
        }

        private CardsForm cardsForm;
        private async void CopyCard()
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_CopyCard),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction = CopyCard,
                IsShowAllFune = false,
                OnCloseAction = () =>
                {
                    CloseCardsFormConfirm(Constant.Localization.Message_ConfirmUnCopyCard);

                }

            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }

        private void CloseCardsFormConfirm(string message)
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(message),
                OnConfirm = () =>
                {
                    GameEntry.UI.CloseUIForm(cardsForm);
                    CheckClose();
                },
                
            });
        }

        private BattleEventItemData GetCurrentBattleEventItemData()
        {
            var battleEventItemDatas = battleEventFormData.BattleEventData.BattleEventItemDatas[curItemIdx];
            var battleEventItemData = battleEventItemDatas[curAcquireIdx];

            return battleEventItemData;
        }
        
        private void CopyCard(int cardIdx)
        {
            var battleEventItemData = GetCurrentBattleEventItemData();
            battleEventItemData.EventValues.Add(cardIdx);
            
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmCopyCard),
                OnConfirm = () =>
                {
                    AcquireItem();
                    GameEntry.UI.CloseUIForm(cardsForm);

                },
                
            });
            
            
        }

        private async void RemoveCard()
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_RemoveCard),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction = RemoveCard,
                IsShowAllFune = false,
                OnCloseAction = () =>
                {
                    CloseCardsFormConfirm(Constant.Localization.Message_ConfirmUnRemoveCard);

                }
                
            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }
        
        private void RemoveCard(int cardIdx)
        {
            var battleEventItemData = GetCurrentBattleEventItemData();
            battleEventItemData.EventValues.Add(cardIdx);
            
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmRemoveCard),
                OnConfirm = () =>
                {
                    AcquireItem();
                    GameEntry.UI.CloseUIForm(cardsForm);
                    CheckClose();
                }
            });
            
            
        }
        
        private async void ChangeCard()
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_ChangeCard),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                    ECardType.Tactic,
                    ECardType.State,
                },
                OnClickAction = ChangeCard,
                IsShowAllFune = false,
                OnCloseAction = () =>
                {
                    CloseCardsFormConfirm(Constant.Localization.Message_ConfirmUnChangeCard);

                }
                
            });
            cardsForm = uiForm.Logic as CardsForm;
            
        }
        
        private void ChangeCard(int cardIdx)
        {
            var battleEventItemData = GetCurrentBattleEventItemData();
            battleEventItemData.EventValues.Add(cardIdx);
            
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmChangeCard),
                OnConfirm = () =>
                {
                    AcquireItem();
                    GameEntry.UI.CloseUIForm(cardsForm);

                }
            });
            
            
        }
        
        private void CheckClose()
        {
            NextAcquireItem();
        }
    }
}