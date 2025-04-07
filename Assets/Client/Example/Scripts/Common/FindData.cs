using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindData : MonoBehaviour
{
	//public DataSender data;

	ClientServerSelector clientServerSelector;

	public void OnButtonDown(string vidName)
	{
		if (clientServerSelector == null)
			clientServerSelector = FindFirstObjectByType<ClientServerSelector>();

        clientServerSelector.GetClientController().GetComponent<TCP_ClientController>().SendMessage(vidName);
        //data.OnPressed(vidName);
    }

}
