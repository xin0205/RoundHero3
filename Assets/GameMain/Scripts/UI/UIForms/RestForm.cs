
using System.Collections.Generic;
using UGFExtensions.Await;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class RestForm : UGuiForm
    {
        [SerializeField] private Button addHeart;
        [SerializeField] private Button addMaxHP;
        
        private SceneEntity restSceneEntity;
        
        protected override async void OnOpen(object userData)
        {
            base.OnOpen(userData);

            // var heroAttribute = BattleHeroManager.Instance.BattleHeroData.Attribute;
            // var curHeart = heroAttribute.GetAttribute(EHeroAttribute.CurHeart);
            // var maxHeart = heroAttribute.GetAttribute(EHeroAttribute.MaxHeart);
            // addHeart.interactable = curHeart < maxHeart;

            restSceneEntity = await GameEntry.Entity.ShowSceneEntityAsync("Rest");

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
            
            GameEntry.Entity.HideEntity(restSceneEntity);
        }
        
        public void AddHeart()
        {
            var heroAttribute = BattleHeroManager.Instance.BattleHeroData.Attribute;
            var curHeart = heroAttribute.GetAttribute(EHeroAttribute.CurHeart);
            var maxHeart = heroAttribute.GetAttribute(EHeroAttribute.MaxHeart);
            if (curHeart >= maxHeart)
            {
                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_MaxHeart);
                return;
            }
            
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {

                Message = GameEntry.Localization.GetLocalizedString(Constant.Localization.Message_ConfirmAddHeart, Constant.Rest.AddHeart),
                OnConfirm = () =>
                {
                    var heroAttribute = BattleHeroManager.Instance.BattleHeroData.Attribute;
                    var curHeart = heroAttribute.GetAttribute(EHeroAttribute.CurHeart);
                    var maxHeart = heroAttribute.GetAttribute(EHeroAttribute.MaxHeart);

                    heroAttribute.SetAttribute(EHeroAttribute.CurHeart, curHeart + 1);
                    GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());
                    Close();
                }
                
            });
            
            
            
        }
        
        public void AddMaxEnergy()
        {
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetLocalizedString(Constant.Localization.Message_ConfirmAddMaxEnergy, Constant.Rest.AddMaxEnergy),
                OnConfirm = () =>
                {
                    BattleHeroManager.Instance.BattleHeroData.BaseMaxHP += Constant.Rest.AddMaxEnergy;
                    BattleHeroManager.Instance.BattleHeroData.CurHP += Constant.Rest.AddMaxEnergy;
            
                    GameEntry.Event.Fire(null, RefreshPlayerInfoEventArgs.Create());
                    Close();
                }
                
            });
            
            

        }

        private CardsForm cardsForm;
        public async void CardAddFune()
        {
            var uiForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardsForm, new CardsFormParams()
            {
                Tips = GameEntry.Localization.GetString(Constant.Localization.Tips_CardAddFune),
                ShowCardTypes = new List<ECardType>()
                {
                    ECardType.Unit,
                },
                OnClickAction = CardAddFuneClickAction,
                IsShowAllFune = true,
                
            });
            
            cardsForm = uiForm.Logic as CardsForm;
        }

        public void CardAddFuneClickAction(int cardIdx)
        {
            var cardData = CardManager.Instance.GetCard(cardIdx);
            if (cardData.MaxFuneCount >= Constant.Card.MaxFuneCount)
            {
                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_MaxFuneCount);
                return;
            }
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmAddFune),
                OnConfirm = () =>
                {
                    
                    var cardData = CardManager.Instance.GetCard(cardIdx);
                    cardData.MaxFuneCount += 1;
                    GameEntry.UI.CloseUIForm(cardsForm);
                    Close();
                }
            });
            
            
        }
        
        
    }
}