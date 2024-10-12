
using System.Collections.Generic;
using System.Linq;
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
        
        
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            

            var funeIdxs = BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs;
            FunesViews.Init(funeIdxs, this.gameObject);

            // switchViewToggle.isOn = true;
            // switchViewToggle.isOn = false;
        }

        

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
        }

        public void RefreshView()
        {
            var funeIdxs = BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs;
            FunesViews.Init(funeIdxs, this.gameObject);
            
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