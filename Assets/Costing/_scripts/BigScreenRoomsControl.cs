using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BigScreenRoomsControl : MonoBehaviour
{
    public static BigScreenRoomsControl Bigroom;
    [Space(5)]
    [Header("Big Screen rooms Image")]
    public Image BigImage;
    // Start is called before the first frame update
    [Space(5)]
    [Header("Rooms Scriptable")]
    public List<RoomType> _roomScriptableB;

    [Space(5)]
    [Header("Big tabs toogle holder")]
    public Transform _toggleButtonHolder;
    private ToggleGroup _toggleGroupHolder;
    public Transform _buttonIntabmenu; // +add room button in tabs system

    [Space(5)]
    [Header("On tab toogle Room Panels holder")]
    public Transform _roomDropHolder; // where marbles will be dropped

    [Space(5)]
    [Header("prefab of tab menu and room drop panel")]
    public GameObject TabPreafabtoggle; // tab prefab will be added on saved data from room manager
    public GameObject _roomDrop;
    private List<GameObject> InstantiatedObjectsTab = new();
    private List<GameObject> InstantiatedObjectsRoom = new();

    [Space(5)]
    [Header("Tween Drag Marbles panel")]
    public Transform DragMarblesPanel;
    // public Transform DragPanelBG; // dragging  bg panel needed for drag code
    [Space(5)]
    [Header("Drag marble image Prefab and scrollview holder")]
    // this data will come from cms selected marble list
    public GameObject DragMarblePrefab;
    public Transform DragMarbleScroller;
    [Space(5)]
    [Header("marble dimension overlay")]
    public Transform MarbleDimensionOverlay;
    public bool _isMarbleDimension; // to check for dimension validation
    public List<MarbleDetails> dumyselectedMarbleList = new();
    // current selected tabid
    public int currentTabId = 0;
    void Awake()
    {
        if (Bigroom != null)
        {
            Destroy(gameObject);
            return;
        }
        Bigroom = this;

    }
    void Start()
    {
        // assign toggle group
        _toggleGroupHolder = _toggleButtonHolder.GetComponent<ToggleGroup>();

        // create selected marble list in drag panel
        AddSelectedMarblelistToScroll();
        MarbleDimensionOverlay.gameObject.SetActive(false);
    }

    public void ShowBigRoomImages(int index)
    {
        BigImage.sprite = _roomScriptableB[index].BigRoomSprite;
        // +add room tab  and show selected room panel on with big image
        ShowSelectedRoom(index);
    }

    // add tabs menu and drop rooms box as per saved data with id
    public void AddRoomTabsFromSaveData(int index)
    {
        // create tab menu as per saved data id
        GameObject _tabbutton = Instantiate(TabPreafabtoggle, _toggleButtonHolder);
        _tabbutton.GetComponent<ToggleSpriteRooms>().myid = _roomScriptableB[index].RoomId;
        _tabbutton.GetComponent<ToggleSpriteRooms>().myname = _roomScriptableB[index].RoomName;
        // _tabbutton.GetComponent<ToggleSpriteRooms>()._isAdd = false;
        _tabbutton.GetComponent<ToggleSpriteRooms>().Tabtext.text = _roomScriptableB[index].RoomName;
        _tabbutton.GetComponent<Toggle>().group = _toggleGroupHolder;
        InstantiatedObjectsTab.Add(_tabbutton);
        // set last index for add button in tab menu system
        _buttonIntabmenu.SetAsLastSibling();

        // create drop panels as per room data added
        GameObject _roomDropPanel = Instantiate(_roomDrop, _roomDropHolder);
        _roomDropPanel.GetComponent<MarbleImageDropData>()._RoomDimensionText.text = _roomScriptableB[index].Roomdimension;
        _roomDropPanel.GetComponent<MarbleImageDropData>()._RoomId = _roomScriptableB[index].RoomId;
        _tabbutton.GetComponent<ToggleSpriteRooms>().TriggerScreen = _roomDropPanel;
        _tabbutton.GetComponent<ToggleSpriteRooms>().TriggerScreen.SetActive(false);
        InstantiatedObjectsRoom.Add(_roomDropPanel);
    }


    //remove tab and remove gameobject which is created from saved data
    public void RemoveRoomTabsFromSaveData(int index)
    {
        print(index + " in remove roomtab");
        if (RoomsManager.RoomInstance.RoomsAddedSequence.Contains(index))
        // if toogle button matches the id then remove it
        {
            int atIndex = RoomsManager.RoomInstance.RoomsAddedSequence.IndexOf(index);
            print("removing id at index " + atIndex);

            Destroy(InstantiatedObjectsTab[atIndex]);
            InstantiatedObjectsTab.RemoveAt(atIndex);

            Destroy(InstantiatedObjectsRoom[atIndex]);
            InstantiatedObjectsRoom.RemoveAt(atIndex);
        }
    }

    // In tabs menu system show selected room underlined 
    public void ShowSelectedRoom(int index)
    {
        int foundIndex = RoomsManager.RoomInstance.RoomsAddedSequence.IndexOf(index);
        print("found at index" + foundIndex);
        // print("toggle name" + _toggleButtonHolder.GetChild(foundIndex).name);
        InstantiatedObjectsTab[foundIndex].GetComponent<Toggle>().isOn = true;
    }

    // remove rooms and all data inside it
    public void RemoveRoomsFromOverlay()
    {
        //clear Marble data if added any       
        RoomsManager.RoomInstance.RemoveRoomData(currentTabId);
        _roomScriptableB[currentTabId].DropSelectedMarbles.Clear();
        RoomsManager.RoomInstance.RemoveRoomOverlay.gameObject.SetActive(false);
    }

    //====================================adding removing Marbles data for costing ==================================== 
    // On click of add Marbles selected marble list will tween in
    public void ClickonAddMarble()
    {
        DragMarblesPanel.GetComponent<RectangleTween>().RectMoveUp();
        // DragPanelBG.gameObject.SetActive(true);
    }
    // create dumy selected marble list in footer scroll
    public void AddSelectedMarblelistToScroll()
    {
        // this count will be cms data of selected marble list       
        // dumyselectedMarbleList will change to selected marble list from cms
        for (int i = 0; i < dumyselectedMarbleList.Count; i++)
        {
            GameObject DragMarbleObj = Instantiate(DragMarblePrefab, DragMarbleScroller);
            //add sprite and name ,id according to list this will change with actual list
            DragMarbleObj.GetComponent<DragMarble>().rawImage.texture = dumyselectedMarbleList[i]._Texture;
            DragMarbleObj.GetComponent<DragMarble>()._Mname.text = dumyselectedMarbleList[i].marble_name;
            DragMarbleObj.GetComponent<DragMarble>().DragMarbleid = dumyselectedMarbleList[i].id;
        }
    }

    // show marble dimension overlay on drop of marle in any room
    public void ShowMarbleDimensionOverlay(int M_id, GameObject droppedmarble)
    {
        _isMarbleDimension = true;
        MarbleDimensionOverlay.gameObject.SetActive(true);
        // MarbleDimensionOverlay.SetAsLastSibling();
        TMP_InputField Inputemp = MarbleDimensionOverlay.GetComponentInChildren<TMP_InputField>();
        Inputemp.text = "";
        Inputemp.Select();
        //make grey
        Inputemp.placeholder.GetComponent<TMP_Text>().color = Color.gray;
        Inputemp.GetComponent<DimensionTextValidation>().Marbleid = M_id;
        Inputemp.GetComponent<DimensionTextValidation>()._droppedMarble = droppedmarble;
        // move footer marbles scrolle tween down
        DragMarblesPanel.GetComponent<RectangleTween>().RectMoveDown();
    }


    // drop marble then save marble data in particular room as per id
    public void SaveMarbleDataInRoom(int marbleid, string _marbleDimension)
    {
        print("current tabid " + currentTabId);
        // check index in Room Drop objects and save marble data
        int roomIndex = RoomsManager.RoomInstance.RoomsAddedSequence.IndexOf(currentTabId);
        print("save marble in roomid " + roomIndex);

        // find index of marble id in dumyselectedmarble list got get marble data from dumy marble list
        int indexById = dumyselectedMarbleList.FindIndex(x => x.id == marbleid);
        //add matched id data in scriptable object of that room with current id
        // add marble dimension in dumy marble list then add to scriptable object of that room
        dumyselectedMarbleList[indexById].marbleDimension = _marbleDimension;
        _roomScriptableB[currentTabId].DropSelectedMarbles.Add(dumyselectedMarbleList[indexById]);
        InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().AddedMarbleSequence.Add(marbleid);
        // ===== add and create marble information text data =================
        InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().CreateMarbleTextInfo(marbleid, dumyselectedMarbleList[indexById].marble_name, dumyselectedMarbleList[indexById].PriceMarble, _marbleDimension);
        // if added marble border count is less than  5 then  cant add marble in that room else show border prefab
        int dropmarblecount = InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().DropImagePanelHolder.childCount;
        // if (dropmarblecount < 5)
        // create extra prefab for drop after each time marble is dropped
        InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().CreateDropBorderone();
        OnoffInvoiceButton();
    }
    // remove marble then remove marble data in particular room as per id
    public void RemoveMarbleDataInRoom(int marbleid)
    {
        // check index in Room Drop objects for remove marble data
        int roomIndex = RoomsManager.RoomInstance.RoomsAddedSequence.IndexOf(currentTabId);
        print("remove marble in roomid " + roomIndex);
        //to remove find index of marble id in dumyselectedmarble list
        int indexById = dumyselectedMarbleList.FindIndex(x => x.id == marbleid);
        // remove marble dimension from dumy marble list
        dumyselectedMarbleList[indexById].marbleDimension = "";
        //remove matching id data in scriptable object of that room
        _roomScriptableB[currentTabId].DropSelectedMarbles.Remove(dumyselectedMarbleList[indexById]);

        // remove marble info object in that room
        InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().RemoveMarbleInfoObject(marbleid);

        int dropmarblecount = InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().DropImagePanelHolder.childCount;
        if (dropmarblecount == 0)
        {
            // create extra prefab for drop image if all added marbles are removed and count is 0
            InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().CreateDropBorderone();
        }
        //print("ddcount " + dropmarblecount);
        OnoffInvoiceButton();
    }

    // To show or off generate invoice btn if marlbes added or removed
    public void OnoffInvoiceButton()
    {
        CostCalculate.Costcal.GenerateBtn.gameObject.SetActive(false);
        for (int k = 0; k < _roomScriptableB.Count; k++)
        {
            if (_roomScriptableB[k].DropSelectedMarbles.Count > 0)
            {
                CostCalculate.Costcal.GenerateBtn.gameObject.SetActive(true);
                break;
            }
        }

    }

    // To check if one marble is dropped in one room then same marble cannot be drop in that room 
    // bool return if true then dont drop
    public bool CheckMarbleExistsInRoom(int mid)
    {
        // check index in Room Drop objects for remove marble data
        int roomIndex = RoomsManager.RoomInstance.RoomsAddedSequence.IndexOf(currentTabId);
        bool marbleExists = InstantiatedObjectsRoom[roomIndex].GetComponent<MarbleImageDropData>().AddedMarbleSequence.Contains(mid);
        print("marble exists " + marbleExists);
        return marbleExists;
    }
}
