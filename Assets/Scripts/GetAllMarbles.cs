using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GetAllMarbles : MonoBehaviour
{
    public MarbleList allMarbles;
    public Catergories categories;

    public UnityEvent OnAllMarbleDataLoaded;
    public UnityEvent OnDataLoaded;


    private async void Start()
    {
        await GetCategoryList();
        await GetAllMarbleList();
    }

    public async UniTask GetCategoryList()
    {
        string url = Url.apiUrl + Url.marbleApi;
        Debug.Log(url);
        var (json, success) = await WWWRequestTC.Get(url, new HeaderDataClass[0]);
        if (success) categories = JsonUtility.FromJson<Catergories>(json);
    }

    public async UniTask GetAllMarbleList()
    {
        WWWForm form = new WWWForm();
        string url = Url.apiUrl + Url.marbleDetails;
        var (Data, isSucess) = await WWWRequestTC.Post(url, form, new HeaderDataClass[0]);
        if (isSucess)
        {
            allMarbles.getMarblesList = JsonUtility.FromJson<GetMarblesList>(Data);
            OnAllMarbleDataLoaded?.Invoke();
        }
    }  

}