using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class UnitStateIconItem : MonoBehaviour
    {
        [SerializeField]
        private CommonIconItem commonIconItem;

        [SerializeField]
        private Text countText;

        
        public void SetIcon(EUnitState unitState, int count)
        {
            commonIconItem.SetIcon(EItemType.UnitState, (int)unitState);
            countText.text = count.ToString();
        }
    }
}