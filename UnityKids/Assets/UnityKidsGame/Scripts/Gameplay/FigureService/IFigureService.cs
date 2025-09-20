using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityKids.Core;

namespace UnityKids.Gameplay
{
    public interface IFigureService
    {
        public GameplaySceneReferences GameplaySceneReferences { get; }
        public List<AbstractFigure> Gameplayfigures { get; }
        public List<AbstractFigure> UIFigures { get; }
        
        public IGameZoneService GameZoneService { get; }
        public ReactiveProperty<bool> IsDragging  { get; }
        public ReactiveProperty<string> ActualFigureId { get; }
        public ReactiveProperty<Vector2> Position { get; }
        public ReactiveProperty<bool> IsUsed { get; }
        
        public ReactiveCommand<FigureActions> LastAction { get; }
        public ReactiveCommand<AbstractFigure> DestroyFigure { get; }
        public ReactiveCommand<PlaceJson> PlaceFigure { get; }
        public ReactiveCommand<StartDragJson> StartDragCommand { get; }
        public ReactiveCommand<Vector2> DragCommand { get; }
        public ReactiveCommand<TryDropJson> DropCommand { get; }
        public ReactiveCommand<List<FigureMovementData>> FixTowerCommand{ get; }

        public void UpdateGameplayFigures(List<AbstractFigure> figures, List<PlacedFigure> placedFigures);
    }
}