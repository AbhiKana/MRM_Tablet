using TMPro;
using UnityEngine;

public class NetworkReachability_Check : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    string m_ReachabilityText;

    void Update()
    {
        //Output the network reachability to the console window
        //Debug.Log("Internet : " + m_ReachabilityText);
        //Check if the device cannot reach the internet
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Change the Text
            m_ReachabilityText = "Not Reachable.";
            textMeshProUGUI.text = m_ReachabilityText;
        }
        //Check if the device can reach the internet via a carrier data network
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            m_ReachabilityText = "Reachable via carrier data network.";
            textMeshProUGUI.text = m_ReachabilityText;
        }
        //Check if the device can reach the internet via a LAN
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            m_ReachabilityText = "Reachable via Local Area Network.";
            textMeshProUGUI.text = m_ReachabilityText;
        }
    }
}
