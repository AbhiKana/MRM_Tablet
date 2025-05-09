using UnityEngine;
using UnityEngine.UI;

public class MarbleItemController : MonoBehaviour
{
    UI_Manager manager;

    [SerializeField] ShowMarbleDetails showMarbleDetails;
    [SerializeField] private Button detailsButton;
    [SerializeField] private Toggle wishlistToggle;

    private void Start()
    {
        showMarbleDetails = GetComponent<ShowMarbleDetails>();
        manager = FindAnyObjectByType<UI_Manager>();

        detailsButton.onClick.AddListener(OnDetailsButtonClicked);
        wishlistToggle.onValueChanged.AddListener(OnWishlistToggled);
    }

    private void OnDetailsButtonClicked()
    {
        manager.HandleLoaderPage(true);
        GetMRMDetails getMRMDetails = FindObjectOfType<GetMRMDetails>();

        string id = showMarbleDetails.tileID.ToString();
        getMRMDetails.ViewMarbleDetails(id);
        
        Debug.Log($"Marble Details - Name: {showMarbleDetails.marbleName}");
        //MarbleDetailsUI.Instance.ShowDetails(marbleName, marbleDescription, marbleImage);
    }

    // Method to handle wishlist toggle
    private void OnWishlistToggled(bool isOn)
    {
        if (isOn)
        {
            AddToWishlist();
        }
        else
        {
            RemoveFromWishlist();
        }
    }

    // Add to wishlist logic
    private void AddToWishlist()
    {
        Debug.Log(showMarbleDetails.marbleName + " added to wishlist.");
        //WishlistManager.Instance.AddMarble(marbleName);
    }

    private void RemoveFromWishlist()
    {
        Debug.Log(showMarbleDetails.marbleName + " removed from wishlist.");
       // WishlistManager.Instance.RemoveMarble(marbleName);
    }
}
