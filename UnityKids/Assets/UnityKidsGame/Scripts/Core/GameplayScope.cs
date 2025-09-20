using UnityEngine;
using UnityKids.Data;
using UnityKids.Gameplay;
using UnityKids.Save;
using VContainer;
using VContainer.Unity;

namespace UnityKids.Core
{
    /// <summary>
    /// Инициализация DI и запуск entry point
    /// </summary>
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private GameplaySceneReferences _gameplaySceneReferences;
        [SerializeField] private GameConfiguration _gameConfiguration;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_gameplaySceneReferences);
            builder.RegisterComponent(_gameConfiguration);
            
            builder.Register<IFigureService, FigureService>(Lifetime.Singleton);
            builder.Register<IGameZoneService, GameZoneService>(Lifetime.Singleton);
            builder.Register<ISaveSystem, SaveSystem>(Lifetime.Singleton);
            builder.Register<FigureListener>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<GameplayFlow>();
        }
    }
    
}
