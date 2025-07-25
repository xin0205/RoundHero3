﻿using System;
using GifImporter;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


namespace RoundHero
{

    [Serializable]
    public class BattleBGSpriteDictionary : SerializableDictionary<int, Sprite>
    {
    }
    
    [Serializable]
    public class MoveDirectGODictionary : SerializableDictionary<ERelativePos, GameObject>
    {
    }

    [Serializable]
    public class MapSiteSpriteDictionary : SerializableDictionary<EMapSite, Sprite>
    {
    }
    
    [Serializable]
    public class HeroIconSpriteDictionary : SerializableDictionary<EHeroID, Sprite>
    {
    }
    
    [Serializable]
    public class FollowerIconSpriteDictionary : SerializableDictionary<EHeroID, Sprite>
    {
    }
    
    [Serializable]
    public class CardTypeToggleDictionary : SerializableDictionary<ECardType, Toggle>
    {
    }
    
    [Serializable]
    public class GIFAssetDictionary : SerializableDictionary<string, Gif>
    {
    }
    
    [Serializable]
    public class VideoAssetDictionary : SerializableDictionary<string, VideoClip>
    {
    }
    
    [Serializable]
    public class TutorialStepGODictionary : SerializableDictionary<ETutorialStep, GameObject>
    {
    }
}