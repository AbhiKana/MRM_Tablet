using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using UnityEngine.Events;
using DG.Tweening;
using System.Threading.Tasks;

public class QRScanner : MonoBehaviour
{
    [SerializeField] FetchQRData fetchQRData;
    WebCamTexture webcamTexture;
    string QrCode = string.Empty;
    public RawImage rawImage;
    public GameObject spinner;
    public Transform BorderBox;
    public IBarcodeReader barcodeReader;

    public static UnityEvent<string> OnQRDetect = new UnityEvent<string>();
    RenderTexture rawimageTexture;

    private void Awake()
    {
        fetchQRData.OnDataLoaded.AddListener(Stop);
        fetchQRData.OnDataLoadError.AddListener(() =>
        {
            Rescan();
        });
    }

    void OnEnable()
    {
        Scan();
    }

    private async void Rescan()
    {
        Stop();
        await Task.Delay(200);
        Scan();
    }

    private void Scan()
    {
        BorderBox.localScale = Vector3.one;
        Debug.Log("Scanner Open");
        QrCode = string.Empty;
        spinner.SetActive(false);
        BorderBox.gameObject.SetActive(false);

        StartWebcam();
        StartCoroutine(GetQRCode());
    }

    void StartWebcam()
    {
        //webcamTexture = new WebCamTexture();
        webcamTexture = new WebCamTexture(742,456,30);
        Debug.Log(webcamTexture.dimension);  
        rawImage.texture = webcamTexture;        
        rawimageTexture = rawImage.texture as RenderTexture;
        webcamTexture.Play();
    }

    void Stopwebcam()
    {
        webcamTexture.Stop();
        rawimageTexture?.Release();
        //rawImage.texture = null;
    }

    IEnumerator GetQRCode()
    {
        BorderBox.gameObject.SetActive(true);
        Debug.Log("ScanAgain");
        IBarcodeReader barCodeReader = new BarcodeReader();
        var snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);

        while (string.IsNullOrEmpty(QrCode))
        {
            try
            {
                snap.SetPixels32(webcamTexture.GetPixels32());
                var Result = barCodeReader.Decode(snap.GetRawTextureData(), webcamTexture.width, webcamTexture.height, RGBLuminanceSource.BitmapFormat.ARGB32);
                if (Result != null)
                {
                    QrCode = Result.Text;
                    if (!string.IsNullOrEmpty(QrCode))
                    {
                        Debug.Log("DECODED TEXT FROM QR: " + QrCode);
                        spinner.SetActive(true);
                        BorderBox.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f).OnComplete(() =>
                        {
                            BorderBox.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetDelay(0.2f);
                        }).SetId(this.gameObject);
                        OnQRDetect?.Invoke(QrCode);
                        break;
                    }
                }
            }
            catch (Exception ex) { Debug.LogWarning(ex.Message); }
            yield return null;
        }
        // webcamTexture.Stop();
    }

    void Stop()
    {
        if (!string.IsNullOrEmpty(QrCode))
            Debug.Log("Detected");
        else
            Debug.Log("Not Detected");

        BorderBox.localScale = Vector3.one;
        spinner.SetActive(false);
        DOTween.Kill(this.gameObject);
        Stopwebcam();
    }

    private void OnDisable()
    {
        Stop();
    }
}
