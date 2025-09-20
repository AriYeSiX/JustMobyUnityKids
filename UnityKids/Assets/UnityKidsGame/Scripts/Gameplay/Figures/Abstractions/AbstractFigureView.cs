using System;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityKids.Gameplay
{
    public abstract class AbstractFigureView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDisposable
    {
        [SerializeField] protected Image _image;
        public Image Image => _image;
     
        protected readonly CompositeDisposable _disposables = new();
        
        public abstract void OnBeginDrag(PointerEventData eventData);
        public abstract void OnEndDrag(PointerEventData eventData);
        public abstract void OnDrag(PointerEventData eventData);
        public abstract float GetFigureSize();
        
        public void Dispose()
        {
            _disposables.Dispose();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }
}