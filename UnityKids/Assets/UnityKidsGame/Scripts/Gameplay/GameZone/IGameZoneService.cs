using System.Collections.Generic;
using UnityEngine;

namespace UnityKids.Gameplay
{
    public interface IGameZoneService
    {
        public RectTransform _zoneRect{ get; }
        public List<PlacedFigure> PlacedFigures { get; }
        
        public void Init(RectTransform zoneRect);
        public FigureActions TryPlaceFigure(string id, Vector2 position, float size , out Vector2 placePosition);

        public bool TryRemoveFigure(string id, out List<FigureMovementData> figuresToMove);
        public void UpdatePlacedFigures(List<PlacedFigure> placedFigures);
    }
}