using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

 
namespace UnityEngine.UI
{
    public class ExtClick : MonoBehaviour,IPointerClickHandler
    {
        [Serializable]
        public class ClickedEvent : UnityEvent
        {
            int actionCount = 0;
            public int ActionCount { get { return actionCount + GetPersistentEventCount(); } }
            public new void AddListener(UnityAction call)
            {
                actionCount++;
                base.AddListener(call);
            }
            public new void RemoveListener(UnityAction call)
            {
                actionCount--;
                base.RemoveListener(call);
            }
 
        }
        /// <summary>
        /// 判断单击双击的时间间隔
        /// </summary>
        public float interval = 0.5f;
        public Action a;
        [SerializeField]
        private ClickedEvent onSingleClick = new ClickedEvent();
        public ClickedEvent OnSingleClick { get { return onSingleClick; } }
        [SerializeField]
        private ClickedEvent onDoubleClick = new ClickedEvent();
        public ClickedEvent OnDoubleClick { get { return onDoubleClick; } }
        [SerializeField]
        private ClickedEvent onTripleClick = new ClickedEvent();
        public ClickedEvent OnTripleClick { get { return onTripleClick; } }
 
        bool isClicked = false;
        int clickCount = 0;
 
        public void OnPointerClick(PointerEventData eventData)
        {
            clickCount++;
            if (!isClicked)
            {
                isClicked = true;
                StartCoroutine(OnClickFinish());
            }

        }
 
 
        private IEnumerator OnClickFinish()
        {
            bool isSingleClickEnable = OnSingleClick != null && OnSingleClick.ActionCount > 0;
            bool isDoubleClickEnable = OnDoubleClick != null && OnDoubleClick.ActionCount > 0;
            bool isTripleClickEnable = OnTripleClick != null && OnTripleClick.ActionCount > 0;
            if (isDoubleClickEnable)
            {
                yield return new WaitForSeconds(interval);
            }
            if (isTripleClickEnable)
            {
                yield return new WaitForSeconds(interval);
            }
            if (clickCount <= 1)
            {
                if (isSingleClickEnable)
                {
                    OnSingleClick.Invoke();
                }
            }
            else if (clickCount == 2)
            {
                if (isDoubleClickEnable)
                {
                    OnDoubleClick.Invoke();
                }
                else if (isSingleClickEnable)
                {
                    OnSingleClick.Invoke();
                }
            }
            else
            {
                if (isTripleClickEnable)
                {
                    OnTripleClick.Invoke();
                }
                else if (isDoubleClickEnable)
                {
                    OnDoubleClick.Invoke();
                }
                else if (isSingleClickEnable)
                {
                    OnSingleClick.Invoke();
                }
            }
            clickCount = 0;
            isClicked = false;
        }
    }
}