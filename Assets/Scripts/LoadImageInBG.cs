using UnityEngine;
using UnityEngine.Events;

public class LoadImageInBG : MonoBehaviour
{
    private ShowMarbleDetails showMarbleDetails;
    WWWRequestTC requestTC;

    int totalImageCount, downlaodedImageCount;

    UnityEvent OnBGImageDownload = new UnityEvent();

    void Start()
    {
        requestTC = new WWWRequestTC();
        showMarbleDetails = GetComponent<ShowMarbleDetails>();

        OnBGImageDownload.AddListener(()=>
        {
            var dataSync = showMarbleDetails.syncMarbleDetails;
            dataSync.SyncMarbleData(showMarbleDetails, dataSync.gridMarbles);
            showMarbleDetails.storeMarbleDetails.StoreTexturesInList(showMarbleDetails.tileID, showMarbleDetails);
        });
    }

    public void LoadMarbleImageData()
    {
        var marbleDet = showMarbleDetails.marbleDetailsWithCategoryID;
        if (marbleDet.texture_img.Count > 0)
        {
            //GetImage(showMarbleDetails.texture, )
            totalImageCount = marbleDet.texture_img.Count;
            for (int i = 0; i < marbleDet.texture_img.Count; i++)
            {
                GetImage(marbleDet.texture_img[i], i);
            }
        }
    }

    void GetImage(string url, int textureIndex)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                TextureScale.Bilinear(rawTex, 200, 200);
                showMarbleDetails.m_Textures[textureIndex] = rawTex;
                downlaodedImageCount++;

                if (totalImageCount == downlaodedImageCount)
                {
                    OnBGImageDownload?.Invoke();
                    Debug.Log("<color=green>All Images downloaded</color>");
                    downlaodedImageCount = 0;
                }
            }
            else
            {
                Debug.Log("<color=red>Couldn't fetch image data</color>");
            }
        });
    }
}
