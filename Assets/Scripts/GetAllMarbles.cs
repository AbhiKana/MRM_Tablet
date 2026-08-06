using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
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
        var (json,success) =  await WWWRequestTC.Get(url, new HeaderDataClass[0]);
        if(success) categories = JsonUtility.FromJson<Catergories>(json);
    }

    public async UniTask GetAllMarbleList()
    {
        WWWForm form = new WWWForm();
        string url = Url.apiUrl + Url.marbleDetails;
        await WWWRequestTC.Post(url, form, new HeaderDataClass[0], (Data, isSucess) =>
        {
            if (isSucess)
            {
                allMarbles.getMarblesList = JsonUtility.FromJson<GetMarblesList>(Data);
                OnAllMarbleDataLoaded?.Invoke();
            }
        });
    }

    /*public void SetMarbleDetails(List<ShowMarbleDetails> showMarbleDetails)
    {
        if (!IsAllImageDownloaded)
        {
            var mDetails = allMarbles.marbleDetails;
            for (int i = 0; i < showMarbleDetails.Count; i++)
            {
                GetImage(mDetails[i].img, showMarbleDetails[i].image);
                showMarbleDetails[i].tileName = mDetails[i].marble_name;
                showMarbleDetails[i].tileID = mDetails[i].id;
            }
        }
    }
    int totalImageCount = 0;
    int downlaodedImageCount = 0;
    void GetImage(string url, RawImage image)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                image.texture = rawTex;
                downlaodedImageCount++;

                if (totalImageCount == downlaodedImageCount)
                {
                    IsAllImageDownloaded = true;
                    OnDataLoaded?.Invoke();
                    Debug.Log("All Image downloaded");
                   downlaodedImageCount = 0;
                }
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }*/
}