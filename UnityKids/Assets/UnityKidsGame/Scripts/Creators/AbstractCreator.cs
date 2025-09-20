using UnityEngine;

namespace UnityKids.Creators
{
    /// <summary>
    /// generic абстракция фабрики
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractCreator<T>
    {
        protected T _prefab;
        public abstract T Create(Vector3 position, Transform parent);

        public AbstractCreator(T prefab)
        {
            _prefab = prefab;
        }
    }
}