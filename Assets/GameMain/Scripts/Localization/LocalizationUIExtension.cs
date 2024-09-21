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
        

    }
}