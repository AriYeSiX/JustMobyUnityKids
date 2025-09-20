using System;
using UnityEngine;

namespace UnityKids.Gameplay
{
    [Serializable]
    public class Square : AbstractFigure
    {
        protected Square(string id, string color, Sprite sprite, string type = "Square") : base(type, id, color, sprite) { }
    }
}