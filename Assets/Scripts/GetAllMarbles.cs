using UnityEngine;
using UnityEngine.Events;

public class GetAllMarbles : MonoBehaviour
{
    public MarbleList allMarbles;
    public Catergories categories;
    //public WWWRequestTC requestTC;

    public UnityEvent OnAllMarbleDataLoaded;
    public UnityEvent OnDataLoaded;

    private void Start()
    {
       // requestTC = new WWWRequestTC();
        GetCategoryList();
        GetAllMarbleList();
    }

    public void GetCategoryList()
    {
        string url = Url.apiUrl + Url.marbleApi;
        Debug.Log(url);
        WWWRequestTC w = new WWWRequestTC();
        w.Get(url, (Data, isSucess) =>
        {
            categories = JsonUtility.FromJson<Catergories>(Data);
        });
    }

    public void GetAllMarbleList()
    {
        WWWForm form = new WWWForm();
        string url = Url.apiUrl + Url.marbleDetails;
        GetMarblesList marbles;
        WWWRequestTC w = new WWWRequestTC();
        w.Post(form, url, (Data, isSucess) =>
        {
            if (isSucess)
            {
                marbles = JsonUtility.FromJson<GetMarblesList>(Data);
                allMarbles.getMarblesList = marbles;
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