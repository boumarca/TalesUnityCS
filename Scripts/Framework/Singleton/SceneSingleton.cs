using UnityEngine;

namespace Framework.Singleton
{
    public abstract class SceneSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
    }
}
