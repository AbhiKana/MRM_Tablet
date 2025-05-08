using UnityEngine;
using UnityEngine.SceneManagement;

namespace Udar.SceneManager
{
    public class SceneFieldRef : MonoBehaviour
    {
        [field: SerializeField] public SceneField SceneField;
        public LoadSceneMode SceneMode;
    }
}