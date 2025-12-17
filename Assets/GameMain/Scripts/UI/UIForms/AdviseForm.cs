using UnityEngine;

namespace RoundHero
{
    public class AdviseForm : UGuiForm
    {

        public void OnpenUrl()
        {
            Application.OpenURL(GameEntry.Localization.GetString(Constant.Localization.Url_Advise));
        }

    }
}