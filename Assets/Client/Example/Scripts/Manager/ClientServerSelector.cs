using UnityEngine;

public class ClientServerSelector : MainManager
{
    private void OnOffObjects(GameObject type, GameObject panel)
    {
        Debug.Log("Object on off");
        if (panel != null && type != null)
        {
            panel.SetActive(false);
            type.SetActive(true);
        }
    }

    public override void ProcessStart()
    {
        base.ProcessStart();
    }

    protected override void ClientSelected()
    {
        base.ClientSelected();
    }

    protected override void ServerSelected()
    {
        base.ServerSelected();
    }

    public GameObject GetServerController()
    {
        return ServerController;
    }

    public GameObject GetClientController()
    {
        return ClientController;
    }
}
