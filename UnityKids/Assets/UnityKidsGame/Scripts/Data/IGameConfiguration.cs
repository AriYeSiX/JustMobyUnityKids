using System.Collections.Generic;

namespace UnityKids.Data
{
    /// <summary>
    /// интерфейс для обыгрывания пункта требований номер 3
    /// </summary>
    public interface IGameConfiguration
    { 
        public List<SquareData> Squares { get; }
    }
}