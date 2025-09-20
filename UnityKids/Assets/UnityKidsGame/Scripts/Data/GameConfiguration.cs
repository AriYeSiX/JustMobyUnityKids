using System.Collections.Generic;
using UnityEngine;
using UnityKids.Gameplay;

namespace UnityKids.Data
{
    /// <summary>
    /// Реализация игровой конфигурации
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "ScriptableObjects/Create new GameConfiguration")]
    public class GameConfiguration : ScriptableObject, IGameConfiguration
    {
        [field: SerializeField] public List<SquareData> Squares { get; private set; } = new();
        [field: SerializeField] public UISquareView UISquarePrefab { get; private set; }
        [field: SerializeField] public GameplaySquareView GameplaySquarePrefab { get; private set; }
    }
}