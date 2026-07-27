using System.Collections;
using System.Net;
using TMPro;
using UnityEngine;

public class UpdateCMSURL : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textStatus;
    [SerializeField] TMP_InputField cmsIPInputField;

    [SerializeField] GameObject[] clientConnectObject;

    private void Start()
    {
        if (PlayerPrefs.HasKey(nameof(cmsIPInputField)))
        {
            cmsIPInputField.text = PlayerPrefs.GetString(nameof(cmsIPInputField));
            SubmitButtonForCMSIP();
        }
    }

    public void SubmitButtonForCMSIP()
    {
        bool isvalid = IPAddress.TryParse(cmsIPInputField.text, out IPAddress address);
        if (!isvalid) return;
        PlayerPrefs.SetString(nameof(cmsIPInputField), address.ToString());
        textStatus.text = string.Empty;
        DelayThis();
    }

    private void DelayThis()
    {
        StartCoroutine(UpdateBaseIPDelay());
    }

    IEnumerator UpdateBaseIPDelay()
    {
        yield return new WaitForSeconds(0.2f);       
        Url.baseIp = cmsIPInputField.text;
        Url.apiUrl = "http://" + Url.baseIp + "/mrm_showroom/";
        Debug.Log(Url.apiUrl);
        WWWForm form = new WWWForm();
        form.AddField("id", "1");
        WWWRequestTC tC = new WWWRequestTC();
        string url = Url.apiUrl + Url.appStatus;
        tC.Post(form, url, (responseJson, isSuccess) =>
        {
            if (!isSuccess)
            {
                textStatus.text = "Unable to connect to the server!";
                textStatus.color = Color.red;
                return;
            }
            AppStatus appStatus = JsonUtility.FromJson<AppStatus>(responseJson);
            if (appStatus == null || !appStatus.success)
            {
                textStatus.text = "App Not Authorised";
                textStatus.color = Color.red;
                Debug.Log("App Not Authorised.");
                return;
            }

            textStatus.text = "App Subscribed";
            textStatus.color = Color.green;
            Debug.Log("App Subscribed");

            foreach (var obj in clientConnectObject)
            {
                obj.SetActive(true);
            }
            gameObject.SetActive(false);
        });

    }
}
