using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[System.Serializable]
class StoreUserData
{
    public bool success;
    public string data;
    public string user_id;
}

class DataSendToConfig
{
    public string mail;
    public string user_id;
}

public class MarbleUploader : MonoBehaviour
{
    [SerializeField] EmailValidation emailValidation;
    [SerializeField] EmailValidation emailValidation2;

    [SerializeField] GameObject thanksForSharingObj;
    //[SerializeField] 
    [SerializeField] GameObject loginPanel;

    [SerializeField] Button configuratorButton;

    [SerializeField] StoreUserData storeUserData;

    DataSendToConfig dataSendToConfig;
    UI_Manager manager;
    public string data;
    //public StoreUserData data => storeUserData;
    public UnityEvent<string> OnDataSave;

    WWWRequestTC requestTC;
    SelectionPanelController SelectionPanelController;
    GetAllMarbles getAllMarbles;

    private void Start()
    {
        requestTC = new WWWRequestTC();
        manager = FindAnyObjectByType<UI_Manager>();

        EventHandler();
    }

    private void EventHandler()
    {
        configuratorButton.onClick.AddListener(
            () =>
            {
                CheckLoginData();
            });

        emailValidation2.EmailSuccess.AddListener(() =>
        {
            SaveUserData(emailValidation2);
        });

        OnDataSave.AddListener(ControlObjectActivtion);
    }

    public void CheckLoginData()
    {
        if (!emailValidation.isCredentialsEntered)
        {
            //manager.OpenPage(7);
            emailValidation2.gameObject.SetActive(true);
            manager.AddPageHistory(emailValidation2.gameObject);
        }
        else
        {
            Debug.Log("Send config to CMS");
            SaveUserData(emailValidation);
        }
    }

    private void ControlObjectActivtion(string data)
    {
        Debug.LogError("Data send 2");
        thanksForSharingObj.SetActive(true);
        manager.AddPageHistory(thanksForSharingObj);
        loginPanel.SetActive(false);
    }

    public void SaveUserData(EmailValidation email)
    {
        Debug.Log("Send data");
        string url = Url.apiUrl + Url.saveUserData;

        WWWForm form = new WWWForm();
        form.AddField("name", email.NameinputField.text);
        form.AddField("email", email.EmailinputField.text);

        form.AddField("marble_id", GetSelectedMarble_ID());
        form.AddField("tab_id", 1);
        requestTC.Post(form, url, (Data, isSucess) =>
        {
            if (isSucess) 
            {
                Debug.Log("On Store: " + Data);
                storeUserData = JsonUtility.FromJson<StoreUserData>(Data);
                dataSendToConfig = new DataSendToConfig
                {
                    mail = email.EmailinputField.text,
                    user_id = storeUserData.user_id
                };
                data = JsonUtility.ToJson(dataSendToConfig);

                OnDataSave?.Invoke("share");
            }
        Debug.Log("Email: "+ email.EmailinputField.text + " Name: " + email.NameinputField.text);
        });
    }

    public string GetSelectedMarble_ID()
    {
        if (SelectionPanelController == null)
            SelectionPanelController = FindObjectOfType<SelectionPanelController>();

        if (getAllMarbles == null)
            getAllMarbles = FindObjectOfType<GetAllMarbles>();

        var marbleList = getAllMarbles.allMarbles.getMarblesList.marbleDetails;
        var selectedTile = SelectionPanelController.availableTile;
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < selectedTile.Count; i++)
        {
            for (int j = 0; j < marbleList.Count; j++)
            {
                if (marbleList[j].marble_name == selectedTile[i].tileNameStr)
                {
                    s.Append(marbleList[i].id + ",");
                    break;
                }
            }
        }

        return EditStringValye(s);
    }

    private string EditStringValye(StringBuilder s)
    {
        if (!string.IsNullOrEmpty(s.ToString()))
        {
            string newString = s.ToString();
            var idx = newString.LastIndexOf(",");

            newString = newString.Remove(idx, 1);
            return newString;
        }
        else
            return "";
    }
}
