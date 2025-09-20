using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityKids.Gameplay;
using UnityKids.Save;
using VContainer.Unity;

namespace UnityKids.Core
{
    /// <summary>
    /// Entry point программы, инициализация всех необходимых для игры элементов
    /// </summary>
    public class GameplayFlow : IStartable
    {
        private readonly IFigureService _figureService;
        private readonly IGameZoneService _gameZoneService;
        private readonly GameplaySceneReferences _gameplayReferences;
        private readonly FigureListener _figureListener;
        private readonly ISaveSystem _saveSystem;

        public GameplayFlow(IFigureService figureService, 
            IGameZoneService gameZoneService, 
            GameplaySceneReferences gameplayReferences, 
            FigureListener figureListener,
            ISaveSystem saveSystem)
        {
            _figureService = figureService;
            _gameZoneService = gameZoneService;
            _gameplayReferences = gameplayReferences;
            _figureListener = figureListener;
            _saveSystem = saveSystem;
        }
        
        public async void Start()
        {
            Debug.Log("Gameplay Flow Started");
            await UniTask.WaitUntilValueChanged(_gameplayReferences.GameplayZoneRect,x=>x.sizeDelta);
            _gameZoneService.Init(_gameplayReferences.GameplayZoneRect);
            _saveSystem.Load();
        }
    }
}
