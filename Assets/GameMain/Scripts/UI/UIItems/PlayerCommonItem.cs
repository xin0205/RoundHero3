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

        public void Init()
        {
            commonItem.Init();
        }
        
        public void SetItemData(PlayerCommonItemData playerCommonItemData)
        {
            commonItem.SetItemData(playerCommonItemData.CommonItemData);
        }

        public void Refresh()
        {
            commonItem.Refresh();
        }
    }
}