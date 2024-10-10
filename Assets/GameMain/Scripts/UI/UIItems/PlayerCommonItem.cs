using System;
using UnityEngine;

namespace RoundHero
{
    public class PlayerCommonItemData
    {
        public CommonItemData CommonItemData { get; set; }
        
        public int ItemIdx { get; set; }
        
        

    }
    
    public class PlayerCommonItem : MonoBehaviour
    {
        [SerializeField] public CommonItem commonItem;

        public Action<int> OnPointDownAction;
        
        public Action OnPointUpAction;

        public PlayerCommonItemData PlayerCommonItemData;

        public void Init()
        {
            commonItem.Init();
        }

        

        public void SetItemData(PlayerCommonItemData playerCommonItemData, Action<int> onPointDownAction, Action onPointUpAction)
        {
            this.PlayerCommonItemData = playerCommonItemData;
            
            commonItem.SetItemData(playerCommonItemData.CommonItemData);
            OnPointDownAction = onPointDownAction;
            OnPointUpAction = onPointUpAction;

        }

        public void Refresh()
        {
            commonItem.Refresh();
        }

        private PlayerCommonItem tempPlayerCommonItem;
        public void OnPointDown()
        {
            OnPointDownAction?.Invoke(PlayerCommonItemData.ItemIdx);
            

        }
        
        public void OnPointUp()
        {
            OnPointUpAction?.Invoke();

            
        }
    }
}