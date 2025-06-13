using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class DataTransmissionController : MonoBehaviour
{
    [SerializeField] public UserData userData;
    
    [SerializeField] public EmailValidation emailValidation;
    [SerializeField] public EmailValidation emailValidation2;

    [SerializeField] public GameObject loginPanel;

    [SerializeField] public Button DataSenderButton;

    protected UI_Manager manager;
    protected WWWRequestTC requestTC;
    protected GetAllMarbles getAllMarbles;
    protected SelectionPanelController SelectionPanelController;

    public string data;
    public bool OnConfigButtonClick;
    
    public UnityEvent OnDataSave;
    protected abstract void ControlObjectActivation();
    
    private void Start()
    {
        requestTC = new WWWRequestTC();
        manager = FindAnyObjectByType<UI_Manager>();
        EventHandler();
    }

    protected virtual void EventHandler()
    {
        DataSenderButton.onClick.AddListener(() =>
        {
            CheckLoginData();
            //OnConfigButtonClick = true;
        });

        emailValidation2.EmailSuccess.AddListener(() =>
        {
            if (OnConfigButtonClick)
                SaveUserData(emailValidation2);
        });

        OnDataSave.AddListener(ControlObjectActivation);
    }


    protected virtual void CheckLoginData()
    {
        if (!emailValidation.isCredentialsEntered && !emailValidation2.isCredentialsEntered)
        {
            //manager.OpenPage(7);
            emailValidation2.gameObject.SetActive(true);
            manager.AddPageHistory(emailValidation2.gameObject);

            emailValidation2.transform.Find("LoginPage").transform.Find("ButtonGroups").transform.Find("Share").GetComponent<Button>().onClick.AddListener(() =>
            {
                OnDataSave?.Invoke();
            });
        }
        else
        {
            if (emailValidation.isCredentialsEntered)
            {
                SaveUserData(emailValidation);
                OnConfigButtonClick = true;
                // OnDataSave?.Invoke();
            }
            else
            {
                SaveUserData(emailValidation2);
                OnConfigButtonClick = true;
                // OnDataSave?.Invoke();
            }
        }
    }

    protected virtual void SaveUserData(EmailValidation email)
    {
        Debug.Log("Get response from CMS"); 
        if (!userData.storeUserData.success)
        {

            string url = Url.apiUrl + Url.saveUserData;

            WWWForm form = new WWWForm();
            form.AddField("name", email.NameinputField.text);
            form.AddField("email", email.EmailinputField.text);
            form.AddField("marble_id", GetSelectedMarble_ID());
            form.AddField("tab_id", Tab_ID.GetID());

            //if (socketConnectionChecker != null)

            Debug.Log("Get response");
            WWWRequestTC w = new WWWRequestTC();
            w.Post(form, url, (Data, isSuccess) =>
            {
                Debug.Log("Response Data: " + Data);
                if (isSuccess)
                {
                    userData.storeUserData = JsonUtility.FromJson<StoreUserData>(Data);
                    MessageFormat messageFormat = new MessageFormat
                    {
                        MessageKey = "user_id",
                        MessageValue = userData.storeUserData.user_id
                    };

                    if (userData.storeUserData.already_register)
                    {
                        Debug.Log("<color=red>Update User</color>");
                        UpdateUser(messageFormat);
                    }
                    else
                    {

                        Debug.Log("<color=red>Update User</color>");
                        data = JsonUtility.ToJson(messageFormat);
                        OnDataSave?.Invoke();
                    }
                }
                else
                {
                    Debug.Log("<color=red>Not success/color>");
                }
            });
        }
        else
        {
            Debug.Log("Get response from CMS 2");

            MessageFormat messageFormat = new MessageFormat
            {
                MessageKey = "user_id",
                MessageValue = userData.storeUserData.user_id
            };
            UpdateUser(messageFormat);
        }
    }


    protected virtual void UpdateUser(MessageFormat m)
    {
        string updateUrl = Url.apiUrl + Url.updateUserData;

        WWWForm updateForm = new WWWForm();
        updateForm.AddField("user_id", userData.storeUserData.user_id);
        updateForm.AddField("marble_id", GetSelectedMarble_ID());
        updateForm.AddField("tab_id", Tab_ID.GetID());

        Debug.Log("<color=red>Use API for Update User</color>");
        WWWRequestTC w = new WWWRequestTC();
        w.Post(updateForm, updateUrl, (UpdateData, isSuccess) =>
        {
            if (isSuccess)
            {
                Debug.Log("User updated with marble ID");
                data = JsonUtility.ToJson(m);
                OnDataSave?.Invoke();
            }
            else 
            {
                Debug.Log("<color=red>Update User false</color>");
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

            Debug.Log("ID to send using Config: " + newString);
            return newString;
        }
        else
            return "";
    }
}
