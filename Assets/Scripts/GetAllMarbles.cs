using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GetAllMarbles : MonoBehaviour
{
    public AllMarbles allMarbles;
    WWWRequestTC requestTC;

    public UnityEvent OnAllMarbleDataLoaded;
    public UnityEvent OnDataLoaded;

    bool IsAllImageDownloaded;
    private void Start()
    {
        requestTC = new WWWRequestTC();
        GetAllMarbleList();
        //OnDataLoaded.AddListener()
    }

    public void GetAllMarbleList()
    {
        string url = Url.apiUrl + Url.marbleApi;
        requestTC.Get(url, (Data, isSucess) =>
        {
            allMarbles = JsonUtility.FromJson<AllMarbles>(Data);
            totalImageCount = allMarbles.marbleDetails.Count;
            Debug.Log("Data Fetched");
            OnAllMarbleDataLoaded?.Invoke();
        });
    }

    public void SetMarbleDetails(List<ShowMarbleDetails> showMarbleDetails)
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
    }
}