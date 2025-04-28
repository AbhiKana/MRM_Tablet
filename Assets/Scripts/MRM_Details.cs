using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MRM_Details : MonoBehaviour
{
    public TextMeshProUGUI MarbleName, MarbleDescription;
    public TextMeshProUGUI MarbleDimension, MarbleMaterial, MarbleFinish;
    public RawImage CircleImage, TopMarbleImage;
    public RawImage[] BgImages;

    public Toggle isSelected;

    StoreMarbleDetails storeMarbleDetails;

    private void Start()
    {
        storeMarbleDetails = FindObjectOfType<StoreMarbleDetails>();
        isSelected.onValueChanged.AddListener(RemoveFromList);
    }
    public void RemoveFromList(bool val)
    {
        if (!val)
        {
            RemoveFromEverywhere();
        }
    }

    public void RemoveFromEverywhere()
    {
        foreach(SpecificMarbleDetails details in storeMarbleDetails.WishListMarble)
        {
            Debug.Log(details.marble_name + " " + MarbleName.ToString());
            if(details.marble_name == MarbleName.ToString())
            {
                Debug.Log(details.marble_name + " DETAILS");
                storeMarbleDetails.list.Remove(details);
            }
        }

        Destroy(this.gameObject);
    }
}
