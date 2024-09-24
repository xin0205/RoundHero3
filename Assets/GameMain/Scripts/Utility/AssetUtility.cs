//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;

namespace RoundHero
{
    public static class AssetUtility
    {
        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Configs/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDataTableAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }
        
        public static string GetLocalizationDataTableAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/DataTables/Localization_{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.{2}", GameEntry.Localization.Language, assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Fonts/{0}", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
        }
        
        public static string GetTreasureBlockItemAsset()
        {
            return "Assets/GameMain/UI/UIItems/Treasure/TreasureBlockItem.prefab";
        }
        
        

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISounds/{0}", assetName);
        }
        
        public static string GetAttributeAsset()
        {
            return "Assets/GameMain/Entities/Items/ItemAttribute.prefab";
        }
        
        public static string GetRoundBuffAsset()
        {
            return "Assets/GameMain/Entities/Items/ItemRoundBuff.prefab";
        }
        
        public static string GetBlockPropAsset()
        {
            return "Assets/GameMain/Entities/Items/ItemBlockProp.prefab";
        }
        

        
        public static string GetBattleCardPrefab()
        {
            return "Assets/GameMain/Entities/Battles/BattleCardEntity.prefab";
        }
        
        public static string GetBattleEnemyPrefab(int enemyTypeID)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/Enemies/BattleEnemyEntity_0.prefab", enemyTypeID);
        }
        
        public static string GetGridPropPrefab(int gridPropID)
        {
            return "Assets/GameMain/Entities/Battles/GridPropEntity_Action_HurtRoundStart_Range.prefab";
            //return Utility.Text.Format("Assets/GameMain/Entities/Battles/GridPropEntity_Action_HurtRoundStart_Range.prefab", Enum.GetName(typeof(EGridPropID), gridPropID));
        }
        
        public static string GetBattleHeroPrefab(EHeroID heroID)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/Heros/BattleHeroEntity_Normal.prefab", Enum.GetName(typeof(EHeroID), heroID));
        }
        
        public static string GetBattleTipsPrefab()
        {
            return "Assets/GameMain/Entities/Battle/BattleTipsEntity.prefab";
        }
        

        

        
        public static string GeItemPrefab(string prefabName)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/Items/{0}.prefab", prefabName);
        }
        
        

        
        public static string GetAttributeIcon(EHeroAttribute heroAttribute)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icons/Attributes/{0}.png", Enum.GetName(typeof(EHeroAttribute), heroAttribute));
        }
        
        public static string GetBlessIcon(EHeroAttribute heroAttribute)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icons/Blesses/{0}.png", Enum.GetName(typeof(EHeroAttribute), heroAttribute));
        }
        
        
        
        

        
        public static string GetGridPrefab()
        {
            return "Assets/GameMain/Entities/Areas/GridEntity.prefab";
        }
        
        public static string GetScenePrefab(string sceneName)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/Scenes/{0}.prefab", sceneName);
        }
        
        public static string GetBattleRoutePrefab()
        {
            return "Assets/GameMain/Entities/Areas/RouteEntity.prefab";
        }
        
        public static string GetBattleSoliderPrefab(int cardID)
        {
            return "Assets/GameMain/Entities/Soliders/BattleSoliderEntity_OnTheWay_Attack.prefab";
            
        }

    }
}
