using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomsManager : MonoBehaviour
{
    [Space(5)]
    [Header("Main screen costing panels 3")]
    public Transform[] CostingPanels;

    public static RoomsManager RoomInstance;
    public int CurrentRoomID;
    public string CurrentRoomName;
    public string CurrentDimension;

    [Space(5)]
    [Header("4 Rooms Scriptable objects")]
    public GameObject[] RoomScriptable;


    [Space(5)]
    [Header("Overlay panel 2 dimension headline and Inputtext")]
    // for over lay dimension panel text headline 
    public TextMeshProUGUI HeadLineDimension;
    public TMP_InputField OverlayInput;

    [Space(5)]
    [Header("Adding rooms sequence use for big screen tabs")]
    public List<int> RoomsAddedSequence;

    [Space(5)]
    [Header("next button")]
    public Transform NextButton;
    public Sprite _Nextsprite, _Closebtnsprite;
    // boolean for coming from bigscreen panel
    public bool FromBigScreen;

    [Space(5)]
    [Header("For first screen show bg layout and transform scale down option")]
    public Transform BlackBglayoutForPanel1;
    public Transform MainCostingPanel;

    [Header("For remove room overlay")]
    public TextMeshProUGUI RemoveHeadline;
    public Transform RemoveRoomOverlay;
    void Awake()
    {
        if (RoomInstance != null)
        {
            Destroy(gameObject);
            return;
        }
        RoomInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        OnOffCostingPanels(0);
        NextButton.gameObject.SetActive(false);
        BlackBglayoutForPanel1.gameObject.SetActive(false);
        RemoveRoomOverlay.gameObject.SetActive(false);
    }

    // on click of next button
    public void OnClickNextButton()
    {
        OnOffCostingPanels(2);
        SendbacktoNormal();
        print("CurrentID in next " + CurrentRoomID);
        BigScreenRoomsControl.Bigroom.ShowBigRoomImages(CurrentRoomID);
    }


    // save room data to scriptable object on click of continue btn
    // index to check which costing panel to show
    public void SaveRoomData(int index)
    {
        // current id is current selected room id
        // on dimension object of selected room as per id
        RoomScriptable[CurrentRoomID].GetComponent<SelectRoomControl>().DimensionObject.gameObject.SetActive(true);
        RoomScriptable[CurrentRoomID].GetComponent<SelectRoomControl>().DimensionObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = CurrentDimension;

        //RoomScriptable[CurrentRoomID].GetComponent<SelectRoomControl>()._roomScriptable.RoomId = CurrentRoomID;
        RoomScriptable[CurrentRoomID].GetComponent<SelectRoomControl>()._roomScriptable.Roomdimension = CurrentDimension;

        // add id in room sequence
        RoomsAddedSequence.Add(CurrentRoomID);
        //on next button
        NextButton.gameObject.SetActive(true);
        // add tabs in big screen tabs system
        BigScreenRoomsControl.Bigroom.AddRoomTabsFromSaveData(CurrentRoomID);
        CostingPanels[1].gameObject.SetActive(false);
        // if continue then show 3rd page
        // OnOffCostingPanels(index);
        // index for which panel to go if close then go back firstcostrooms
        //if index is 2 then call bigscreen functions
        //if (index == 2)
        //{
        //    SendbacktoNormal();
        //    print("CurrentID " + CurrentRoomID);
        //    BigScreenRoomsControl.Bigroom.ShowBigRoomImages(CurrentRoomID);
        //}

    }

    //On click of close room dimension overalay =============
    public void CloseRoomData()
    {
        RoomScriptable[CurrentRoomID].GetComponent<SelectRoomControl>()._toggle.isOn = false;
        CostingPanels[1].gameObject.SetActive(false);
    }

    // click on add data in selection room so show overlay panel and then save data later on click of continue button
    public void AddRoomData(int id, string name)
    {
        CurrentRoomID = id;
        CurrentRoomName = name;
        HeadLineDimension.text = CurrentRoomName + " Dimensions";
        OverlayInput.text = "";
      //  OverlayInput.placeholder.GetComponent<TMP_Text>().color = Color.gray;
        OverlayInput.Select();
    }

    // click on remove button in room selection so remove room data
    public void RemoveRoomData(int id)
    {
        // if rooms added then only remove
        if (RoomsAddedSequence.Count > 0)
        {   //CurrentRoomName = null;
            RoomScriptable[id].GetComponent<SelectRoomControl>().DimensionObject.gameObject.SetActive(false);
            RoomScriptable[id].GetComponent<SelectRoomControl>().DimensionObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
            // remove dimesion data from scriptable objet
            RoomScriptable[id].GetComponent<SelectRoomControl>()._roomScriptable.Roomdimension = "";
            RoomScriptable[id].GetComponent<SelectRoomControl>()._toggle.isOn = false;
            // remove data from big screen tabs and make tab on addroom sprite
            BigScreenRoomsControl.Bigroom.RemoveRoomTabsFromSaveData(id);
            // remove id in room sequence
            RoomsAddedSequence.Remove(id);

            // off next button
            if (RoomsAddedSequence.Count == 0)
            {
                NextButton.gameObject.SetActive(false);
                // if all rooms removed in overlay then goto start screen
                if (FromBigScreen)
                {
                    SendbacktoNormal();
                    OnOffCostingPanels(0);
                }
            }
            else
            { // if all data removes the by default id will be in sequence id
                CurrentRoomID = RoomsAddedSequence[0];
                print("after remove current roomId" + CurrentRoomID);
            }
        }
        BigScreenRoomsControl.Bigroom.OnoffInvoiceButton();
    }

    //on off main 3 pages(panels)
    public void OnOffCostingPanels(int index)
    {
        foreach (var group in CostingPanels)
        {
            group.gameObject.SetActive(false);
        }
        CostingPanels[index].gameObject.SetActive(true);
    }

    // send back first panel sibling index to start position and off bg black layout
    private void SendbacktoNormal()
    {
        // if came from bigscreen then set sibling index back to first position
        if (FromBigScreen)
        {
            //came from bigscreen
            CostingPanels[0].SetSiblingIndex(1);
            CostingPanels[1].SetSiblingIndex(2);
            CostingPanels[2].SetSiblingIndex(3);
            //scale down first cost panel
            MainCostingPanel.localScale = new Vector3(1f, 1f, 1f);
            BlackBglayoutForPanel1.gameObject.SetActive(false);
            NextButton.GetComponent<Image>().sprite = _Nextsprite;
            FromBigScreen = false;
        }
    }
    // show add room panel on black overlay after clicking from tab menu +add room
    public void ShowAddRoomPanel()
    {
        //change cost panel index and black layout index to top       
        CostingPanels[2].SetSiblingIndex(1);
        CostingPanels[0].SetSiblingIndex(2);
        CostingPanels[1].SetSiblingIndex(3);
        //show black bg layout for first cost panel with scale down first costrooms object
        MainCostingPanel.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        BlackBglayoutForPanel1.gameObject.SetActive(true);
        FromBigScreen = true;
        // change next button sprite to close button
        NextButton.GetComponent<Image>().sprite = _Closebtnsprite;
        CostingPanels[0].gameObject.SetActive(true);
    }

    // show remove room overlay on click of tab menu X selelcted room
    public void ShowRemoveOverlay(int id, string roomname)
    {
        FromBigScreen = true;
        RemoveRoomOverlay.gameObject.SetActive(true);
        RemoveHeadline.text = "Remove " + roomname + "!";
        print("currenttabid " + BigScreenRoomsControl.Bigroom.currentTabId);
    }

}
