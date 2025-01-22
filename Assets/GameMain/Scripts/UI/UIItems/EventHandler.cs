using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoundHero
{
    public class EventHandler : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        public UnityEvent<PointerEventData> onPointerEnter;
        [SerializeField]
        public UnityEvent<PointerEventData> onPointerExit;
        [SerializeField]
        public UnityEvent<PointerEventData> onPointerClick;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick.Invoke(eventData);
        }
    }
}