using UnityEngine;

namespace RoundHero
{
    public class PlayerInfoForm : UGuiForm
    {
        public void ShowCards()
        {
            GameEntry.UI.OpenUIForm(UIFormId.CardsForm);
        }
    }
}