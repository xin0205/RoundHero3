
using UnityEngine;

namespace RoundHero
{
    public class CommonButton : MonoBehaviour
    {
        public void PlaySound()
        {
            GameEntry.Sound.PlayUISound(EUISound.CommonButton);
        }
    }
}