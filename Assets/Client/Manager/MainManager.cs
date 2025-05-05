using UnityEngine;

public class MainManager : MonoBehaviour
{
    public enum SelectType { Server, Client };
    public SelectType selectedType;

    protected GameObject ServerController;
    protected GameObject ClientController;

    [Space(10)]
    [SerializeField] bool IsAutoStart;

    public void SetAutoStart(bool autoStart)
    {
        IsAutoStart = autoStart;
    }
    private void Start()
    {
        ProcessStart();
    }

    public virtual void ProcessStart()
    {
        if(IsAutoStart)
            GetSelectType();
    }


    public SelectType GetSelectType()
    {
        Debug.Log("Get sleceted type: "+ selectedType.ToString());
        switch (selectedType)
        {
            case SelectType.Server:
                Debug.Log("Server selected");
                ServerSelected();
                break;
            case SelectType.Client:
                Debug.Log("Client selected");
                ClientSelected();
                break;
        }
        return selectedType;
    }

    protected virtual void ServerSelected()
    {
        SetUpServer();
    }

    protected virtual void ClientSelected()
    {
        SetUpClient();      
    }

    private void SetUpServer()
    {
        Debug.Log("Set up server");
        ServerController = new GameObject("ServerController");
        ServerController.transform.SetParent(transform);
        ServerController.AddComponent<TCP_ServerController>();
        ServerController.AddComponent<ClientStatus_ServerSide>();
        ServerController.GetComponent<TCP_ServerController>()._Initialize(); 
    }

    private void SetUpClient()
    {
        Debug.Log("Set up client");
        GameObject temp = GameObject.Find("ClientController");
        if (temp == null)
        {
            ClientController = new GameObject("ClientController");
            ClientController.transform.SetParent(transform);
            ClientController.AddComponent<TCP_ClientController>();
            ClientController.AddComponent<ClientStatus_ClientSide>();
            ClientController.GetComponent<TCP_ClientController>()._Initialze(); 
        }
    }

    public void DestroyClientObject()
    {
        Destroy(ClientController);
    }

    public ClientStatus_ServerSide Get_ServerSide_ClientStatus()
    {
        return ServerController.GetComponent<ClientStatus_ServerSide>();
    }
    public ClientStatus_ClientSide Get_ClientSide_ClientStatus()
    {
        return ServerController.GetComponent<ClientStatus_ClientSide>();
    }
}
