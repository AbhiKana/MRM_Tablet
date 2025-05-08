using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Udar.SceneManager;


public class SceneSwitchManager : MonoBehaviour
{
    public static SceneSwitchManager instance;  

    [SerializeField] private RectTransform progressHolder;
    [SerializeField] private UnityEngine.UI.Image progressBarImage;
    private float _target;
    public UnityEvent sceneChangeEvent;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        progressHolder.gameObject.SetActive(false);
    }

    public void LoadScene(SceneFieldRef sceneIndex)
    {
        LoadScene_Coroutine(sceneIndex);
    }   

    public async void LoadScene_Coroutine(SceneFieldRef index)
    {
        progressHolder.gameObject.SetActive(true);        
        progressBarImage.fillAmount = _target = 0;

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(index.SceneField.Name, index.SceneMode);
        asyncOp.allowSceneActivation = false;

        while (!asyncOp.isDone)
        {
            progressBarImage.fillAmount = _target = Mathf.MoveTowards(_target, asyncOp.progress, Time.deltaTime);
            
            if (_target >= 0.9f)
            {
                progressBarImage.fillAmount = _target;
                asyncOp.allowSceneActivation = true;
            }
            //yield return null;
            await Task.Yield();
        }
        //add your function or event here
        sceneChangeEvent?.Invoke();
        progressHolder.gameObject.SetActive(false);
    }

    public void UnLoadScene(SceneFieldRef index)
    {
        SceneManager.UnloadSceneAsync(index.SceneField.Name);
    }
}
