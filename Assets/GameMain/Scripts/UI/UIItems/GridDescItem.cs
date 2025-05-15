using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class GridDescItem : MonoBehaviour
    {
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text descText;

        
        public void SetDesc(string name, string desc)
        {
            nameText.text = name;
            descText.text = desc;
            
        }
    }
}