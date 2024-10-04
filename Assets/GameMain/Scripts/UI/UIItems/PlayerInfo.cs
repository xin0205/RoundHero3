using UnityEngine;

namespace RoundHero
{
    public class PlayerInfo : MonoBehaviour
    {
        public void ShowCards()
        {
            GameEntry.UI.OpenUIForm(UIFormId.CardsForm);
        }
    }
}