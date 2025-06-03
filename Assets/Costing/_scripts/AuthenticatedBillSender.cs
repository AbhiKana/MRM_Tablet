using Udar.SceneManager;
using UnityEngine;

public class AuthenticatedBillSender : DataTransmissionController
{

    [Header("Authentication Propertites")]
    [SerializeField] private GameObject marbleSelectionPrompt;

    [SerializeField] SceneSwitchManager sceneSwitchManager;
    [SerializeField] SceneFieldRef sceneField;
    [SerializeField] StoreMarbleDetails storeMarbleDetails;

/*    private void Start()
    {
        DataSenderButton.onClick.AddListener(CheckIfUserLoggedIn);
    }
    public void CheckIfUserLoggedIn()
    {
        if (storeMarbleDetails.list.Count == 0)
            marbleSelectionPrompt.SetActive(true);
        //else
            //sceneSwitchManager.LoadScene(sceneField);
    }*/
    protected override void CheckLoginData()
    {
        if (storeMarbleDetails.list.Count == 0)
            marbleSelectionPrompt.SetActive(true);
        else
            base.CheckLoginData();
    }

    protected override void ControlObjectActivation()
    {
        if (storeMarbleDetails.list.Count == 0)
            marbleSelectionPrompt.SetActive(true);
        else
            sceneSwitchManager.LoadScene(sceneField);
    }
}
