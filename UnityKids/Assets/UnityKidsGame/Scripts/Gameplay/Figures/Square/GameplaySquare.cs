using System;
using UnityEngine;

namespace UnityKids.Gameplay
{
    [Serializable]
    public class GameplaySquare : Square
    {
        public GameplaySquare(string id, string color, Sprite sprite, string type = "Square") : base(id, color, sprite, type) { }
    }
}