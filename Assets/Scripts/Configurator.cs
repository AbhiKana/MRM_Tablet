using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Configurator : DataTransmissionController
{

    [Header("Child Properties")]
    [SerializeField] ConnectViaInput connectViaInput;
    [SerializeField] GameObject endSessionParent;
    [SerializeField] GameObject waitObjectPanel;
    [SerializeField] GameObject endSessionButton;

    protected override void CheckLoginData()
    {
        if (!connectViaInput.IsConnectedToServer)
        {
            connectViaInput.EnableInputField(true);
            connectViaInput.panel.GetComponentInChildren<Button>().onClick.AddListener(GetCheckLoginData);
        }
        else
            base.CheckLoginData();
    }

    private async void GetCheckLoginData()
    {
        Debug.Log("Check Login");
        await Task.Delay(100);

        if (connectViaInput.IsConnectedToServer)
        {
            Debug.Log("Check Client connected");            
            base.CheckLoginData();
        }
    }

    protected override void ControlObjectActivation()
    {
        manager.OpenPage(8);
        loginPanel.SetActive(false);
    }

    public void OnTabAccepted()
    {
        waitObjectPanel.SetActive(false);
        endSessionButton.SetActive(true);
    }
}
