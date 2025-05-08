using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRoomControl : MonoBehaviour
{
    public RoomType _roomScriptable;
    public Transform DimensionObject;
    public Toggle _toggle;
    // Start is called before the first frame update

    private void Start()
    {
        // off Dimensions objects in each room at start
        DimensionObject.gameObject.SetActive(false);
    }
}
