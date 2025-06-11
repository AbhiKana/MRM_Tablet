using UnityEngine;


public class MarbleUploader : DataTransmissionController
{
    [SerializeField] GameObject thanksForSharingObj;

    protected override void ControlObjectActivation()
    {
        Debug.Log("<color=green>OnDataSave Invoke</color>");
        if(thanksForSharingObj != null)
            thanksForSharingObj.SetActive(true);
        manager.AddPageHistory(thanksForSharingObj);
        loginPanel.SetActive(false);
    }

    protected override void UpdateUser(MessageFormat m)
    {
        base.UpdateUser(m);
        Invoke(nameof(Pdf_apiCall), 1f);
        //Pdf_Generate(m.MessageValue);
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
        else
        {
            //Updated user
            Pdf_Generate(userData.storeUserData.user_id);
        }
    }

    private void Pdf_Generate(string user_id)
    {
        string pdfUrl = Url.apiUrl + Url.pdf;

        WWWForm form = new WWWForm();
        form.AddField("user_id", user_id);

        WWWRequestTC w = new WWWRequestTC();
        w.Post(form, pdfUrl, (formData, isSuccess) =>
        {
            if (isSuccess)
                Debug.Log("PDF Generated");
        });
    }
}
