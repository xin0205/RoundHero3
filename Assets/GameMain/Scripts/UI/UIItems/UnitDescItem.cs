using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class UnitDescItem : MonoBehaviour
    {
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text descText;
        [SerializeField]
        private Text powerText;
        
        public void SetDesc(string name, string power, string desc)
        {
            nameText.text = name;
            powerText.text = power;
            descText.text = desc;
            
        }
    }
}