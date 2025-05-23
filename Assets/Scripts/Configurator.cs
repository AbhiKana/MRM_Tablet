using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Configurator : DataTransmissionController
{
    [SerializeField] ConnectViaInput connectViaInput;
    [SerializeField] GameObject endSession;

    protected override void CheckLoginData()
    {
        if (!connectViaInput.IsConnectedToServer)
            connectViaInput.EnableInputField(true);
        else
            base.CheckLoginData();
    }

    protected override void ControlObjectActivation()
    {
        endSession.SetActive(true);
        manager.AddPageHistory(endSession);
        loginPanel.SetActive(false);
    }
}
