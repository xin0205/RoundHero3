using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace RoundHero
{
    public class MessageFormParams
    {
        public string Message { get; set; }
    }
    
    public class MessageForm : UGuiForm
    {
        [SerializeField] private Text message;

        private MessageFormParams messageFormParams;
        private CanvasGroup canvasGroup;
        [SerializeField] private Animation root;

        protected override void OnInit(object userData)

        {
            base.OnInit(userData);

            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            //root.Play();
            
            messageFormParams = userData as MessageFormParams;

            message.text = messageFormParams.Message;

            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0f, 2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Close();
            });
        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            canvasGroup.DOKill(false);
            base.OnClose(isShutdown, userData);
            
        }
    }
}