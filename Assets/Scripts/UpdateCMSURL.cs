using Cysharp.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UpdateCMSURL : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textStatus;
    [SerializeField] TMP_InputField cmsIPInputField;
    [SerializeField] UnityEngine.UI.Button continueButton;

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
        continueButton.interactable = false;
        PlayerPrefs.SetString(nameof(cmsIPInputField), address.ToString());
        textStatus.text = string.Empty;
        UpdateBaseIPDelay();
    }   

    async void UpdateBaseIPDelay()
    {           
        Url.baseIp = cmsIPInputField.text;
        Url.apiUrl = "http://" + Url.baseIp + "/mrm_showroom/";
        Debug.Log(Url.apiUrl);
        WWWForm form = new WWWForm();
        form.AddField("id", "1");
        string url = Url.apiUrl + Url.appStatus;
        await WWWRequestTC.Post( url, form, new HeaderDataClass[0], (responseJson, isSuccess) =>
        {
            if (!isSuccess)
            {
                textStatus.text = "Unable to connect to the server!";
                textStatus.color = Color.red;
                continueButton.interactable = true;
                return;
            }
            AppStatus appStatus = JsonUtility.FromJson<AppStatus>(responseJson);
            if (appStatus == null || !appStatus.success)
            {
                textStatus.text = "App Not Authorised";
                textStatus.color = Color.red;
                Debug.Log("App Not Authorised.");
                continueButton.interactable = true;
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
