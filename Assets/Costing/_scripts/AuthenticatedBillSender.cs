using Udar.SceneManager;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticatedBillSender : DataTransmissionController
{

    [Header("Authentication Propertites")]
    [SerializeField] private GameObject marbleSelectionPrompt;

    [SerializeField] SceneSwitchManager sceneSwitchManager;
    [SerializeField] SceneFieldRef sceneField;
    [SerializeField] StoreMarbleDetails storeMarbleDetails;
    public void CheckIfUserLoggedIn()
    {
        //if (!string.IsNullOrEmpty(userData.storeUserData.user_id))
        //{
        //    Debug.Log("Send Invoice Bill");
        //    InvoiceManager._invoiceMananger.MarbleInvoiceData.user_id = int.Parse(userData.storeUserData.user_id);
        //    InvoiceManager._invoiceMananger.OnclickOfShareInvoice();
        //}
        //else
        //{
        //    canvas.sortingOrder = 1;
        //    CheckLoginData();
        //    marbleUploader.DataSenderButton.onClick.Invoke();
        //}
    }

    protected override void ControlObjectActivation()
    {
        if (storeMarbleDetails.list.Count == 0)
            marbleSelectionPrompt.SetActive(true);
        else
            sceneSwitchManager.LoadScene(sceneField);
    }
}
