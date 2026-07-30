using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;


public class MarbleUploader : DataTransmissionController
{
    [SerializeField] GameObject thanksForSharingObj;

    [Header("Scriptable Objects")]
    [SerializeField] MessageSenderDetails user_id_object;
    protected override void ControlObjectActivation()
    {
        Debug.Log("<color=green>OnDataSave Invoke</color>");
        if(thanksForSharingObj != null)
            thanksForSharingObj.SetActive(true);
        manager.AddPageHistory(thanksForSharingObj);
        emailValidation2.gameObject.SetActive(false);
    }

    protected override async UniTask UpdateUser(MessageFormat m)
    {
        await base.UpdateUser(m);
        Invoke(nameof(Pdf_apiCall), 1f);
        //Pdf_Generate(m.MessageValue);
    }

    protected override async UniTask SaveUserData(EmailValidation email)
    {
        await base.SaveUserData(email);
        Invoke(nameof(Pdf_apiCall),1f);
    }

    private void Pdf_apiCall()
    {
        if (!userData.storeUserData.already_register)
        {
            Pdf_Generate(userData.storeUserData.user_id).Forget();
        }
        else
        {
            //Updated user
            Pdf_Generate(userData.storeUserData.user_id).Forget();
        }
    }

    private async UniTask Pdf_Generate(string user_id)
    {
        string pdfUrl = Url.apiUrl + Url.pdf;

        WWWForm form = new WWWForm();
        form.AddField(user_id_object.nameVal, user_id);

        WWWRequestTC w = new WWWRequestTC();
        await w.Post(pdfUrl, form, new HeaderDataClass[0], (formData, isSuccess) =>
        {
            if (isSuccess)
                Debug.Log("PDF Generated");
        });
    }
}
