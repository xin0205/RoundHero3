using System.Collections.Generic;
using System.Linq;
using GameFramework.Localization;
using UnityEngine;

namespace RoundHero
{
    public static partial class Constant
    {
        public static class Game
        {
            public const string GameDataKey = "GameData";
            public const string VersionKey = "Version";
            public const int RandomRange = 99999999;

            
            
            public static Dictionary<Language, string> Languages = new Dictionary<Language, string>()
            {
                [Language.ChineseSimplified] = "简体中文",
                [Language.ChineseTraditional] = "繁體中文",
                [Language.English] = "English",
                //[Language.Russian] = "Русский",
                // [Language.Korean] = "Korea",
                // [Language.Japanese] = "やまと",

            };
                
            public static Dictionary<Language, string> LanguageRestart = new Dictionary<Language, string>()
            {
                [Language.ChineseSimplified] = "重启后生效",
                [Language.ChineseTraditional] = "重啟後生效",
                [Language.English] = "Effective after reboot",
                //[Language.Russian] = "Действует после перезагрузки",
                
                // [Language.Korean] = "Korea",
                // [Language.Japanese] = "再起動後に有効",

            };

            
        }

    }
}