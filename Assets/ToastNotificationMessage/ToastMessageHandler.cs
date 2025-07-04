using UnityEngine;

public class ToastMessageHandler: MonoBehaviour
{
    public void ShowMsg()
    {
        //ToastNotification.messageScreenPosition = MessageScreenPosition.BottomCenter;
        ToastNotification.Show("Invalid QR code!");
    }
}
