using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleSpriteRooms : MonoBehaviour
{
    public Toggle toggle;
    public bool getImage;
    public Image image;
    public Sprite Selectedsprite, UnselectedSprite;
    public GameObject TriggerScreen;
    public int myid;
    public string myname;
    public TextMeshProUGUI Tabtext;
    public GameObject CrossButton;

    // Start is called before the first frame update
    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        if (getImage)
            image = GetComponent<Image>();
    }

    void Start()
    {
        //Tabtext.text = "+ Add Room";
    }

    public void OnToggleClicked()
    {
        // only after add room           
        if (image) image.sprite = toggle.isOn ? Selectedsprite : UnselectedSprite;
        // on off remove button
        CrossButton.SetActive(toggle.isOn);
        //print(">> "+TriggerScreen.name);
        // print("toggle " + toggle.isOn);
        if (TriggerScreen != null)
        {
            TriggerScreen.SetActive(toggle.isOn);
            // change big image of room
            BigScreenRoomsControl.Bigroom.BigImage.sprite = BigScreenRoomsControl.Bigroom._roomScriptableB[myid].BigRoomSprite;
            BigScreenRoomsControl.Bigroom.currentTabId = myid;
        }
    }

    // to remove room
    public void OnCrossBtnClicked()
    {
        RoomsManager.RoomInstance.ShowRemoveOverlay(myid, myname);
    }
}
