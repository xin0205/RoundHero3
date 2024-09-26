
using RoundHero;
using UnityEngine;

public class HeroIcon : MonoBehaviour
{
    public HeroIconSpriteDictionary HeroIconSprite;
    
    
    public Sprite this[EHeroID heroID]
    {
        get
        {
            if(HeroIconSprite.ContainsKey(heroID))
                return HeroIconSprite[heroID];

            return HeroIconSprite[EHeroID.Normal];
        }
        
    }
    
}
