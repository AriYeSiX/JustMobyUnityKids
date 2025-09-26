using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityKids.Gameplay
{
    public class UISquareView : AbstractSquareView
    {
        [SerializeField] protected  Image _animationImage;
        [SerializeField] private float _dragScale = 1.1f;
        
        private bool _isDragging;
        private Vector2 _originalPosition;
        private Vector3 _originalScale;
        private Vector2 _originalAnchorMin;
        private Vector2 _originalAnchorMax;
        private Vector2 _originalPivot;
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private RectTransform _canvasRectTransform;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        private CancellationTokenSource _cancellationTokenSource;
        private async void Awake()
        {
            _rectTransform = _animationImage.rectTransform;
            _canvas = GetComponentInParent<Canvas>();
            _canvasRectTransform = _canvas.transform as RectTransform;
            _originalScale = transform.localScale;
            
            _cancellationTokenSource = new CancellationTokenSource();
            await UniTask.DelayFrame(1, cancellationToken: _cancellationTokenSource.Token);
            _size = GetFigureSize();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cancellationTokenSource.Cancel();
        }
        public override void Initialize(Square square, IFigureService figureService)
        {
            base.Initialize(square, figureService);
            _image.sprite = square.Sprite;
            _animationImage.sprite = square.Sprite;
            
            _originalPosition = _rectTransform.anchoredPosition;
            _originalAnchorMin = _rectTransform.anchorMin;
            _originalAnchorMax = _rectTransform.anchorMax;
            _originalPivot = _rectTransform.pivot;
            _isDragging = false;
            
            _figureService.ActualFigureId
                .Subscribe(UpdateDraggingState)
                .AddTo(_disposables);
                
            _figureService.Position
                .Subscribe(UpdatePosition)
                .AddTo(_disposables);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _figureService.StartDragCommand.Execute(new StartDragJson(Square.Id ,eventData.position));
            _isDragging = true;
            _animationImage.maskable = false;
        }
        
        public override void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            _figureService.DragCommand.Execute(eventData.position);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            var anchoredPosition = _animationImage.rectTransform.anchoredPosition;
            if (anchoredPosition.y > 0 && anchoredPosition.y >= _animationImage.rectTransform.sizeDelta.y/2)
            {
                _figureService.DropCommand.Execute(new TryDropJson(eventData.pointerEnter,
                    eventData.pointerCurrentRaycast.worldPosition, Square.Id, _size));
            }
            _moveTween?.Kill();
            _rectTransform.anchoredPosition = Vector2.zero;
            _animationImage.transform.localScale = _originalScale;
            _animationImage.maskable = false;
        }

        private void UpdateDraggingState(string id)
        {
            if (string.Equals(id, Square.Id))
            {
                _isDragging = true;
                _animationImage.transform.localScale = _originalScale * _dragScale;
            }
            else
            {
                _isDragging = false;
                _animationImage.transform.localScale = _originalScale;
                
                if (!_figureService.IsUsed.Value)
                {
                    _rectTransform.anchorMin = _originalAnchorMin;
                    _rectTransform.anchorMax = _originalAnchorMax;
                    _rectTransform.pivot = _originalPivot;
                    
                    _rectTransform.anchoredPosition = _originalPosition;
                }
            }
        }
        
        private void UpdatePosition(Vector2 position)
        {
            if (!_isDragging)
            {
                return;
            }

            if (_canvasRectTransform == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform, 
                Input.mousePosition, 
                _canvas.worldCamera, 
                out var localPoint
            );
            
            var parentLocalPoint = _canvasRectTransform.TransformPoint(localPoint);
            parentLocalPoint = _rectTransform.parent.InverseTransformPoint(parentLocalPoint);

            if (_moveTween != null)
            {
                _moveTween?.Kill();
            }
            _moveTween = _rectTransform.DOLocalMove(parentLocalPoint, 0.5f);
        }
        
        public override float GetFigureSize()
        {
            var figureTopPoint = new Vector3(0, _rectTransform.rect.height / 2, 0);
            var heightPoint = _rectTransform.TransformPoint(figureTopPoint);
            var figureCenterPoint = new Vector3(0, _rectTransform.anchoredPosition.y, 0);
            var centerPoint = _rectTransform.TransformPoint(figureCenterPoint);
            var size = (Math.Abs(heightPoint.y - centerPoint.y));
            return size;
        }
    }
}