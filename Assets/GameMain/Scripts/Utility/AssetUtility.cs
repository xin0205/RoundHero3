//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using UGFExtensions.Await;
using UnityEngine;

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
            return "Assets/GameMain/Entities/Battles/GridPropEntity_Obstacle.prefab";
            //return Utility.Text.Format("Assets/GameMain/Entities/Battles/GridPropEntity_Action_HurtRoundStart_Range.prefab", Enum.GetName(typeof(EGridPropID), gridPropID));
        }
        

        
        public static string GetBattleHeroPrefab(int heroID)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/Heros/BattleHeroEntity_{0}.prefab", heroID);
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
        
        public static string GetDisplayHeroPrefab(int heroID)
        {
            var assetPath = "Assets/GameMain/Entities/DisplayHeros/DisplayHero_{0}.prefab";
            var assetName = Utility.Text.Format(assetPath, heroID);
            
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return assetName;
            }
            else
            {
                return Utility.Text.Format(assetPath, 0);
            }

        }
        
        public static string GetBattleRoutePrefab()
        {
            return "Assets/GameMain/Entities/Areas/RouteEntity.prefab";
        }
        
        public static string GetBattleAttackTagPrefab()
        {
            return "Assets/GameMain/Entities/Areas/BattleAttackTagEntity.prefab";
        }
        
        public static string GetBattleParabolaBulletPrefab()
        {
            return "Assets/GameMain/Entities/Bullets/ParabolaBullet.prefab";
        }
        
        public static string GetBattleLineBulletPrefab()
        {
            return "Assets/GameMain/Entities/Bullets/LineBullet.prefab";
        }
        
        public static string GetBattleSoliderPrefab(int cardID)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/Soliders/BattleSoliderEntity_0.prefab", cardID);
            
        }
        
        public static string GetBattleWeaponPrefab(EWeaponHoldingType weaponHoldingType, EWeaponType weaponType, int weaponID)
        {
            return $"Assets/GameMain/Entities/Weapons/{weaponHoldingType}/{weaponType}/Weapon_{weaponID}.prefab";

        }
        
        public static string GetHeroIconName(int heroID)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icons/Heros/{0}.png", heroID);
            
        }
        
        
        
        public static Task<Sprite> GetHeroIcon(int heroID)
        {
            var assetName = GetHeroIconName(heroID);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetHeroIconName(0));
            }

        }
        
        public static string GetBlessIconName(int blessID)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icons/Blesses/{0}.png", blessID);
            
        }
        
        
        
        public static Task<Sprite> GetBlessIcon(int blessID)
        {
            var assetName = GetBlessIconName(blessID);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetBlessIconName(0));
            }

        }
        
        public static string GetTacticIconName(int tacticID)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icons/Tactics/{0}.png", tacticID);
            
        }
        
        public static Task<Sprite> GetTacticIcon(int tacticID)
        {
            var assetName = GetTacticIconName(tacticID);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetTacticIconName(10000));
            }

        }
        
        public static string GetFollowerIconName(int fllowerID)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icons/Followers/{0}.png", fllowerID);
            
        }
        
        public static Task<Sprite> GetFollowerIcon(int unitID)
        {
            var assetName = GetFollowerIconName(unitID);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetFollowerIconName(0));
            }

        }
        
        public static string GetMapSiteIconName(EMapSite mapSite)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Map/{0}.png", mapSite.ToString());
            
        }
        
        public static Task<Sprite> GetMapSiteIcon(EMapSite mapSite)
        {
            var assetName = GetMapSiteIconName(mapSite);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetMapSiteIconName(EMapSite.Empty));
            }

        }
        
        public static string GetFuneIconName(int funeID)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icon/Fune/{0}.png", funeID.ToString());
            
        }
        
        public static string GetCommonIconName(EItemType itemType)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icon/Common/{0}.png", itemType.ToString());
            
        }
        
        public static Task<Sprite> GetCommonIcon(EItemType itemType)
        {
            var assetName = GetCommonIconName(itemType);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetEmptyIconName());
            }

        }
        
        public static string GetEmptyIconName()
        {
            return "Assets/GameMain/UI/UISprites/Icons/Empty.png";
            
        }
        
        public static Task<Sprite> GetFuneIcon(int funeID)
        {
            var assetName = GetFuneIconName(funeID);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetEmptyIconName());
            }

        }
        
        public static string GetBlessIconName(EBlessID blessID)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISprites/Icon/Bless/{0}.png", blessID.ToString());
            
        }
        
        public static Task<Sprite> GetBlessIcon(EBlessID blessID)
        {
            var assetName = GetBlessIconName(blessID);
            if (GameEntry.Resource.HasAsset(assetName) != HasAssetResult.NotExist)
            {
                return GameEntry.Resource.LoadSpriteAsync(assetName);
            }
            else
            {
                return GameEntry.Resource.LoadSpriteAsync(GetEmptyIconName());
            }

        }
        
        public static string GetBattleHurtPrefab()
        {
            return $"Assets/GameMain/Entities/Battles/BattleHurtEntity.prefab";

        }
        
        public static string GetBattleMoveValuePrefab()
        {
            return $"Assets/GameMain/Entities/Battles/BattleMoveValueEntity.prefab";

        }
        
        public static string GetBattleDisplayValuePrefab()
        {
            return "Assets/GameMain/Entities/Battles/BattleDisplayValueEntity.prefab";
        }
        
        public static string GetEffectPrefab(string assetName)
        {
            return $"Assets/GameMain/Entities/Effects/{assetName}.prefab";

        }

        public static string GetBattleFlyDirectEntityPrefab()
        {
            return "Assets/GameMain/Entities/Battles/BattleFlyDirectEntity.prefab";

        }
        
        public static string GetBattleIconEntityPrefab()
        {
            return "Assets/GameMain/Entities/Battles/BattleIconEntity.prefab";

        }
        
        
        
    }
}
