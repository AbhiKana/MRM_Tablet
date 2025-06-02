using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MRM_Details : MonoBehaviour
{
    TileDetailsList tileDetailsList;

    public TextMeshProUGUI MarbleName, MarbleDescription;
    public TextMeshProUGUI MarbleDimension, MarbleMaterial, MarbleFinish, MarbleAvailability, MarblePrice;
    public RawImage CircleImage, TopMarbleImage;
    public RawImage[] BgImages;
    public Toggle isSelected;

    private void Start()
    {
        tileDetailsList = FindObjectOfType<TileDetailsList>();

        isSelected.onValueChanged.AddListener(RemoveFromList);
    }
    public void RemoveFromList(bool val)
    {
        if (!val)
        {
            UpdatePage();
            MarbleManager.RemoveMarbleFromWishlist(MarbleName.text);
            MarbleManager.RemoveMarbleFromSelectionMenu(MarbleName.text);
            Destroy(this.gameObject);
            //RemoveFromEverywhere();
        }
    }

    void UpdatePage()
    {
        if(tileDetailsList.noof_imagedownload > 0)
            tileDetailsList.noof_imagedownload--;
    }
}
