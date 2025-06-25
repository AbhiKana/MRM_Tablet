using UnityEngine;

public class Loader : MonoBehaviour
{
    private static Loader _instance;
    public static Loader Instance { get { return _instance; } }


    [SerializeField] private GameObject _gameObject;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void LoaderActivation(bool value)
    {
        _gameObject.SetActive(value);
    }
}
