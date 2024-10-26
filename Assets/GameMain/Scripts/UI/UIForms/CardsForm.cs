
using System.Collections.Generic;
using System.Linq;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class CardsForm : UGuiForm
    {
        [SerializeField]
        public CardsView CardsViews;
        
        [SerializeField]
        public FunesView FunesViews;
        
        // [SerializeField]
        // private GameObject cardGO;
        //
        // [SerializeField]
        // private GameObject funeGO;
        //
        // [SerializeField]
        // private GameObject funeTag;
        //
        // [SerializeField]
        // private Toggle switchViewToggle;
        
        [SerializeField]
        private Toggle unitToggle;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            
            FunesViews.Init(this.gameObject);

            unitToggle.isOn = false;
            unitToggle.isOn = true;

            // switchViewToggle.isOn = true;
            // switchViewToggle.isOn = false;
            GameEntry.Event.Subscribe(RefreshCardsFormEventArgs.EventId, OnRefreshCardsForm);
        }

        

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshCardsFormEventArgs.EventId, OnRefreshCardsForm);
            GameManager.Instance.CardsForm_EquipFuneIdxs.Clear();
        }

        public void ConfirmClose()
        {
            if (GameManager.Instance.CardsForm_EquipFuneIdxs.Count <= 0)
            {
                Close();
                return;
            }
            GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            {
                Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmEquipFune),
                OnConfirm = () =>
                {
                    Close();
                },
            });
        }
        
        public void OnRefreshCardsForm(object sender, GameEventArgs e)
        {
            RefreshView();
        }

        public void RefreshView()
        {
            FunesViews.Refresh();
            CardsViews.Refresh();
        }
        
        
        // public void SwitchView(bool isOn)
        // {
        //     cardGO.SetActive(!isOn);
        //     funeGO.SetActive(isOn);
        //     // funeTag.SetActive(!isOn);
        //
        //     if (isOn)
        //     {
        //         
        //     }
        //     else
        //     {
        //         unitToggle.isOn = false;
        //         unitToggle.isOn = true;
        //     }
        // }
        
    }
}