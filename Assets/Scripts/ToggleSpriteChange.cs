using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSpriteChange : MonoBehaviour
{
    public Toggle toggle;
    public Image image;
    public Sprite Selectedsprite, UnselectedSprite;
    public GameObject TriggerScreen, CurrentScreen;

    public TextMeshProUGUI textMesh;
   
    public bool getImage;
    public bool IsTextEdit;

    // Start is called before the first frame update
    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        if (getImage)
            image = GetComponent<Image>();
    }

    public void OnToggleClicked()
    {
        //Debug.Log("Check on clicks");
        if (image) image.sprite = toggle.isOn ? Selectedsprite : UnselectedSprite;
        SetScreenStatus();
        ChnageTextColor();
    }

    private void SetScreenStatus()
    {
        if (TriggerScreen != null)
            TriggerScreen.SetActive(toggle.isOn);

        if (CurrentScreen != null)
            CurrentScreen.SetActive(!toggle.isOn);
    }

    public void ChnageTextColor()
    {
        if(IsTextEdit)
        {
            textMesh.color = toggle.isOn? Color.white: Color.black;
        }
    }
}
