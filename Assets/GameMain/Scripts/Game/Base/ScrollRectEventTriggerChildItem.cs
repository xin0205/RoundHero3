using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoundHero
{
    public class ScrollRectEventTriggerChildItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private ScrollRect scrollRect;
        
        public void Start()
        {
            var scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                Transform rect = scrollRect.content;
                if (rect != null)
                {
                    if (transform.IsChildOf(rect.parent))
                    {
                        this.scrollRect = scrollRect;
                    }
                }
            }
           
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (scrollRect == null)
                return;
            scrollRect.OnBeginDrag(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (scrollRect == null)
                return;
            scrollRect.OnDrag(eventData as PointerEventData);
        }
        
        public void  OnEndDrag(PointerEventData eventData)
        {
            if (scrollRect == null)
                return;
            scrollRect.OnEndDrag(eventData);
        }
    }
}