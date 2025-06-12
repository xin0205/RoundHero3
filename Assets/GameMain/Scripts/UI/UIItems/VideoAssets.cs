using UnityEngine;

namespace RoundHero
{

    [CreateAssetMenu(fileName = "VideoAssets", menuName = "ScriptableObject/VideoAssets", order = 0)]
    public class VideoAssets : ScriptableObject
    {
        [SerializeField] public VideoAssetDictionary VideoAssetDict;
    }
}