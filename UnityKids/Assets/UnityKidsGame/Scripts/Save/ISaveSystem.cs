using System.Collections;
using UnityKids.Core;
using UnityKids.Gameplay;

namespace UnityKids.Save
{
    public interface ISaveSystem
    {
        public IFigureService FigureService { get; }
        public IGameZoneService GameZoneService { get; }
        
        public void Save();
        public void Load();
    }
}