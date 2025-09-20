using System;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace UnityKids.Gameplay
{
    /// <summary>
    /// View составляющая FigureListener с обыгрыванием пункта требований 2
    /// </summary>
    public class FigureListenerView : MonoBehaviour, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        
        [SerializeField] private TMP_Text _tmpText;
        
        private FigureListener _figureListener;
        private void Awake()
        {
            _tmpText.text = string.Empty;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        [Inject]
        private void Construct(FigureListener figureListener)
        {
            _figureListener = figureListener;

            figureListener.NewRumor
                .Subscribe(Localize)
                .AddTo(_disposables);
        }

        /// <summary>
        /// "Предусмотрите возможность локализации (можно без самой системы);"
        /// </summary>
        /// <param name="key"></param>
        private void Localize(string key)
        {
            _tmpText.text = key;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}