using UnityEngine;
using UnityKids.Gameplay;

namespace UnityKids.Creators
{
    /// <summary>
    /// Фабрика интерфейсных кубов
    /// </summary>
    public class UISquareCreator : AbstractCreator<UISquareView>
    {
        public override UISquareView Create(Vector3 position, Transform parent)
        {
            var uiSquareView = Object.Instantiate(_prefab, parent, false);
            uiSquareView.transform.localScale = Vector3.one;
            return uiSquareView;
        }
        
        public UISquareCreator(UISquareView prefab) : base(prefab)
        {
            _prefab = prefab;
        }
    }
}