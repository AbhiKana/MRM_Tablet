using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FetchQRData : MonoBehaviour
{
    [SerializeField] GameObject mrmDetails;
    [SerializeField] BackButtonController backButton;
    [SerializeField] FetchData fetchData;
    private void Start()
    {
        QRScanner.OnQRDetect.AddListener(LoadData);
    }

    public void LoadData(string data)
    {
        Debug.Log(data);
        backButton.OpenPage(mrmDetails);
        string url = Url.apiUrl + Url.marbleDetails;
        fetchData.GetDataFrom(url, data);
    }
}
