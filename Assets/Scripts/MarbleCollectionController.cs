using UnityEngine;
using UnityEngine.UI;

class MarbleData
{
    public string marbleName;
    public string marbleDetails;
    public Toggle isFavourite;
}

public class MarbleCollectionController : MonoBehaviour
{
    [SerializeField] SelectionPanelController selectionPanelController;
    [SerializeField] GameObject scannedObjectData;


}
