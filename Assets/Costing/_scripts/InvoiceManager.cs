using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InvoiceManager : MonoBehaviour
{
    // "http://192.168.1.151/mrm_showroom/api/marble_quotation



    public static InvoiceManager _invoiceMananger;
    //Upload marble invoice data to cms 
    public string CostcmsUrl;
    public MarbleInvoice MarbleInvoiceData;
    public Transform ShowShareMsg;
    void Awake()
    {
        if (_invoiceMananger != null)
        {
            Destroy(gameObject);
            return;
        }
        _invoiceMananger = this;
    }
    void Start()
    {
        ShowShareMsg.gameObject.SetActive(false);
    }
    // call this on share button
    public void OnclickOfShareInvoice()
    {
        StartCoroutine(UploadData(MarbleInvoiceData));
    }

    IEnumerator UploadData(MarbleInvoice data)
    {
        string jsonData = JsonUtility.ToJson(data);
        // print(">>>" + jsonData);

        // UnityWebRequest request = new UnityWebRequest(CostcmsUrl, "POST");
        CostcmsUrl = Url.apiUrl + Url.costcms;
        UnityWebRequest request = UnityWebRequest.Put(CostcmsUrl, jsonData);
        // to send raw json data to cms
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.certificateHandler = new CertificateWhore();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload successful!");
            Debug.Log("Response: " + request.downloadHandler.text);
            // show thank u for sharing page
            ShowShareMsg.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"Upload failed: {request.error}");
        }

    }

    //In case there is a certificate error
    public class CertificateWhore : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}