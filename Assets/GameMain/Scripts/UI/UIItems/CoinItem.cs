using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class CoinItem : MonoBehaviour
    {
        [SerializeField] private Text Coin;

        public void SetPrice(int price)
        {
            Coin.text = price.ToString();
        }
        
    }
}