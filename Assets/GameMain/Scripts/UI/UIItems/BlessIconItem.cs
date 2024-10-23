using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class BlessIconItem : MonoBehaviour
    {
        [SerializeField] 
        private Image BlessIcon;
        
        [SerializeField] 
        private Text Value;

        private int blessID = -1;
        
        public void Init()
        {
            
        }
        
        public async void SetItemData(Data_Bless blessData, int itemIndex,int row,int column)
        {
            if (blessID != blessData.BlessID)
            {
                blessID = blessData.BlessID;

                BlessIcon.sprite = await AssetUtility.GetBlessIcon(blessID);
                
            }

            Value.gameObject.SetActive(blessData.Value != 0);
            if (blessData.Value != 0)
            {
                Value.text = blessData.Value.ToString();
            }

            
            
        }
    }
}