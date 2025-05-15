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

        //if (currentSelectedMarble.texture == null)
        //{
        //    //string id = currentSelectedMarble.tileID.ToString();
        //    fetchQRData.LoadData(id);
        //}
        
        /*if (storeMarbleDetails.loadedMarbles.Count > 0)
        {
            foreach (var marble in storeMarbleDetails.loadedMarbles)
            {
                //Debug.Log("Curr ID: " + id + " =>>>> " + marble.id.ToString());
                if (marble.id.ToString() == id)
                {
                    Debug.Log("Found in loaded marbles");
                    storeMarbleDetails.ShowLoadedMarbleData(id);
                    return;
                }
            }
            Debug.Log("Not Found in loaded marbles");
            fetchQRData.LoadData(id);
        }
        else
        {
            Debug.Log("Not Fount for the first time0");
            fetchQRData.LoadData(id);
        }*/
    }

    public void GetCurrentSelectedMarble()
    {
        var childObj = circularScrollingList.transform.GetChild(circularScrollingList.transform.childCount - 1).gameObject;
        currentSelectedMarble = childObj.GetComponent<ShowMarbleDetails>();

        marbleNameText.text = currentSelectedMarble.marbleName;
    }
}
