using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace UnityKids.Gameplay
{
    [Serializable]
    public abstract class AbstractFigure
    {
        public string Type { get; protected set; }
        public string Id { get; protected set; }
        public String Color { get; protected set; }
        
        [JsonIgnore] public Sprite Sprite { get; protected set; }
        
        protected AbstractFigure(string type, string id, string color, Sprite sprite)
        {
            Type = type;
            Id = id;
            Color = color;
            Sprite = sprite;
        }
    }
}