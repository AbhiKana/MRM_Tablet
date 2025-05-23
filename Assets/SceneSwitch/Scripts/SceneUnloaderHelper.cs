using Udar.SceneManager;
using UnityEngine;
using UnityEngine.UI;

public class SceneUnloaderHelper : MonoBehaviour
{
    Button closeButton;
    //[SerializeField] SceneSwitchManager sceneSwitchManager;
    [SerializeField] SceneFieldRef sceneFieldRef;
    void Start()
    {
        closeButton = GetComponent<Button>();
        //sceneSwitchManager = FindAnyObjectByType<SceneSwitchManager>();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                SceneSwitchManager.instance.UnLoadScene(sceneFieldRef);
                //sceneSwitchManager.UnLoadScene(sceneFieldRef);
            });
        }
    }
}
