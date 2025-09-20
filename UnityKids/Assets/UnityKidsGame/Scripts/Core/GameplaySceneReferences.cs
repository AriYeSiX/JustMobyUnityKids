using UnityEngine;

namespace UnityKids.Core
{
    /// <summary>
    /// Монобех ссылки для всех заинтересованных
    /// </summary>
    public class GameplaySceneReferences : MonoBehaviour
    {
        [field: SerializeField] public GameObject UIFigureParent { get; private set; }
        [field: SerializeField] public GameObject GameplayZoneObject { get; private set; }
        [field: SerializeField] public RectTransform GameplayZoneRect { get; private set; }
        [field: SerializeField] public GameObject HoleObject { get; private set; }
    }
}