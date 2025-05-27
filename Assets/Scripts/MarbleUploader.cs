using UnityEngine;

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

public class MarbleUploader : DataTransmissionController
{
    [SerializeField] GameObject thanksForSharingObj;

    protected override void ControlObjectActivation()
    {
        if(thanksForSharingObj != null)
            thanksForSharingObj.SetActive(true);
        manager.AddPageHistory(thanksForSharingObj);
        loginPanel.SetActive(false);
    }

    protected override void UpdateUser(MessageFormat m)
    {
        base.UpdateUser(m);
        Pdf_Generate(m.MessageValue);
    }

    protected override void SaveUserData(EmailValidation email)
    {
        base.SaveUserData(email);
        Invoke(nameof(Pdf_apiCall),1f);
    }

    private void Pdf_apiCall()
    {
        if (!userData.storeUserData.already_register)
        {
            Pdf_Generate(userData.storeUserData.user_id);
        }
    }

    private void Pdf_Generate(string user_id)
    {
        string pdfUrl = Url.apiUrl + Url.pdf;

        WWWForm form = new WWWForm();
        form.AddField("user_id", user_id);

        requestTC.Post(form, pdfUrl, (formData, isSuccess) =>
        {
            if (isSuccess)
                Debug.Log("PDF Generated");
        });
    }
}
