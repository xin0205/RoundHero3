
using System.Collections.Generic;
using System.Linq;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class CardAndFuneForm : UGuiForm
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

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            CardsViews.Init(null, true);
            FunesViews.Init(this.gameObject);

            

            // switchViewToggle.isOn = true;
            // switchViewToggle.isOn = false;
            GameEntry.Event.Subscribe(RefreshCardsFormEventArgs.EventId, OnRefreshCardsForm);
            GameEntry.Event.Subscribe(UnEquipFuneEventArgs.EventId, OnUnEquipFune);
        }

        

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshCardsFormEventArgs.EventId, OnRefreshCardsForm);
            GameEntry.Event.Unsubscribe(UnEquipFuneEventArgs.EventId, OnUnEquipFune);
            GameManager.Instance.CardsForm_EquipFuneIdxs.Clear();
        }
        
        public void OnUnEquipFune(object sender, GameEventArgs e)
        {
            var ne = e as UnEquipFuneEventArgs;
            GameManager.Instance.CardsForm_EquipFuneIdxs.Remove(ne.FuneIdx);
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

        public void Back()
        {
            foreach (var kv in CardManager.Instance.CardDatas)
            {
                for (int i = GameManager.Instance.CardsForm_EquipFuneIdxs.Count - 1; i >= 0; i--)
                {
                    var equipFuneIdx = GameManager.Instance.CardsForm_EquipFuneIdxs[i];
                    if (kv.Value.FuneIdxs.Contains(equipFuneIdx))
                    {
                        kv.Value.FuneIdxs.Remove(equipFuneIdx);
                        BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(equipFuneIdx);
                    }
                }

                
            }
            
            Close();

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