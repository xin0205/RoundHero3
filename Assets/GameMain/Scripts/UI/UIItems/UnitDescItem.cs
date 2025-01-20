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
        
        public void SetDesc(string name, int power, string desc)
        {
            nameText.text = name;
            descText.text = desc;
            powerText.text = power.ToString();
        }
    }
}