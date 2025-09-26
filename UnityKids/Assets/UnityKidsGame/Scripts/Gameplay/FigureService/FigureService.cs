using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityKids.Core;
using UnityKids.Creators;
using UnityKids.Data;
using VContainer;

namespace UnityKids.Gameplay
{
    /// <summary>
    /// Центральный сервис по перемещению фигур
    /// получает инпуты от пользователя
    /// отдает команды на генерацию фигур при согласии с IGameZoneService
    /// </summary>
    public class FigureService : IFigureService, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        
        private GameConfiguration _gameConfiguration;
        
        private GameplaySquareCreator _gameplaySquareCreator;
        private UISquareCreator _uiSquareCreator;
        public GameplaySceneReferences GameplaySceneReferences { get; private set; }
        public List<AbstractFigure> Gameplayfigures { get; } = new();
        public List<AbstractFigure> UIFigures { get; } = new();
        
        public IGameZoneService GameZoneService { get; private set; }
        public ReactiveProperty<bool> IsDragging { get; } = new();
        public ReactiveProperty<string> ActualFigureId { get; } = new();
        public ReactiveProperty<Vector2> Position { get; } = new();
        public ReactiveProperty<bool> IsUsed { get; } = new();
        
        public ReactiveCommand<FigureActions> LastAction { get; } = new();
        public ReactiveCommand<AbstractFigure> DestroyFigure { get; } = new();
        public ReactiveCommand<PlaceJson> PlaceFigure { get; } = new();
        public ReactiveCommand<StartDragJson> StartDragCommand { get; } = new();
        public ReactiveCommand<Vector2> DragCommand { get; } = new();
        public ReactiveCommand<TryDropJson> DropCommand { get; } = new();
        public ReactiveCommand<List<FigureMovementData>> FixTowerCommand { get; } = new();

        [Inject]
        public void Construct(GameConfiguration gameConfiguration, GameplaySceneReferences gameplaySceneReferences, IGameZoneService gameZoneService)
        {
            GameZoneService = gameZoneService;
            _gameConfiguration = gameConfiguration;
            GameplaySceneReferences = gameplaySceneReferences;
            _uiSquareCreator = new UISquareCreator(_gameConfiguration.UISquarePrefab);
            _gameplaySquareCreator = new GameplaySquareCreator(_gameConfiguration.GameplaySquarePrefab);
            
            CreateUISquares();
        }
        
        public FigureService()
        {
            StartDragCommand.Subscribe(StartDrag)
                .AddTo(_disposables);
            
            DragCommand
                .Where(_ => IsDragging.Value)
                .Subscribe(UpdateDragPosition)
                .AddTo(_disposables);
            
            DropCommand
                .Where(_ => IsDragging.Value)
                .Subscribe(TryDrop)
                .AddTo(_disposables);
            
            DestroyFigure
                .Subscribe(DestroyGameplaySquare)
                .AddTo(_disposables);
        }

        public void CreateUISquares()
        {
            foreach (var squareData in _gameConfiguration.Squares)
            {
                var square = new UISquare(Guid.NewGuid().ToString(), ColorUtility.ToHtmlStringRGBA(squareData.Color), squareData.Sprite);
                UIFigures.Add(square);
                var squareView = _uiSquareCreator.Create(Vector3.zero,GameplaySceneReferences.UIFigureParent.transform);
                squareView.Initialize(square, this);
            }
        }

        private GameplaySquare CreateGameplaySquare(string id, Vector3 position)
        {
            if (UIFigures.First(x => x.Id == id) is not UISquare uiSquare) return null;
            var square = new GameplaySquare(Guid.NewGuid().ToString(), uiSquare.Color, uiSquare.Sprite);
            Gameplayfigures.Add(square);
            var squareView = _gameplaySquareCreator.Create(new Vector3(position.x,position.y, 0f), GameplaySceneReferences.GameplayZoneObject.transform);
            squareView.Initialize(square, this);
            
            return square;

        }
        private GameplaySquare CreateGameplaySquare(string id, Vector3 position, string customId)
        {
            if (UIFigures.First(x => x.Id == id) is not UISquare uiSquare) return null;
            var square = new GameplaySquare(customId, uiSquare.Color, uiSquare.Sprite);
            Gameplayfigures.Add(square);
            var squareView = _gameplaySquareCreator.Create(new Vector3(position.x,position.y, 0f), GameplaySceneReferences.GameplayZoneObject.transform);
            squareView.Initialize(square, this);
            
            return square;

        }

        private void StartDrag(StartDragJson startDragJson)
        {
            IsDragging.Value = true;
            ActualFigureId.Value = startDragJson.Id;
            Position.Value = startDragJson.Position;
        }
        
        private void UpdateDragPosition(Vector2 position)
        {
            Position.Value = position;
        }

        private void TryDrop(TryDropJson tryDropJson)
        {
            if (tryDropJson.Target == GameplaySceneReferences.HoleObject && UIFigures.All(x=>x.Id!=tryDropJson.Id))
            {
                var remove = GameZoneService.TryRemoveFigure(tryDropJson.Id, out var moveList);

                if (remove)
                {
                    FixTowerCommand.Execute(moveList);
                }
                
                LastAction.Execute(FigureActions.DropFigure);
                DestroyFigure.Execute(Gameplayfigures.First(x => x.Id == tryDropJson.Id));
            }
            else if (tryDropJson.Target == GameplaySceneReferences.GameplayZoneObject)
            {
                TryDropOnZone(tryDropJson);
            }
            else if(tryDropJson.Target == null)
            {
                LastAction.Execute(FigureActions.CapFigure);
                DestroyFigure.Execute(CreateGameplaySquare(tryDropJson.Id, tryDropJson.Position));
            }
            else
            {
                LastAction.Execute(FigureActions.HideFigure);
                DestroyFigure.Execute(CreateGameplaySquare(tryDropJson.Id, tryDropJson.Position));
            }
            
            IsDragging.Value = false;
            ActualFigureId.Value = string.Empty;
        }
        
        private void TryDropOnZone(TryDropJson tryDropJson)
        {
            var id = Guid.NewGuid().ToString();
            var placed = GameZoneService.TryPlaceFigure(id, tryDropJson.Position, tryDropJson.Size, out var placePos);
            LastAction.Execute(placed);
            
            switch (placed)
            {
                case FigureActions.PlaceFigure:
                    var figure = CreateGameplaySquare(tryDropJson.Id, tryDropJson.Position, id);
                    if (figure != null)
                        PlaceFigure.Execute(new PlaceJson(figure, placePos));
                    break;
                default:
                case FigureActions.DropFigure:
                case FigureActions.HideFigure:
                case FigureActions.CapFigure:
                    DestroyFigure.Execute(CreateGameplaySquare(tryDropJson.Id, tryDropJson.Position));
                    break;
            }
        }
        
        public void UpdateGameplayFigures(List<AbstractFigure> figures, List<PlacedFigure> placedFigures)
        {
            GameZoneService.UpdatePlacedFigures(placedFigures);

            if (figures.Count != placedFigures.Count)
            {
                foreach (var placedFigure in placedFigures.Where(placedFigure => figures.All(x=>x.Id != placedFigure.Id)))
                {
                    figures.Remove(figures.First(x=>x.Id == placedFigure.Id));
                }
            }
            
            for (int i = 0; i < figures.Count; i++)
            {
                var uiFigure = UIFigures.First(x =>string.Equals(x.Color,figures[i].Color)).Id; 
                CreateGameplaySquare(uiFigure, placedFigures[i].Position.UnityVector, placedFigures[i].Id);
            }
        }
        
        private void DestroyGameplaySquare(AbstractFigure figure)
        {
            var removeFigure = Gameplayfigures.First(x => x == figure);
            Gameplayfigures.Remove(removeFigure);
        }
        

        public void Dispose()
        {
            _disposables?.Dispose();
            IsDragging?.Dispose();
            ActualFigureId?.Dispose() ;
            Position?.Dispose() ;
            IsUsed?.Dispose() ;
            LastAction?.Dispose();
            DestroyFigure?.Dispose();
            PlaceFigure?.Dispose();
            StartDragCommand?.Dispose();
            DragCommand?.Dispose();
            DropCommand?.Dispose();
            FixTowerCommand?.Dispose();
        }
    }
    
    public class StartDragJson
    {
        public string Id { get; private set; }
        public Vector2 Position { get; private set; }

        public StartDragJson(string id, Vector2 position)
        {
            Id = id;
            Position = position;
        }        
    }
    
    public class TryDropJson
    {
        public string Id { get; }
        public GameObject Target { get; }
        public Vector2 Position { get; }
        public float Size{ get; }
        
        public TryDropJson(GameObject target, Vector2 position, string id, float size)
        {
            Id = id;
            Target = target;
            Position = position;
            Size = size;
        }
        
    }
    public class PlaceJson
    {
        public AbstractFigure Figure{ get;}
        public Vector2 PlacePosition{ get;}

        public PlaceJson(AbstractFigure figure, Vector2 placePosition)
        {
            Figure = figure;
            PlacePosition = placePosition;
        }
    }
    
    public enum FigureActions
    {
        PlaceFigure = 0,
        DropFigure = 1,
        HideFigure = 2,
        CapFigure = 3
    }
}