using System;
using UnityEngine;

namespace UnityKids.Data
{
    [Serializable]
    public struct SquareData
    {
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
    }
}