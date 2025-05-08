using AirFishLab.ScrollingList;
using UnityEngine;
using UnityEngine.UI;

public class GetMRMDetails : MonoBehaviour
{
    [SerializeField] StoreMarbleDetails storeMarbleDetails;
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

        if (storeMarbleDetails.loadedMarbles.Count > 0)
        {
            foreach (var marble in storeMarbleDetails.loadedMarbles)
            {
                if (marble.id.ToString() == id)
                {
                    Debug.Log("Found in loaded marbles");
                    storeMarbleDetails.ShowLoadedMarbleData(id);
                }
                else
                {
                    Debug.Log("Not Found in loaded marbles");
                    fetchQRData.LoadData(id);
                }
            }
        }
        else
        {
            Debug.Log("Not Found in loaded marbles");
            fetchQRData.LoadData(id);
        }
    }

    public void GetCurrentSelectedMarble()
    {
        var childObj = circularScrollingList.transform.GetChild(circularScrollingList.transform.childCount - 1).gameObject;
        currentSelectedMarble = childObj.GetComponent<ShowMarbleDetails>();
    }
}
