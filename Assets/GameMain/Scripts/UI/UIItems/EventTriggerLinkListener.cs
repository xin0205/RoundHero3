using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace RoundHero
{
    
    // 
    public class EventTriggerLinkListener : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ScrollRect Scroll;

        private bool isDrag = false;
        
        [SerializeField]
        public UnityEvent onPointerClickAction;

        private void Start()
        {
        
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isDrag)
            {
                return;
            }
            onPointerClickAction?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Scroll.OnDrag(eventData);
            isDrag = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Scroll.OnBeginDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Scroll.OnEndDrag(eventData);
            isDrag = false;
        }
    }
}