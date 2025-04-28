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

    private void Start()
    {
        isSelected.onValueChanged.AddListener(RemoveFromList);
    }
    public void RemoveFromList(bool val)
    {
        if (!val)
        {
            MarbleManager.RemoveMarbleFromWishlist(MarbleName.text);
            Destroy(this.gameObject);

            //RemoveFromEverywhere();
        }
    }

}
