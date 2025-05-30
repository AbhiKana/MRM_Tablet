using UnityEngine;
using UnityEngine.UI;

public class MessageReader : MonoBehaviour
{
    [SerializeField] Image a, b;

    private void Start()
    {
        TCP_ClientController.OnMessageReceived += ProcessMessage;
    }

    public void ProcessMessage(string revmsg)
    {
        //Debug.Log("Process receive msg: " + revmsg);
        switch (revmsg)
        {
            case "a":
                a.color = Color.white;
                break;
            case "b":
                b.color = Color.black;
                break;
        }
    }
}
