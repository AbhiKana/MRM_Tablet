using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MarbleApiData;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class FetchData : MonoBehaviour
{
    //  public MarbleApiData _marbleApiData;
    public MarbleQRDATA _marbleQrDatascritable;

    // Text details info
    [SerializeField] TextMeshProUGUI MarbleName, MarbleDetails;
    [SerializeField] TextMeshProUGUI MarbleDimension, MarbleMaterial, MarbleFinish;
    [SerializeField] RawImage CircleImage, TopMarbleImage;
    [SerializeField] RawImage[] BgImages;
    public void GetDataFrom(string url, string id)
    {
        StartCoroutine(PostRequest(url, id));
    }

    IEnumerator PostRequest(string url, string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                //  _marbleApiData = MarbleApiData();
                // assign json data of scanned marble to scriptable object
                _marbleQrDatascritable._marbleApiData = JsonUtility.FromJson<MarbleApiData>(request.downloadHandler.text);
                LoadMarbleTextData();
                LoadMarbleImageData();
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    // load marble text data after QR scan From Scriptable object
    void LoadMarbleTextData()
    {
        print(_marbleQrDatascritable._marbleApiData.marbleDetails.marble_name);
        MarbleName.text = _marbleQrDatascritable._marbleApiData.marbleDetails.marble_name;
        MarbleDetails.text = _marbleQrDatascritable._marbleApiData.marbleDetails.description;
        MarbleDimension.text = _marbleQrDatascritable._marbleApiData.marbleDetails.dimension;
        MarbleMaterial.text = _marbleQrDatascritable._marbleApiData.marbleDetails.material;
        MarbleFinish.text = _marbleQrDatascritable._marbleApiData.marbleDetails.finish;
    }

    // load marble Image data after QR scan From Scriptable object
    void LoadMarbleImageData()
    {
        StartCoroutine(LoadImage(_marbleQrDatascritable._marbleApiData.marbleDetails.texture_img[0], CircleImage));
        StartCoroutine(LoadImage(_marbleQrDatascritable._marbleApiData.marbleDetails.main_img, TopMarbleImage));

        var count = _marbleQrDatascritable._marbleApiData.marbleDetails.texture_img.Count;
        for (int i = 1; i < count; i++)
        {
            StartCoroutine(LoadImage(_marbleQrDatascritable._marbleApiData.marbleDetails.texture_img[i], BgImages[i-1]));
        }
    }

    IEnumerator LoadImage(string ImageUrl, RawImage rawImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(ImageUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            yield return new WaitUntil(() => request.isDone);

            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            //_Displayimage.rectTransform.sizeDelta = CardImageSizeToParent(_Displayimage);
        }
        request.Dispose();
    }
}
