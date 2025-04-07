using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using UnityEngine.Events;

public class QRScanner : MonoBehaviour
{
    WebCamTexture webcamTexture;
    string QrCode = string.Empty;
    public RawImage rawImage;
    public GameObject spinner;
    public IBarcodeReader barcodeReader;

    public static UnityEvent<String> OnQRDetect = new UnityEvent<string>(); 
    void OnEnable()
    {
        QrCode = string.Empty;
        spinner.SetActive(false);
        startWebcam();
        StartCoroutine(GetQRCode());
    }

    void startWebcam()
    {
        webcamTexture = new WebCamTexture(742, 456);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }

    void Stopwebcam()
    {
        webcamTexture.Stop();
        rawImage.texture = null;
    }
    IEnumerator GetQRCode()
    {
        IBarcodeReader barCodeReader = new BarcodeReader();
        var snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);
       
        while (string.IsNullOrEmpty(QrCode))
        {
            try
            {
                snap.SetPixels32(webcamTexture.GetPixels32());
                var Result = barCodeReader.Decode(snap.GetRawTextureData(), webcamTexture.width, webcamTexture.height, RGBLuminanceSource.BitmapFormat.ARGB32);
                if (Result != null) { 
                    QrCode = Result.Text;
                    if (!string.IsNullOrEmpty(QrCode))
                    {
                        Debug.Log("DECODED TEXT FROM QR: " + QrCode);
                        spinner.SetActive(true);
                        StartCoroutine(WaitforSec());
                        break;
                    }
                }
            }
            catch (Exception ex) { Debug.LogWarning(ex.Message); }
            yield return null;
        }
       // webcamTexture.Stop();
    }
        

    IEnumerator WaitforSec()
    {
        yield return new WaitForSeconds(2.1f);
        if (!String.IsNullOrEmpty(QrCode))
        {
            OnQRDetect?.Invoke(QrCode);
            Debug.Log("Detected");
        }
        else
        {
            Debug.Log("Not Detected");
        }
            spinner.SetActive(false);
        Stopwebcam();
    }

    //private void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, w, h * 2 / 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = h * 2 / 50;
    //    style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
    //    string text =QrCode;
    //    GUI.Label(rect, text, style);
    //}
}
