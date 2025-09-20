using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityKids.Gameplay
{
    /// <summary>
    /// Свод правил о том когда и как помещать фигуры на игровую зону
    /// </summary>
    public class GameZoneService : IGameZoneService
    {
        public RectTransform _zoneRect { get; private set; }
        public List<PlacedFigure> PlacedFigures { get; private set; } = new List<PlacedFigure>();

        public void Init(RectTransform zoneRect)
        {
            _zoneRect = zoneRect;
            
            PlacedFigures.Clear();
        }

        public FigureActions TryPlaceFigure(string id, Vector2 position, float figureSize, out Vector2 placePosition)
        {
            placePosition = position;
            //Если фигура первая на поле
            if (PlacedFigures.Count == 0)
            {
                PlacedFigures.Add(new PlacedFigure(id, new SerializableVector2(position), figureSize));
                return FigureActions.PlaceFigure;
            }

            var highestFigure = GetHighestFigure();
            
            //Если фигура не первая и выше
            if (placePosition.y > highestFigure.Position.y)
            {
                //Если фигура над верхней
                if (placePosition.x>highestFigure.Position.x+figureSize*2)
                {
                    return FigureActions.HideFigure;
                }

                //Если фигура над верхней
                if (placePosition.x < highestFigure.Position.x - figureSize * 2)
                {
                    return FigureActions.HideFigure;
                }
                //задаем случайный отступ но не выше чем ребра фигуры
                var newX = Random.Range(highestFigure.Position.x-figureSize, highestFigure.Position.x+figureSize);
                var newY = highestFigure.Position.y+figureSize*2;
                placePosition = new Vector2(newX,newY);
                PlacedFigures.Add(new PlacedFigure(id, new SerializableVector2(placePosition), figureSize));
                return FigureActions.PlaceFigure;
            }
            //Если условия не выполнены
            return FigureActions.HideFigure;
        }

        public bool TryRemoveFigure(string id, out List<FigureMovementData> figuresToMove)
        {
            figuresToMove = new List<FigureMovementData>();
            
            var figureToRemove = PlacedFigures.FirstOrDefault(f => f.Id == id);
            if (figureToRemove == null)
                return false;

            var figuresAbove = PlacedFigures.FindAll(f => 
                f.Position.y > figureToRemove.Position.y);
            figuresAbove = figuresAbove.OrderBy(f => f.Position.y).ToList();
            var newPos = figureToRemove.Position;
            foreach (var figure in figuresAbove)
            {
                (figure.Position, newPos) = (newPos, figure.Position);
                figuresToMove.Add(new FigureMovementData(figure.Id, figure.Position.UnityVector));
            }

            PlacedFigures.Remove(figureToRemove);
            return true;
        }
        
        private PlacedFigure GetHighestFigure()
        {
            PlacedFigure highest = null;
            foreach (PlacedFigure figure in PlacedFigures)
            {
                if (highest == null || figure.Position.y > highest.Position.y)
                {
                    highest = figure;
                }
            }
            return highest;
        }

        public void UpdatePlacedFigures(List<PlacedFigure> placedFigures)
        {
            foreach (var placedFigure in placedFigures)
            {
                PlacedFigures.Add(placedFigure);
            }
        }
    }
    
    [Serializable]
    public class PlacedFigure
    {
        public string Id { get; set; }
        public SerializableVector2 Position { get; set; }
        public float Size { get; set; }

        public PlacedFigure(string id, SerializableVector2 position, float size)
        {
            Id = id;
            Position = position;
            Size = size;
        }
    }
    
    /// <summary>
    /// Данные которые передаем когда сообщаем о том каким элементам стоит подвинуться
    /// </summary>
    public class FigureMovementData
    {
        public string Id { get; set; }
        public Vector2 NewPosition { get; set; }

        public FigureMovementData(string id, Vector2 newPosition)
        {
            Id = id;
            NewPosition = newPosition;
        }
    }
}
