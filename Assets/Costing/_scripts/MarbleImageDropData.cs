using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MarbleImageDropData : MonoBehaviour
{
    // show the room dimensions saved in scriptable object
    public TextMeshProUGUI _RoomDimensionText;
    public int _RoomId;
    public GameObject DropBorderPrefab;
    public Transform DropImagePanelHolder;   
    public List<int> AddedMarbleSequence = new();
    public GameObject MarbleTextData;
    public Transform MarbleTextDataHolder;

    // Start is called before the first frame update
    void Start()
    {
        CreateDropBorderone();
    }

    // create drop border one at a time
    public void CreateDropBorderone()
    {
         Instantiate(DropBorderPrefab, DropImagePanelHolder);
    }

    // create marble info line on each marble drop in a Room and store info
    public void CreateMarbleTextInfo(int mid,string mname,string mprice,string mdimension)
    {
        GameObject _marbleInfo = Instantiate(MarbleTextData, MarbleTextDataHolder);
        _marbleInfo.GetComponent<MarbleInformation>().Marbleinfoid = mid;
        _marbleInfo.GetComponent<MarbleInformation>().MarbleNameInfo.text = mname;
        _marbleInfo.GetComponent<MarbleInformation>().MarblePriceInfo.text = mprice + " sq/ft";
        _marbleInfo.GetComponent<MarbleInformation>().MarbleDimensionInfo.text = mdimension;
        float priceFloat = float.Parse(mprice);
        int dprice = CostCalculate.Costcal.CalculatePrice(priceFloat, mdimension);
        _marbleInfo.GetComponent<MarbleInformation>().MarbleAmountInfo.text = "Rs."+dprice;

    }

    // remove marble info data
    public void RemoveMarbleInfoObject(int marbleid)
    {
        if (AddedMarbleSequence.Count > 0)
        {
            int indexToremove = AddedMarbleSequence.IndexOf(marbleid);
            print("indextoremove" + indexToremove);
            if (indexToremove >= 0)
            {
                Destroy(MarbleTextDataHolder.GetChild(indexToremove).gameObject);
            }
            // remove from addedmarble sequence
            AddedMarbleSequence.Remove(marbleid);
        }
    }
 
}
