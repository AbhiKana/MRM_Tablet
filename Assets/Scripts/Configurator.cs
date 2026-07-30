using Cysharp.Threading.Tasks;
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
    [SerializeField] GameObject TabDisconnected;
    private void OnEnable()
    {
        TCP_ClientController.OnServerDisconnected += ResetConfig;
    }

    private void OnDestroy()
    {
        TCP_ClientController.OnServerDisconnected -= ResetConfig;
    }

    protected override async UniTask CheckLoginData()
    {
        if (!connectViaInput.IsConnectedToServer)
        {
            connectViaInput.EnableInputField(true);
            connectViaInput.panel.GetComponentInChildren<Button>().onClick.AddListener(GetCheckLoginData);
        }
        else
           await base.CheckLoginData();
    }

    private async void GetCheckLoginData()
    {
        Debug.Log("Check Login");
        await Task.Delay(100);

        if (connectViaInput.IsConnectedToServer)
        {
            Debug.Log("Check Client connected");            
            await base.CheckLoginData();
        }
    }

    protected override void ControlObjectActivation()
    {
        manager.OpenPage(8);
        emailValidation2.gameObject.SetActive(false);
    }

    public void OnTabAccepted()
    {
        endSessionButton.SetActive(true);
        waitObjectPanel.SetActive(false);
        TabDisconnected.gameObject.SetActive(false);
    }

    public void ResetConfig()
    {
        waitObjectPanel.SetActive(true);
        endSessionButton.SetActive(false);
        TabDisconnected.gameObject.SetActive(false);
    }

    public void TabDisconnectThroughConfig()
    {
        TabDisconnected.gameObject.SetActive(true);
        endSessionButton.SetActive(false);
        waitObjectPanel.SetActive(false);
    }
}
