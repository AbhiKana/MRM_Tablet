using System;
using AirFishLab.ScrollingList;
using UnityEngine;
using UnityEngine.UI;

public class GetMRMDetails : MonoBehaviour
{
    [SerializeField] InfiniteScrollerAdjuster adjuster;
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] FetchQRData fetchQRData;
    [SerializeField] Button viewMrmButton;
    
    ShowMarbleDetails currentSelectedMarble;

    private void Start()
    {
        viewMrmButton.onClick.AddListener(ViewMarbleDetails);
    }

    private void ViewMarbleDetails()
    {
        string id = currentSelectedMarble.tileID.ToString();
        fetchQRData.LoadData(id);
    }

    public void GetCurrentSelectedMarble()
    {
        currentSelectedMarble = circularScrollingList.transform.GetChild(circularScrollingList.transform.childCount - 1).gameObject.GetComponent<ShowMarbleDetails>();
    }


}
