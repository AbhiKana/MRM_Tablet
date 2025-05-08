using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleSprite : MonoBehaviour
{
    public Toggle toggle;
    public bool getImage;
    public Image image;
    public Sprite Selectedsprite, UnselectedSprite;
    public GameObject TriggerScreen;
    public int myid;
    public string myname;
    public SelectRoomControl _selectroomControl;


    // Start is called before the first frame update
    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        if (getImage)
            image = GetComponent<Image>();
    }

    void Start()
    {
        myid = _selectroomControl._roomScriptable.RoomId;
        myname = _selectroomControl._roomScriptable.RoomName;
    }

    public void OnToggleClicked()
    {
        if (image) image.sprite = toggle.isOn ? Selectedsprite : UnselectedSprite;

        if (TriggerScreen)
        {
            TriggerScreen.SetActive(toggle.isOn);
        }

        // if toogle is on then add room data else Remove room data
        if (toggle.isOn)
            RoomsManager.RoomInstance.AddRoomData(myid, myname);
        else
            RoomsManager.RoomInstance.RemoveRoomData(myid);


    }

}
