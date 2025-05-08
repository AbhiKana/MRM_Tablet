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
    public RawImage CircleImage, TopMarbleImage;
    [SerializeField] UI_Manager ui;
    
    [SerializeField] string fetchedData;
    
    public RawImage[] BgImages;    
    public Texture[] boxImageTexture;

    WWWRequestTC requestTC;

    public UnityEvent OnDataLoaded;
    public UnityEvent OnDataLoadError = new UnityEvent();

    [SerializeField] int totalImageCount = 0;
    [SerializeField] int downlaodedImageCount = 0;
    private void Start()
    {
        requestTC = new WWWRequestTC();
        QRScanner.OnQRDetect.AddListener(LoadData);
        OnDataLoaded.AddListener(() =>
        {
            ui.OpenPage(3);
            StoreTexture();
        });
    }

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
                OnDataLoadError?.Invoke();
                Debug.Log("Couldn't fetch data");
            }
        });
    }

    // load marble text data after QR scan From Scriptable object
    void LoadMarbleTextData()
    {
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
            GetImageTop(marbleDet.main_img, TopMarbleImage);

            totalImageCount = _marbleQrDatascritable._marbleApiData.marbleDetails.texture_img.Count;

            GetImage(marbleDet.texture_img[0], CircleImage);


            var count = _marbleQrDatascritable._marbleApiData.marbleDetails.texture_img.Count;
            for (int i = 1; i < count; i++)
            {
                GetImage(marbleDet.texture_img[i], BgImages[i - 1]);
            }
        }
    }

    void StoreTexture()
    {
        Debug.Log("Total Count: " + totalImageCount);
        for (int i = 0; i < totalImageCount; i++)
        {
            boxImageTexture[i] = BgImages[i].texture;
        }
    }

    void GetImage(string url, RawImage image)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) => 
        {
            if (isSucess)
            {
                //Debug.LogError("URL: " + url);
                image.texture = rawTex;
                downlaodedImageCount++;

                if (totalImageCount == downlaodedImageCount)
                {
                    OnDataLoaded?.Invoke();
                    Debug.Log("All Image downloaded");
                    downlaodedImageCount = 0;
                }
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }

    void GetImageTop(string url, RawImage image)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                //Debug.LogError("URL main: " + url + " " + image.name);
                image.texture = rawTex;
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }
}
