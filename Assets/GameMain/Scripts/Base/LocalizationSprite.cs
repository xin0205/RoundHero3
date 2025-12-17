
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class LocalizationSprite : MonoBehaviour
    {
        [SerializeField]
        private LocalizationSpriteDictionary LocalizationSpriteDict;
        [SerializeField] private Image image;

        private void OnEnable()
        {
            if(LocalizationSpriteDict.Contains(GameEntry.Localization.Language))
            {
                image.sprite = LocalizationSpriteDict[GameEntry.Localization.Language];
            }
            
        }
    }
}