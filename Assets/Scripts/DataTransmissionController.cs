using Cysharp.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class DataTransmissionController : MonoBehaviour
{
    [SerializeField] public EmailValidation emailValidation;
    [SerializeField] public EmailValidation emailValidation2;
    [SerializeField] public Button DataSenderButton;

    protected UserData userData;
    protected UI_Manager manager;
    protected GetAllMarbles getAllMarbles;
    protected SelectionPanelController SelectionPanelController;

    public string data;
    public bool OnConfigButtonClick;

    public UnityEvent OnDataSave;

    protected abstract void ControlObjectActivation();

    private void Start()
    {
        manager = FindAnyObjectByType<UI_Manager>();
        userData = FindAnyObjectByType<UserData>();
        EventHandler();

        //OnDataSave.AddListener(() => Debug.Log("how many times get it's called"));
    }

    protected virtual void EventHandler()
    {
        DataSenderButton.onClick.AddListener(async () =>
        {
            await CheckLoginData();
            //OnConfigButtonClick = true;
        });

        emailValidation2.EmailSuccess.AddListener(async () =>
        {
            if (OnConfigButtonClick)
                await SaveUserData(emailValidation2);
        });

        OnDataSave.AddListener(ControlObjectActivation);
    }


    protected virtual async UniTask CheckLoginData()
    {
        if (!emailValidation.isCredentialsEntered && !emailValidation2.isCredentialsEntered)
        {
            //manager.OpenPage(7);
            emailValidation2.gameObject.SetActive(true);
            manager.AddPageHistory(emailValidation2.gameObject);

            emailValidation2.transform.Find("LoginPage").transform.Find("ButtonGroups").transform.Find("Share").GetComponent<Button>().onClick.AddListener(async () =>
            {
                OnDataSave?.Invoke();
                await SaveUserData(emailValidation2);
            });
        }
        else
        {
            if (emailValidation.isCredentialsEntered)
            {
                await SaveUserData(emailValidation);
                OnConfigButtonClick = true;
                OnDataSave?.Invoke();
            }
            else
            {
                await SaveUserData(emailValidation2);
                OnConfigButtonClick = true;
                OnDataSave?.Invoke();
            }
        }
    }

    protected virtual async UniTask SaveUserData(EmailValidation email)
    {
        Debug.Log("Get response from CMS");
        //if (!userData.storeUserData.success)
        //{
        string url = Url.apiUrl + Url.saveUserData;
        WWWForm form = new WWWForm();
        form.AddField("name", email.NameinputField.text);
        form.AddField("email", email.EmailinputField.text);
        form.AddField("marble_id", GetSelectedMarble_ID());
        form.AddField("tab_id", Tab_ID.GetID());

        //if (socketConnectionChecker != null)

        Debug.Log("Get response");
        var (Data, isSuccess) = await WWWRequestTC.Post(url, form, new HeaderDataClass[0]);
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
                await UpdateUser(messageFormat);
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

        // }
        //else
        // {
        /* Debug.Log("Get response from CMS 2");

         MessageFormat messageFormat = new MessageFormat
         {
             MessageKey = "user_id",
             MessageValue = userData.storeUserData.user_id
         };
         UpdateUser(messageFormat);*/
        //  }
    }


    protected virtual async UniTask UpdateUser(MessageFormat m)
    {
        string updateUrl = Url.apiUrl + Url.updateUserData;

        WWWForm updateForm = new WWWForm();
        updateForm.AddField("user_id", userData.storeUserData.user_id);
        updateForm.AddField("marble_id", GetSelectedMarble_ID());
        updateForm.AddField("tab_id", Tab_ID.GetID());

        Debug.Log("<color=red>Use API for Update User</color>");
        var (UpdateData, isSuccess) = await WWWRequestTC.Post(updateUrl, updateForm, new HeaderDataClass[0]);
        if (isSuccess)
        {
            Debug.Log("User updated with marble ID");
            data = JsonUtility.ToJson(m);
            //OnDataSave?.Invoke();
        }
        else
        {
            Debug.Log("<color=red>Update User false</color>");
        }

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
        return EditStringValue(s);
    }

    private string EditStringValue(StringBuilder s)
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
