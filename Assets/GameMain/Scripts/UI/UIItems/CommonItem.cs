using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public enum EItemType
    {
        Card,
        Bless,
        Fune,
    }
    
    public class CommonItemData
    {
        public EItemType ItemType { get; set; }
        
        public int FuneID { get; set; }
        public int CardID { get; set; }
        public EBlessID BlessID { get; set; }
    }
    
    public class CommonItem : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Text desc;
        [SerializeField] private Image icon;
        
        
        private CommonItemData _commonItemData;
        
        public void Init()
        {
            
        }
        
        public void SetItemData(CommonItemData commonItemData)
        {
            this._commonItemData = commonItemData;
            
            Refresh();
        }


        public async void Refresh()
        {
            var name = "";
            var desc = "";
            switch (_commonItemData.ItemType)
            {
 
                case EItemType.Bless:
                    GameUtility.GetItemText(_commonItemData.BlessID, ref name, ref desc);
                    icon.sprite = await AssetUtility.GetBlessIcon(_commonItemData.BlessID);
                    break;
                case EItemType.Fune:
                    GameUtility.GetItemText(_commonItemData.FuneID, ref name, ref desc);
                    icon.sprite = await AssetUtility.GetFuneIcon(_commonItemData.FuneID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            title.text = name; 
            this.desc.text = desc;

        }

    }
}