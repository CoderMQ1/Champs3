// 
// 2024/01/11

using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NotImplementedException = System.NotImplementedException;

namespace SquareHero.Hotfix
{
    public class UIPointerAdapter: MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler,IPointerExitHandler
    {
        public UnityEvent<PointerEventData> PointerDownListener = new UnityEvent<PointerEventData>();
        public UnityEvent<PointerEventData> DragListener = new UnityEvent<PointerEventData>();
        public UnityEvent<PointerEventData> PointerUpListener = new UnityEvent<PointerEventData>();
        
        public void OnPointerDown(PointerEventData eventData)
        {
            this.PointerDownListener?.Invoke(eventData);
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.DragListener?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.PointerUpListener?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            LogKit.I("Point enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            LogKit.I("Point Exit");
        }
    }
}