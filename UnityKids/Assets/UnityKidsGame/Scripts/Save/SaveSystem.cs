using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityKids.Core;
using UnityKids.Gameplay;
using VContainer;

namespace UnityKids.Save
{
    /// <summary>
    /// Реализация системы сохранений
    /// </summary>
    public class SaveSystem : ISaveSystem, IDisposable
    {
        private const string FIGURE_SERVICE_SAVE_KEY = "FIGURE_SERVICE_SAVE_KEY";
        private const string GAME_ZONE_SERVICE_SAVE_KEY = "GAME_ZONE_SERVICE_SAVE_KEY";
        public IFigureService FigureService { get; private set; }
        public IGameZoneService GameZoneService { get; private set; }

        [Inject]
        public void Construct(IFigureService figureService, IGameZoneService gameZoneService)
        {
            FigureService = figureService;
            GameZoneService = gameZoneService;
        }
        
        public void Save()
        {
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => 
                {
                    args.ErrorContext.Handled = true;
                }
            };
            var figureSave = JsonConvert.SerializeObject(FigureService.Gameplayfigures, Formatting.Indented, settings);
            PlayerPrefs.SetString(FIGURE_SERVICE_SAVE_KEY, figureSave);
            var zoneSave = JsonConvert.SerializeObject(GameZoneService.PlacedFigures, Formatting.Indented);
            PlayerPrefs.SetString(GAME_ZONE_SERVICE_SAVE_KEY, zoneSave);
            
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(FIGURE_SERVICE_SAVE_KEY))
            {
                var figureSave = PlayerPrefs.GetString(FIGURE_SERVICE_SAVE_KEY);
                var figureService = JsonConvert.DeserializeObject<List<GameplaySquare>>(figureSave);
                var abstractList = figureService.Cast<AbstractFigure>().ToList();
                
                var zoneSave = PlayerPrefs.GetString(GAME_ZONE_SERVICE_SAVE_KEY);
                var zoneService = JsonConvert.DeserializeObject<List<PlacedFigure>>(zoneSave);
                FigureService.UpdateGameplayFigures(abstractList, zoneService);
            }
        }

        public void Dispose()
        {
            Save();
        }
    }
}