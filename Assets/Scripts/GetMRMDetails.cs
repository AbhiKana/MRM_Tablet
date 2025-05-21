using AirFishLab.ScrollingList;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetMRMDetails : MonoBehaviour
{
    public StoreMarbleDetails storeMarbleDetails;
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] FetchQRData fetchQRData;
    [SerializeField] Button viewMrmButton;
    [SerializeField] TextMeshProUGUI marbleNameText;
    
    [SerializeField] ShowMarbleDetails currentSelectedMarble;

    private void Start()
    {
        viewMrmButton.onClick.AddListener(()=>
        {
            string id = currentSelectedMarble.tileID.ToString();
            //ViewMarbleDetails(id);
            ViewMarbleDetails();
        });
    }

    void ViewMarbleDetails()
    {
        currentSelectedMarble.GetComponent<MarbleItemController>().OnDetailsButtonClicked();
    }

    public void ViewMarbleDetails(string id)
    {
        fetchQRData.LoadData(id);
    }

    public void LoadMarbleDetails(string id)
    {
        //LoadMarbleImageDataBG();
    }

    public void GetCurrentSelectedMarble()
    {
        var childObj = circularScrollingList.transform.GetChild(circularScrollingList.transform.childCount - 1).gameObject;
        currentSelectedMarble = childObj.GetComponent<ShowMarbleDetails>();

        marbleNameText.text = currentSelectedMarble.marbleName;
    }
}
