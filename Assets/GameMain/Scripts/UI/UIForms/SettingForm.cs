using System.Linq;
using GameFramework.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class SettingForm  : UGuiForm
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Toggle musicToggle;
        
        [SerializeField] private Slider uiSoundSlider;
        [SerializeField] private Toggle uiSoundToggle;
        
        [SerializeField] private Toggle fullScreenToggle;
        
        [SerializeField] private Dropdown languageDropdown;
        
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            languageDropdown.options.Clear();
            foreach (Language language in Constant.Game.Languages.Keys)
            {
                var optionData = new Dropdown.OptionData(Constant.Game.Languages[language]);
                languageDropdown.options.Add(optionData); 
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            musicToggle.isOn = !GameEntry.Sound.IsMuted("Music");
            musicSlider.value = GameEntry.Sound.GetVolume("Music");

            uiSoundToggle.isOn = !GameEntry.Sound.IsMuted("UISound");
            uiSoundSlider.value = GameEntry.Sound.GetVolume("UISound");
            fullScreenToggle.isOn = GameEntry.Setting.GetBool("FullScreen");

            var languageList = Constant.Game.Languages.Keys.ToList();
            for (int i = 0; i < languageList.Count; i++)
            {
                if (languageList[i] == (Language) GameEntry.Setting.GetInt(Constant.Setting.Language))
                {
                    languageDropdown.value = i;
                    break;
                }
            }

            //fullScreenToggle.isOn = Screen.fullScreen;
        }
        
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     Close();
            // }
        }

        public void OnLangeuageChanged(int idx)
        {
            var languageList = Constant.Game.Languages.Keys.ToList();
            var language = languageList[idx];

            if (language == (Language)GameEntry.Setting.GetInt(Constant.Setting.Language))
            {
                return;
            }
            
            GameEntry.UI.OpenMessage(Constant.Game.LanguageRestart[language]);
            
            GameEntry.Setting.SetInt(Constant.Setting.Language, (int)language);
            GameEntry.Setting.Save();
        }

        public void OnMusicMuteChanged(bool isOn)
        {
            GameEntry.Sound.Mute("Music", !isOn);
            musicSlider.gameObject.SetActive(isOn);
            GameEntry.Sound.PlayUISound(EUISound.CommonButton);
        }
        
        public void OnMusicVolumeChanged(float volume)
        {
            GameEntry.Sound.SetVolume("Music", volume);
        }
        
        public void OnUISoundMuteChanged(bool isOn)
        {
            GameEntry.Sound.Mute("UISound", !isOn);
            uiSoundSlider.gameObject.SetActive(isOn);
            GameEntry.Sound.PlayUISound(EUISound.CommonButton);
        }

        public void OnUISoundVolumeChanged(float volume)
        {
            GameEntry.Sound.SetVolume("UISound", volume);
        }
        
        public void OnFullScreenChanged(bool isOn)
        {
            Screen.fullScreen = isOn;
            GameEntry.Setting.SetBool("FullScreen", isOn);
            GameEntry.Setting.Save();
            GameEntry.Sound.PlayUISound(EUISound.CommonButton);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            GameEntry.Sound.PlayUISound(EUISound.CommonButton);
            base.OnClose(isShutdown, userData);
            
        }
    }
}