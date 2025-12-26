using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class BattleValue : MonoBehaviour
    {
        [SerializeField] private Text text;
        
        public void SetData(Vector2 pos, int value)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            transform.localScale = Vector3.one; 
            
            text.text = value.ToString();

            //text.color = hurtColor;
            transform.localPosition = pos;
        }
    }
}