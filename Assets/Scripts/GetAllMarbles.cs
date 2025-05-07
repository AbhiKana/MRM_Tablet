using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GetAllMarbles : MonoBehaviour
{
    public AllMarbles allMarbles;
    public WWWRequestTC requestTC;

    public UnityEvent OnAllMarbleDataLoaded;
    public UnityEvent OnDataLoaded;

    private void Start()
    {
        requestTC = new WWWRequestTC();
        GetAllMarbleList();
    }

    public void GetAllMarbleList()
    {
        //string url = Url.apiUrl + Url.marbleApi;
        string url = Url.marbleDetails;
        requestTC.Get(url, (Data, isSucess) =>
        {
            allMarbles = JsonUtility.FromJson<AllMarbles>(Data);
            Debug.Log("Data Fetched");
            OnAllMarbleDataLoaded?.Invoke();
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