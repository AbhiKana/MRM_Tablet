using UnityEngine;
using UnityEngine.Events;

public class LoadImageInBG : MonoBehaviour
{
    [HideInInspector]public ShowMarbleDetails showMarbleDetails;
    WWWRequestTC requestTC;

    int totalImageCount, downlaodedImageCount;

    public UnityEvent OnBGImageDownload = new UnityEvent();

    void Start()
    {
        requestTC = new WWWRequestTC();
        showMarbleDetails = GetComponent<ShowMarbleDetails>();

        OnBGImageDownload.AddListener(()=>
        {
            var dataSync = showMarbleDetails.syncMarbleDetails;
            dataSync.SyncMarbleArrayTexture(showMarbleDetails, dataSync.marqueeMarbles);

            showMarbleDetails.storeMarbleDetails.StoreTexturesInList(showMarbleDetails.tileID, showMarbleDetails);
        });
    }

    public void LoadMarbleImageData()
    {
        var marbleDet = showMarbleDetails.marbleDetailsWithCategoryID;
        if (marbleDet.texture_img.Count > 0)
        {
            if (showMarbleDetails.m_Textures.Length < 1)
                showMarbleDetails.m_Textures = new Texture[5];

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
