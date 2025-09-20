using UnityEngine;
using UnityKids.Gameplay;

namespace UnityKids.Creators
{
    /// <summary>
    /// Фабрика геймплей кубов
    /// </summary>
    public class GameplaySquareCreator : AbstractCreator<GameplaySquareView>
    {
        public override GameplaySquareView Create(Vector3 position, Transform parent)
        {
            var squareView = Object.Instantiate(_prefab, parent, false);
            squareView.transform.localScale = Vector3.one;
            squareView.transform.position = position;
            if (squareView.transform.localPosition.z != 0) 
            {
                squareView.transform.localPosition = new Vector3(squareView.transform.localPosition.x,squareView.transform.localPosition.y,0f);
            }
            return squareView;
        }
        
        public GameplaySquareCreator(GameplaySquareView prefab) : base(prefab)
        {
            _prefab = prefab;
        }
    }
}