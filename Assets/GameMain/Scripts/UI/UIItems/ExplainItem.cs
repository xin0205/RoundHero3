using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class ExplainItem : MonoBehaviour
    {
        [SerializeField] private Text desc;
        
        private CommonItemData commonItemData;
        
        public void SetItemData(CommonItemData commonItemData)
        {
            this.commonItemData = commonItemData;
            
            Refresh();
        }


        public async void Refresh()
        {
            var name = "";
            var desc = "";
            GameUtility.GetItemText(commonItemData.ItemType, commonItemData.ItemID, ref name, ref desc);

            this.desc.text = name + ":" + desc;

        }
        
    }
}