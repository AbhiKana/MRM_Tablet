using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InvoiceManager : MonoBehaviour
{
    public static InvoiceManager _invoiceMananger;
    //Upload marble invoice data to cms 
    public string cmsUrl = "YOUR_CMS_ENDPOINT_URL";
    public MarbleInvoice MarbleInvoiceData = new MarbleInvoice();
    void Awake()
    {
        if (_invoiceMananger != null)
        {
            Destroy(gameObject);
            return;
        }
        _invoiceMananger = this;
    }

    // call this on share button
    public void OnclickOfShareInvoice()
    {
        StartCoroutine(UploadData(MarbleInvoiceData));
    }

    IEnumerator UploadData(MarbleInvoice data)
    {
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(cmsUrl, "POST"))
        {
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Upload failed: {request.error}");
            }
            else
            {
                Debug.Log("Upload successful!");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}