using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadImageInBG : MonoBehaviour
{
    [HideInInspector] public ShowMarbleDetails showMarbleDetails;
    [SerializeField] RawImage dummy;

    public int totalImageCount, downlaodedImageCount;

    public UnityEvent OnBGImageDownload = new UnityEvent();

    void Start()
    {
        showMarbleDetails = GetComponent<ShowMarbleDetails>();

        OnBGImageDownload.AddListener(() =>
        {
            var dataSync = showMarbleDetails.syncMarbleDetails;
            dataSync.SyncMarbleArrayTexture(showMarbleDetails, dataSync.marqueeMarbles);

            showMarbleDetails.storeMarbleDetails.StoreTexturesInList(showMarbleDetails.tileID, showMarbleDetails);
        });
    }

    //public async UniTask LoadMarbleImageData()
    //{
    //    var marbleDet = showMarbleDetails.marbleDetailsWithCategoryID;
    //    if (marbleDet.texture_img.Count > 0)
    //    {
    //        if (showMarbleDetails.m_Textures.Length < 1)
    //            showMarbleDetails.m_Textures = new Texture[5];

    //        totalImageCount = marbleDet.texture_img.Count;
    //       // GetImageUsingthread(marbleDet.texture_img.ToArray());

    //        for (int i = 0; i < marbleDet.texture_img.Count; i++)
    //        {
    //            //GetImageUsingthread(marbleDet.texture_img[i], i);
    //            await GetImage(marbleDet.texture_img[i], i);
    //        }
    //    }
    //}
    void GetImageUsingthread(string[] url)
    {
        int count = 0;
        ThreadedImageDownloader.Instance.DownloadAndProcessImage
        (
            url,
            dummy,
            (success) =>
            {
                //showMarbleDetails.m_Textures[count] = dummy.texture;
                if (success)
                    CheckAllImageLoaded();
            },
            () => { count++; Debug.Log("Count: " + count); }
        );

        /*AdvancedImageDownloader.Instance.DownloadMultipleImages
        (
            url, 
            dummy.texture,
            (value)=> { count++; CheckAllImageLoaded(); showMarbleDetails.m_Textures[value] = dummy.texture; }
        );*/
    }

    async UniTask GetImage(string url, int textureIndex)
    {
        await WWWRequestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                TextureScale.Bilinear(rawTex, 200, 200);
                //showMarbleDetails.m_Textures[textureIndex] = rawTex;
                CheckAllImageLoaded();
            }
            else
            {
                Debug.Log("<color=red>Couldn't fetch image data</color>");
            }
        });
    }
    private void CheckAllImageLoaded()
    {
        downlaodedImageCount++;
        if (totalImageCount == downlaodedImageCount)
        {
            StopLoader();
        }
    }

    private void StopLoader()
    {
        OnBGImageDownload?.Invoke();
        Debug.Log("<color=green>All Images downloaded</color>");
        downlaodedImageCount = 0;
    }
}