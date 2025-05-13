using System.Collections;
using UnityEngine;
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
        Debug.Log(StartCoroutine(PostRequest(url, id)));
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
                yield return request.downloadHandler.text;
                //  _marbleApiData = MarbleApiData();
                // assign json data of scanned marble to scriptable object
                
                _marbleQrDatascritable._marbleApiData = JsonUtility.FromJson<MarbleApiData>(request.downloadHandler.text);
                //LoadMarbleTextData();
                //LoadMarbleImageData();
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
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
