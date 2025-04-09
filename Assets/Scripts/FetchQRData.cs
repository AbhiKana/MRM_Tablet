using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FetchQRData : MonoBehaviour
{
    public MarbleQRDATA _marbleQrDatascritable;

    // Text details info
    [SerializeField] TextMeshProUGUI MarbleName, MarbleDetails;
    [SerializeField] TextMeshProUGUI MarbleDimension, MarbleMaterial, MarbleFinish;
    [SerializeField] RawImage CircleImage, TopMarbleImage;
    [SerializeField] RawImage[] BgImages;

    [SerializeField] GameObject mrmDetails;
    [SerializeField] UI_Manager ui;

    WWWRequestTC requestTC;

    public static UnityEvent OnDataLoaded = new UnityEvent();
    public static UnityEvent OnDataLoadError = new UnityEvent();

    int totalImageCount = 0;
    int downlaodedImageCount = 0;
    private void Start()
    {
        requestTC = new WWWRequestTC();
        QRScanner.OnQRDetect.AddListener(LoadData);
        OnDataLoaded.AddListener(() => ui.OpenPage(mrmDetails));
    }

    [SerializeField] string fetchedData;

    public void LoadData(string data)
    {
        Debug.Log(data);
        string url = Url.apiUrl + Url.marbleDetails;
       
        WWWForm form = new WWWForm();
        form.AddField("id", data);
        requestTC.Post(form, url, (Data, isSucess) =>
        {
            if (isSucess)
            {
                fetchedData = Data;
                Debug.Log("QR scanned data: "+fetchedData);
                _marbleQrDatascritable._marbleApiData = JsonUtility.FromJson<MarbleApiData>(Data);

                if (_marbleQrDatascritable._marbleApiData.success)
                {
                    totalImageCount = _marbleQrDatascritable._marbleApiData.marbleDetails.texture_img.Count + 1;
                    LoadMarbleTextData();
                    LoadMarbleImageData();
                }
                else
                {
                    OnDataLoadError?.Invoke();
                    Debug.Log("Couldn't fetch data");
                }
            }
            else 
            {
                Debug.Log("Couldn't fetch data");
            }
        });
    }

    // load marble text data after QR scan From Scriptable object
    void LoadMarbleTextData()
    {
        //print(_marbleQrDatascritable._marbleApiData.marbleDetails.marble_name);
        MarbleName.text = _marbleQrDatascritable._marbleApiData.marbleDetails.marble_name;
        MarbleDetails.text = _marbleQrDatascritable._marbleApiData.marbleDetails.description;
        MarbleDimension.text = _marbleQrDatascritable._marbleApiData.marbleDetails.dimension;
        MarbleMaterial.text = _marbleQrDatascritable._marbleApiData.marbleDetails.material;
        MarbleFinish.text = _marbleQrDatascritable._marbleApiData.marbleDetails.finish;
    }

    // load marble Image data after QR scan From Scriptable object
    void LoadMarbleImageData()
    {
        var marbleDet = _marbleQrDatascritable._marbleApiData.marbleDetails;
        if (marbleDet.texture_img.Count > 0)
        {
            GetImage(marbleDet.texture_img[0], CircleImage);
            GetImage(marbleDet.main_img, TopMarbleImage);

            var count = _marbleQrDatascritable._marbleApiData.marbleDetails.texture_img.Count;
            for (int i = 1; i < count; i++)
            {
                GetImage(marbleDet.texture_img[i], BgImages[i - 1]);
            }
        }
    }

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
                    Debug.Log("All Image downloaded");
                    OnDataLoaded?.Invoke();
                    downlaodedImageCount = 0;
                }
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }
}
