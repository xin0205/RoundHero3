﻿
using System.Collections.Generic;
using GameFramework;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public static class LocalizationUIExtension
    {
        private const string NonBreakingSpace = "\u00A0";
        public static string GetString(this LocalizationComponent localizationComponent, string key)
        {
            var drLocalization = GameEntry.DataTable.GetLocalization(key);
            if (drLocalization != null)
            {
                string localization = drLocalization.Value;
                if (localization.Contains(" "))
                {
                    localization = localization.Replace(" ", NonBreakingSpace);
                }
                
                return  localization.Replace ("\\n", "\n");  
                
            }

            //return "No Key";
            return key;
        }

        public static string GetLocalizedString<T1, T2>(this LocalizationComponent localizationComponent, string localizationStr, T1 val1, T2 val2)
        {
            var str = GetString(localizationComponent, localizationStr);
            return Utility.Text.Format(str, val1, val2);
        }
        
        public static string GetLocalizedString<T1>(this LocalizationComponent localizationComponent, string localizationStr, T1 val1)
        {
            var str = GetString(localizationComponent, localizationStr);
            return Utility.Text.Format(str, val1);
        }
        
        public static string GetLocalizedStrings<T>(this LocalizationComponent localizationComponent, string localizationStr, List<T> vals)
        {
            if (vals.Count == 0)
            {
                return GetString(localizationComponent, localizationStr);;
            }
            else if (vals.Count == 1)
            {
                return GetLocalizedString(localizationComponent, localizationStr, vals[0]);

            }
            else if(vals.Count == 2)
            {
                return GetLocalizedString(localizationComponent, localizationStr, vals[0], vals[1]);

            }
            else
            {
                return localizationStr;
            }

        }
    }
}