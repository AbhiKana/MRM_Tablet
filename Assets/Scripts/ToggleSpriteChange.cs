using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleSpriteChange : MonoBehaviour
{
    public Toggle toggle;
    public bool getImage;
    public Image image;
    public Image GlowImage;
    public Sprite Selectedsprite, UnselectedSprite;
    public GameObject TriggerScreen;
    public int myid;
    public string myname;
   

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

        if (TriggerScreen)
        {
            TriggerScreen.SetActive(toggle.isOn);

        }

        if (toggle.isOn)
            GlowImage.gameObject.SetActive(true);
        else
            GlowImage.gameObject.SetActive(false);
    }

}
