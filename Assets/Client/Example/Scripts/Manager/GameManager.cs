using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] ClientServerSelector clientServerSelector;

    public ClientServerSelector cs => clientServerSelector; 
    public void Initialize()
    {
        //clientServerSelector.ProcessStart();
        ServerClientMessageType();
    }

    void ServerClientMessageType()
    {
        /*if (canvasController.selectedType == MainManager.SelectType.Server)
        {
            messageSpawn.enabled = false;
            messageLoader.Initialize();
        }

        if (canvasController.selectedType == MainManager.SelectType.Client)
        {
            messageLoader.enabled = false;
            messageSpawn.Initialize();
        }*/
    }
}
