using UnityEngine;
using UnityEngine.UI;

public class ToggleSpriteChange : MonoBehaviour
{
    public Toggle toggle;
    public Image image;
    //public Image GlowImage;
    public Sprite Selectedsprite, UnselectedSprite;
    public GameObject TriggerScreen, CurrentScreen;
    public int myid;
    public string myname;
   
    public bool getImage;

    // Start is called before the first frame update
    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        if (getImage)
            image = GetComponent<Image>();
    }

    void Start()
    {
       
    }

    public void OnToggleClicked()
    {
        //Debug.Log("Check on clicks");
        if (image) image.sprite = toggle.isOn ? Selectedsprite : UnselectedSprite;

        if (TriggerScreen != null)
        {
            TriggerScreen.SetActive(toggle.isOn);
        }

        if (CurrentScreen != null)
        {
            CurrentScreen.SetActive(!toggle.isOn);
        }

        /*if (toggle.isOn)
            GlowImage.gameObject.SetActive(true);
        else
            GlowImage.gameObject.SetActive(false);*/
    }

}
