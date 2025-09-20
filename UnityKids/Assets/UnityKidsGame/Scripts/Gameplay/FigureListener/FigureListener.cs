using System;
using R3;
using UnityEngine;
using VContainer;

namespace UnityKids.Gameplay
{
    /// <summary>
    /// Слушатель IFigureService для реализации сообщений о действиях игрока
    /// </summary>
    public class FigureListener : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        public readonly ReactiveCommand<string> NewRumor = new();
        private IFigureService _figureService;
        
        [Inject]
        public void Construct(IFigureService figureService)
        {
            _figureService = figureService;
            _figureService.LastAction
                .Subscribe(lastAction=> 
                {
                    switch (lastAction)
                    {
                        case FigureActions.PlaceFigure:
                            OnFigurePlace();
                            break;
                        case FigureActions.DropFigure:
                            OnFigureDrop();
                            break;
                        case FigureActions.HideFigure:
                            OnFigureHide();
                            break;
                        case FigureActions.CapFigure:
                            OnFigureCap();
                            break;
                        default:
                            Debug.Log("Unexpected figure action");
                            break;
                    }
                }).AddTo(_disposables);
        }

        private void OnFigurePlace()
        {
            NewRumor.Execute("установка кубика");
        }

        private void OnFigureDrop()
        {
            NewRumor.Execute("выкидывание кубика");
        }

        private void OnFigureHide()
        {
            NewRumor.Execute("пропадание кубика");
        }

        private void OnFigureCap()
        {
            NewRumor.Execute("ограничение по высоте");
        }

        public void Dispose()
        {
            _disposables.Dispose();
            NewRumor.Dispose();
        }
    }
}