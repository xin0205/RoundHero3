//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Localization;
using CatJson;
using UGFExtensions.Await;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityEngine;

namespace RoundHero
{
    public class ProcedureLaunch : ProcedureBase
    {

        private bool initUserData;
        
        public override bool UseNativeDialog
        {
            get
            {
                return true;
            }
        }
        
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            
            
            Application.targetFrameRate = 60;
            AwaitableExtensions.SubscribeEvent();
            
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            // 构建信息：发布版本时，把一些数据以 Json 的格式写入 Assets/GameMain/Configs/BuildInfo.txt，供游戏逻辑读取
            GameEntry.BuiltinData.InitBuildInfo();

            // 语言配置：设置当前使用的语言，如果不设置，则默认使用操作系统语言
            InitLanguageSettings();

            // 变体配置：根据使用的语言，通知底层加载对应的资源变体
            InitCurrentVariant();

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();

            // 默认字典：加载默认字典文件 Assets/GameMain/Configs/DefaultDictionary.xml
            // 此字典文件记录了资源更新前使用的各种语言的字符串，会随 App 一起发布，故不可更新
            GameEntry.BuiltinData.InitDefaultDictionary();
            
            JsonParser.Default.IsPolymorphic = true;
            JsonParser.Default.IsFormat = false;
            
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

#if !UNITY_EDITOR
            // if (!initUserData && SteamManager.Initialized)
            // {
            //     var steamID = SteamUser.GetSteamID().m_SteamID.ToString();
            //     DataManager.Instance.InitData(steamID);
            //     initUserData = true;
            //     Log.Debug("steamID:" + steamID);
            // }

             DataManager.Instance.Init("test");
             ChangeState<ProcedureSplash>(procedureOwner);
#else
            
            ChangeState<ProcedureSplash>(procedureOwner);
#endif
            
        }

        private void InitLanguageSettings()
        {
            if (GameEntry.Base.EditorResourceMode && GameEntry.Base.EditorLanguage != Language.Unspecified)
            {
                // 编辑器资源模式直接使用 Inspector 上设置的语言
                return;
            }

            Language language = GameEntry.Localization.Language;
            if (GameEntry.Setting.HasSetting(Constant.Setting.Language))
            {
                try
                {
                    language = (Language)GameEntry.Setting.GetInt(Constant.Setting.Language);
                }
                catch
                {
                }
            }

            if (!Constant.Game.Languages.ContainsKey(language))
            {
                // 若是暂不支持的语言，则使用英语
                language = Language.English;

                
            }
            
            GameEntry.Setting.SetInt(Constant.Setting.Language, (int)language);
            GameEntry.Setting.Save();
            
            GameEntry.Localization.Language = language;
            Log.Info("GenerateData language settings complete, current language is '{0}'.", language.ToString());
        }

        private void InitCurrentVariant()
        {
            if (GameEntry.Base.EditorResourceMode)
            {
                // 编辑器资源模式不使用 AssetBundle，也就没有变体了
                return;
            }

            string currentVariant = null;
            switch (GameEntry.Localization.Language)
            {
                case Language.English:
                    currentVariant = "en-us";
                    break;

                case Language.ChineseSimplified:
                    currentVariant = "zh-cn";
                    break;

                case Language.ChineseTraditional:
                    currentVariant = "zh-tw";
                    break;

                case Language.Korean:
                    currentVariant = "ko-kr";
                    break;

                default:
                    currentVariant = "zh-cn";
                    break;
            }

            GameEntry.Resource.SetCurrentVariant(currentVariant);
            Log.Info("GenerateData current variant complete.");
        }

        private void InitSoundSettings()
        {
            //GameEntry.Setting.RemoveAllSettings();
            GameEntry.Sound.Mute("Music", GameEntry.Setting.GetBool(Constant.Setting.MusicMuted, false));
            GameEntry.Sound.SetVolume("Music", GameEntry.Setting.GetFloat(Constant.Setting.MusicVolume, 1f));
            // GameEntry.Sound.Mute("Sound", GameEntry.Setting.GetBool(Constant.Setting.SoundMuted, false));
            // GameEntry.Sound.SetVolume("Sound", GameEntry.Setting.GetFloat(Constant.Setting.SoundVolume, 1f));
            GameEntry.Sound.Mute("UISound", GameEntry.Setting.GetBool(Constant.Setting.UISoundMuted, false));
            GameEntry.Sound.SetVolume("UISound", GameEntry.Setting.GetFloat(Constant.Setting.UISoundVolume, 1f));
            Log.Info("GenerateData sound settings complete.");
        }
    }
}
