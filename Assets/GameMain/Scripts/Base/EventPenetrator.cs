using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RoundHero
{
    [RequireComponent(typeof(EventTrigger))]
    public class EventPenetrator : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        IPointerClickHandler, IDragHandler,
        IBeginDragHandler, IEndDragHandler,
        IDropHandler, IScrollHandler
    {
        [System.Serializable]
        public enum PassthroughEvent
        {
            PointerEnter,
            PointerExit,
            PointerDown,
            PointerUp,
            PointerClick,
            Drag,
            BeginDrag,
            EndDrag,
            Drop,
            Scroll
        }

        [Tooltip("选择需要穿透到下层的事件类型")] public List<PassthroughEvent> eventsToPassthrough = new List<PassthroughEvent>();

        [Tooltip("是否在触发下层事件前先触发当前对象的事件")] public bool triggerSelfFirst = true;

        private EventTrigger _eventTrigger;
        private bool _isProcessingEvent = false; // 防止递归调用

        private void Awake()
        {
            _eventTrigger = GetComponent<EventTrigger>();
        }

        // 获取所有下层可接收事件的对象
        private List<GameObject> GetUnderlyingObjects()
        {
            List<GameObject> results = new List<GameObject>();

            // 从当前对象位置发射射线，检测所有下层UI元素
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            // 跳过自身，获取其他所有UI元素
            foreach (var result in raycastResults)
            {
                if (result.gameObject != gameObject)
                {
                    results.Add(result.gameObject);
                }
            }

            return results;
        }

        // 传递事件到下层对象
        private void PassEventToUnderlyingObjects<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            foreach (var obj in GetUnderlyingObjects())
            {
                // 确保不会将事件传递回自身
                if (obj != gameObject)
                {
                    ExecuteEvents.Execute(obj, eventData, function);
                }
            }
        }

        // 执行当前对象的事件触发器中的回调
        private void ExecuteSelfCallbacks(EventTriggerType type, BaseEventData eventData)
        {
            if (_eventTrigger == null) return;

            foreach (var entry in _eventTrigger.triggers)
            {
                if (entry.eventID == type)
                {
                    entry.callback.Invoke(eventData);
                }
            }
        }

        #region 事件实现

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.PointerEnter, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.PointerEnter))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.pointerEnterHandler);

            _isProcessingEvent = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.PointerExit, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.PointerExit))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.pointerExitHandler);

            _isProcessingEvent = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.PointerDown, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.PointerDown))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.pointerDownHandler);

            _isProcessingEvent = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.PointerUp, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.PointerUp))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.pointerUpHandler);

            _isProcessingEvent = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.PointerClick, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.PointerClick))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.pointerClickHandler);

            _isProcessingEvent = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.Drag, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.Drag))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.dragHandler);

            _isProcessingEvent = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.BeginDrag, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.BeginDrag))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.beginDragHandler);

            _isProcessingEvent = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.EndDrag, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.EndDrag))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.endDragHandler);

            _isProcessingEvent = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.Drop, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.Drop))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.dropHandler);

            _isProcessingEvent = false;
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (_isProcessingEvent) return;
            _isProcessingEvent = true;

            if (triggerSelfFirst)
                ExecuteSelfCallbacks(EventTriggerType.Scroll, eventData);

            if (eventsToPassthrough.Contains(PassthroughEvent.Scroll))
                PassEventToUnderlyingObjects(eventData, ExecuteEvents.scrollHandler);

            _isProcessingEvent = false;
        }

        #endregion
    }
}
    