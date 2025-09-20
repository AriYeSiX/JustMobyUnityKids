using System;
using UnityEngine;

namespace UnityKids.Gameplay
{
    [Serializable]
    public class UISquare : Square
    {
        public UISquare(string id, string color, Sprite sprite, string type = "SquareUI") : base(id, color, sprite, type) { }
    }
}