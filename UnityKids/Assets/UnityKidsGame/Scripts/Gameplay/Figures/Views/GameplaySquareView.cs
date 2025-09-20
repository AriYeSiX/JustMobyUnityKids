using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GameplaySquareView : AbstractSquareView
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

        private GameObject _holeObject;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        private CancellationTokenSource _cancellationTokenSource;
        private async void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = _image.rectTransform;
            _canvas = GetComponentInParent<Canvas>();
            _canvasRectTransform = _canvas.transform as RectTransform;
            _originalScale = Vector3.one;
            _image.transform.localScale = _originalScale;
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
            _animationImage.sprite = square.Sprite;
            _holeObject = figureService.GameplaySceneReferences.HoleObject;

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
            _figureService.DestroyFigure
                .Subscribe(DestroyFigure)
                .AddTo(_disposables);            
            _figureService.PlaceFigure
                .Subscribe(PlaceFigure)
                .AddTo(_disposables);

            _figureService.FixTowerCommand.Subscribe(FixSquarePosition).AddTo(_disposables);
        }

        private void FixSquarePosition(List<FigureMovementData> figures)
        {
            if (figures.Any(x=>x.Id == Square.Id))
            {
                _moveTween?.Kill();
                _moveTween = _rectTransform.DOMove(figures.First(x=>x.Id == Square.Id).NewPosition, 1f);
            }
        }

        private void DestroyFigure(AbstractFigure figure)
        {
            if (string.Equals(figure.Id, Square.Id))
            {
                _moveTween?.Kill(true);
                _moveTween = _rectTransform
                    .DOScale(Vector2.zero, 0.5f)
                    .SetEase(Ease.InCirc)
                    .OnComplete(DestroyOnComplete);
            }
        }

        private void DestroyOnComplete()
        {
            DestroyImmediate(gameObject);
        }
        
        private void PlaceFigure(PlaceJson placeJson)
        {
            if (string.Equals(placeJson.Figure.Id, Square.Id))
            {
                _moveTween?.Kill();
                _moveTween = _rectTransform.DOMove(placeJson.PlacePosition, 1f).SetEase(Ease.OutFlash);
                
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _figureService.StartDragCommand.Execute(new StartDragJson(Square.Id ,eventData.position));
            _isDragging = true;
            _image.raycastTarget = false;
            
            _originalAnchorMin = _rectTransform.anchorMin;
            _originalAnchorMax = _rectTransform.anchorMax;
            _originalPivot = _rectTransform.pivot;
            _originalPosition = _rectTransform.anchoredPosition;
        }
        
        public override void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            _figureService.DragCommand.Execute(eventData.position);
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            if (eventData.pointerEnter == _holeObject)
            {
                var image = eventData.pointerEnter.GetComponent<Image>();
                var sprite = image.sprite;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, eventData.position,
                    eventData.pressEventCamera, out var localPoint);

                var rect = image.rectTransform.rect;
                var normalized = new Vector2((localPoint.x - rect.x) / rect.width, (localPoint.y - rect.y) / rect.height);

                var texture = sprite.texture;
                var x = Mathf.FloorToInt(normalized.x * texture.width);
                var y = Mathf.FloorToInt(normalized.y * texture.height);
                
                var color = texture.GetPixel(x, y);
                
                if (color.a >= 1)
                {
                    _figureService.DropCommand.Execute(new TryDropJson(eventData.pointerEnter,
                        eventData.pointerCurrentRaycast.worldPosition, Square.Id,GetFigureSize())); 
                    return;
                }
                Debug.Log("Object not in the Hole");
            }

            ResetSquarePosition();
            _image.raycastTarget = true;
            _figureService.IsDragging.Value = false;
            _figureService.ActualFigureId.Value = string.Empty;
            _figureService.DropCommand.Execute(new TryDropJson(eventData.pointerEnter,
                eventData.pointerCurrentRaycast.worldPosition, Square.Id,_size));
        }

        private void ResetSquarePosition()
        {
            _moveTween?.Kill();
            
            _image.transform.localScale = _originalScale;
            
            _rectTransform.anchorMin = _originalAnchorMin;
            _rectTransform.anchorMax = _originalAnchorMax;
            _rectTransform.pivot = _originalPivot;
                
            _rectTransform.anchoredPosition = _originalPosition;
        }
        
        private void UpdateDraggingState(string id)
        {
            if (string.Equals(id, Square.Id))
            {
                _isDragging = true;
                _image.transform.localScale = _originalScale * _dragScale;
            }
            else
            {
                _isDragging = false;
                _image.transform.localScale = _originalScale;
            }
        }
        
        private void UpdatePosition(Vector2 position)
        {
            if (!_isDragging)
            {
                return;
            }

            var canvasRectTransform = _canvas.GetComponent<RectTransform>();
            if (canvasRectTransform == null)
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
            _moveTween = _rectTransform.DOLocalMove(parentLocalPoint, 1f);
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