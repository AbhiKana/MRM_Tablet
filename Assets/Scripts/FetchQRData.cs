using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FetchQRData : MonoBehaviour
{
    [SerializeField] UI_Manager ui;
    public MarbleQRDATA _marbleQrDatascritable;
    public SpecificMarbleDetails _specificMarbleDetails;

    // Text details info
    [SerializeField] TextMeshProUGUI MarbleName, MarbleDetails;
    [SerializeField] TextMeshProUGUI MarbleDimension, MarbleType, MarbleOrigin, MarbleAvailability, MarblePrice;
    public RawImage CircleImage, TopMarbleImage;

    [SerializeField] string fetchedData;

    public RawImage[] BgImages;
    public Texture[] boxImageTexture;

    //public bool IsDetailView = false;   

    public UnityEvent OnDataLoaded;
    public UnityEvent OnDataLoadError;

    [SerializeField] int totalImageCount = 0;
    [SerializeField] int downlaodedImageCount = 0;
    private CancellationTokenSource qrTextureToken = new CancellationTokenSource();

    private void Start()
    {
        QRScanner.OnQRDetect.AddListener(async (data) => await LoadData(data));
        OnDataLoaded.AddListener(() =>
        {
            //if (!IsDetailView)
            ui.OpenPage(3);
            //StoreTexture();
        });
    }

    public async UniTask LoadData(string data)
    {
        Debug.Log(data);
        string url = Url.apiUrl + Url.marbleDetails;

        WWWForm form = new WWWForm();
        form.AddField("id", data);
        var (Data, isSucess) = await WWWRequestTC.Post(url, form, new HeaderDataClass[0]);
        if (isSucess)
        {
            fetchedData = Data;
            Debug.Log("QR scanned data: " + fetchedData);
            _marbleQrDatascritable._marbleApiData = JsonUtility.FromJson<MarbleApiData>(Data);

            if (_marbleQrDatascritable._marbleApiData.success)
            {
                LoadMarbleTextData();
                await LoadMarbleImageData();
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
    }

    public void LoadedData(string data)
    {
    }

    // load marble text data after QR scan From Scriptable object
    void LoadMarbleTextData()
    {
        var marble = _marbleQrDatascritable._marbleApiData.marbleDetails;
        _specificMarbleDetails.id = marble.id;
        _specificMarbleDetails.marble_name = MarbleName.text = marble.marble_name;
        _specificMarbleDetails.description = MarbleDetails.text = marble.description;
        _specificMarbleDetails.dimension = MarbleDimension.text = marble.dimension;
        _specificMarbleDetails.material = MarbleType.text = marble.material;
        _specificMarbleDetails.finish = MarbleOrigin.text = marble.finish;
        MarblePrice.text = marble.price + " sq/ft";
        _specificMarbleDetails.price = marble.price;
        _specificMarbleDetails.url = marble.main_img;
        MarbleAvailability.text = marble.availability.ToString();
        _specificMarbleDetails.availability = marble.availability;
    }
    public void LoadMarbleTextData(string marble_name, string description, string dimension, string material, string finish, string availibility, string price)
    {
        MarbleName.text = marble_name;
        MarbleDetails.text = description;
        MarbleDimension.text = dimension;
        MarbleType.text = material;
        MarbleOrigin.text = finish;
        MarbleAvailability.text = availibility;
        MarblePrice.text = price + " sq/ft";
    }
    public void LoadedMarbleImageData(Texture mainTexture, Texture circleImg, Texture[] textures)
    {
        var marbleDet = _specificMarbleDetails;
        TopMarbleImage.texture = mainTexture;
        //CircleImage.texture = circleImg;
        //for (int i = 0; i < marbleDet.textures.Length; i++)
        //{
        //    BgImages[i].texture = textures[i];
        //}
    }
    // load marble Image data after QR scan From Scriptable object
    async UniTask LoadMarbleImageData()
    {
        var marbleDet = _marbleQrDatascritable._marbleApiData.marbleDetails;
        await GetImageTop(marbleDet.main_img, TopMarbleImage);
        //if (marbleDet.texture_img.Count > 0)
        //{
        //    totalImageCount = _marbleQrDatascritable._marbleApiData.marbleDetails.texture_img.Count;
        //    GetImage(marbleDet.texture_img[0], CircleImage);
        //    var count = _marbleQrDatascritable._marbleApiData.marbleDetails.texture_img.Count;
        //    for (int i = 1; i < count; i++)
        //    {
        //        GetImage(marbleDet.texture_img[i], BgImages[i - 1]);
        //    }
        //}
    }
    void StoreTexture()
    {
        Debug.Log("Total Count: " + totalImageCount);
        for (int i = 0; i < totalImageCount; i++)
        {
            boxImageTexture[i] = BgImages[i].texture;
        }
    }
    async UniTask GetImage(string url, RawImage image)
    {
        await WWWRequestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                //Debug.LogError("URL: " + url);
                TextureScale.Bilinear(rawTex, 200, 200);
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

    async UniTask GetImageTop(string url, RawImage image)
    {
        CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
        qrTextureToken.Token,
        this.GetCancellationTokenOnDestroy());
        await WWWRequestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                TextureScale.Bilinear(rawTex, 100, 100);
                //Debug.LogError("URL main: " + url + " " + image.name);
                image.texture = rawTex;
                OnDataLoaded?.Invoke();
            }
            else
                Debug.Log("Couldn't fetch image data");
        }, linkedCts.Token, true);
    }
}
