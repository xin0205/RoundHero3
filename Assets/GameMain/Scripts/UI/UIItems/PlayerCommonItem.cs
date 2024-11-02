using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoundHero
{
    public class PlayerCommonItemData
    {
        public CommonItemData CommonItemData { get; set; }
        
        public int ItemIdx { get; set; }
        
        

    }
    
    public class PlayerCommonItem : MonoBehaviour
    {
        [FormerlySerializedAs("commonItem")] [SerializeField] public CommonDescItem commonDescItem;

        public Action<int> OnPointDownAction;
        
        public Action OnPointUpAction;

        public PlayerCommonItemData PlayerCommonItemData;

        public void Init()
        {
            commonDescItem.Init();
        }

        

        public void SetItemData(PlayerCommonItemData playerCommonItemData, Action<int> onPointDownAction, Action onPointUpAction)
        {
            this.PlayerCommonItemData = playerCommonItemData;
            
            commonDescItem.SetItemData(playerCommonItemData.CommonItemData);
            OnPointDownAction = onPointDownAction;
            OnPointUpAction = onPointUpAction;

        }

        public void Refresh()
        {
            commonDescItem.Refresh();
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