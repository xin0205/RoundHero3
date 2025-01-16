using UnityEngine;

namespace RoundHero
{
    [CreateAssetMenu(fileName = "GIFAssets", menuName = "ScriptableObject/GIFAssets", order = 0)]
    public class GIFAssets : ScriptableObject
    {
        [SerializeField] public GIFAssetDictionary GifAssetDict;
    }
}