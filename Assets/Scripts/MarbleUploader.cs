using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class StoreUserData
{
    public bool success;
    public bool already_register;
    public string data;
    public string user_id;
}

[System.Serializable]
public class DataSendToConfig
{
    public string mail;
    public string user_id;
}

public class MarbleUploader : MonoBehaviour
{
    [SerializeField] EmailValidation emailValidation;
    [SerializeField] EmailValidation emailValidation2;

    [SerializeField] GameObject thanksForSharingObj;
    [SerializeField] GameObject loginPanel;

    [SerializeField] Button configuratorButton;

    [SerializeField] UserData userData;
    DataSendToConfig dataSendToConfig;
    UI_Manager manager;
    public string data;
    //public StoreUserData data => storeUserData;
    public UnityEvent<string> OnDataSave;

    WWWRequestTC requestTC;
    GetAllMarbles getAllMarbles;
    SelectionPanelController SelectionPanelController;
    SocketConnectionChecker socketConnectionChecker;

    bool OnShareButtonClicked;

    private void Start()
    {
        requestTC = new WWWRequestTC();
        manager = FindAnyObjectByType<UI_Manager>();
        socketConnectionChecker = GetComponent<SocketConnectionChecker>();
        EventHandler();
    }

    private void EventHandler()
    {
        configuratorButton.onClick.AddListener(() =>
        {
            CheckLoginData();
            OnShareButtonClicked = true;
        });

        emailValidation2.EmailSuccess.AddListener(() =>
        {
            if(OnShareButtonClicked)
                SaveUserData(emailValidation2);
        });

        OnDataSave.AddListener(ControlObjectActivtion);
    }

    public void CheckLoginData()
    {
        if (!emailValidation.isCredentialsEntered && !emailValidation2.isCredentialsEntered)
        {
            //manager.OpenPage(7);
            emailValidation2.gameObject.SetActive(true);
            manager.AddPageHistory(emailValidation2.gameObject);

            emailValidation2.transform.Find("LoginPage").transform.Find("ButtonGroups").transform.Find("Share").GetComponent<Button>().onClick.AddListener(() =>
            {
                //Debug.LogError("Send data to CMS");
                OnDataSave?.Invoke("share");
            });
        }
        else
        {
            if (emailValidation.isCredentialsEntered)
            {
                Debug.Log("Send config to CMS");
                SaveUserData(emailValidation);
                ControlObjectActivtion("share");
            }
            else
            {
                Debug.Log("Send config to CMS using mail 2");
                SaveUserData(emailValidation2);
            }
        }
    }

    private void ControlObjectActivtion(string data)
    {
        //Debug.LogError("Data send 2");
        thanksForSharingObj.SetActive(true);
        manager.AddPageHistory(thanksForSharingObj);
        loginPanel.SetActive(false);
    }

    public void SaveUserData(EmailValidation email)
    {
        if (!userData.storeUserData.success)
        {
            Debug.Log("Send data");
            string url = Url.apiUrl + Url.saveUserData;

            WWWForm form = new WWWForm();
            form.AddField("name", email.NameinputField.text);
            form.AddField("email", email.EmailinputField.text);

            form.AddField("marble_id", GetSelectedMarble_ID());
            form.AddField("tab_id", socketConnectionChecker.GetID());
            requestTC.Post(form, url, (Data, isSucess) =>
            {
                if (isSucess)
                {
                    Debug.Log("On Store: " + Data);
                    userData.storeUserData = JsonUtility.FromJson<StoreUserData>(Data);

                    MessageFormat messageFormat = new MessageFormat();
                    messageFormat.MessageKey = "user_id";
                    messageFormat.MessageValue = userData.storeUserData.user_id;
                    if (userData.storeUserData.already_register)
                    {
                        Debug.Log("<color=green> Call Update API</color>");
                        UpdateUser(messageFormat);
                    }
                    else
                    {
                        data = JsonUtility.ToJson(messageFormat);
                        Pdf_Generate(userData.storeUserData.user_id);
                    }
                }
                Debug.Log("Email: " + email.EmailinputField.text + " Name: " + email.NameinputField.text);
            });
        }
        else
        {
            Debug.Log("<color=green> Update API should be call</color>");
            MessageFormat messageFormat = new MessageFormat();
            messageFormat.MessageKey = "user_id";
            messageFormat.MessageValue = userData.storeUserData.user_id;

            UpdateUser(messageFormat);
        }
    }

    private void UpdateUser(MessageFormat m)
    {
        string updateUrl = Url.apiUrl + Url.updateUserData;

        WWWForm updateForm = new WWWForm();
        updateForm.AddField("user_id", userData.storeUserData.user_id);
        updateForm.AddField("marble_id", GetSelectedMarble_ID());

        requestTC.Post(updateForm, updateUrl, (UpdateData, isSucess) =>
        {
            if (isSucess)
            {
                Debug.Log("User updated with marble ID");
                data = JsonUtility.ToJson(m);
                Pdf_Generate(userData.storeUserData.user_id);
            }
        });
    }

    private void Pdf_Generate(string user_id)
    {
        string pdfUrl = Url.apiUrl + Url.pdf;

        WWWForm form = new WWWForm();
        form.AddField("user_id", user_id);

        requestTC.Post(form, pdfUrl, (formData, isSuccess) =>
        {
            if (isSuccess)
            {
                Debug.Log("On PDF sent");
            }

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
                    s.Append(marbleList[j].id + ",");
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

            Debug.Log("ID to send usning Share: " + newString);
            return newString;
        }
        else
            return "";
    }
}
