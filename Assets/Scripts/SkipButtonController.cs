using UnityEngine;

public class SkipButtonController : MonoBehaviour
{
    [SerializeField] UI_Manager manager;
    [SerializeField] ConnectViaInput connectViaInput;

    [SerializeField] bool IsConfigClick;
    
    public void OnSkipClicked()
    {
        if (IsConfigClick)
        {
            connectViaInput.EnableInputField(false);
        }
        else
        {
            connectViaInput.EnableInputField(false);
            manager.OpenPage(0);
        }
    }
    public void SetConfigClick(bool val)
    {
        IsConfigClick = val;
    }
}
