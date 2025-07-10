using Udar.SceneManager;
using UnityEngine;

public class AuthenticatedBillSender : DataTransmissionController
{
    [Header("Authentication Propertites")]
    [SerializeField] private GameObject marbleSelectionPrompt;

    [SerializeField] SceneSwitchManager sceneSwitchManager;
    [SerializeField] SceneFieldRef sceneField;
    [SerializeField] StoreMarbleDetails storeMarbleDetails;
    [SerializeField] ConnectionStateManager connectionStateManager;
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
        {
            marbleSelectionPrompt.SetActive(true);
        }
        else
        {
            CloseTabWhileWaiting();
            sceneSwitchManager.LoadScene(sceneField);
        }
    }

    private void CloseTabWhileWaiting()
    {
        if (connectionStateManager != null)
        {
            if (connectionStateManager.CurrentState == ConnectionState.WaitingForConfigurator)
            {
                var socketChecker = FindObjectOfType<SocketConnectionChecker>();
                if (socketChecker != null)
                {
                    socketChecker.SendCloseTabID();
                }
            }
        }
    }
}
