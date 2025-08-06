
using UGFExtensions.Await;
using UnityEngine;


namespace RoundHero
{
    public class VideoTriggerItem : MonoBehaviour
    {
        [SerializeField]
        public VideoFormData VideoFormData;

        private VideoForm videoForm;
        
        private bool isOpen = false;
        
        public async void OnPointerEnter()
        {
            if(isOpen)
                return;

            isOpen = true;
            var formAsync = await GameEntry.UI.OpenUIFormAsync(UIFormId.VideoForm, VideoFormData);
            videoForm = formAsync?.Logic as VideoForm;
        }

        public void OnPointerExit()
        {

            if(!isOpen)
                return;

            isOpen = false;
            if (videoForm == null)
                return;
                
            
            GameEntry.UI.CloseUIForm(videoForm);
            videoForm = null;
        }

        private void Update()
        {
            if (!isOpen && videoForm != null)
            {
                GameEntry.UI.CloseUIForm(videoForm);
                videoForm = null;
            }
        }
    }
}