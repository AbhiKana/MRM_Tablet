using UnityEngine;
using UnityEngine.UI;

public class MarbleItemController : MonoBehaviour
{
    UI_Manager manager;

    [SerializeField] ShowMarbleDetails showMarbleDetails;
    [SerializeField] private Button detailsButton;
    [SerializeField] private Toggle wishlistToggle;
    GetMRMDetails getMRMDetails;
    private void Start()
    {
        showMarbleDetails = GetComponent<ShowMarbleDetails>();
        manager = FindAnyObjectByType<UI_Manager>();
        getMRMDetails = FindObjectOfType<GetMRMDetails>();

        detailsButton.onClick.AddListener(OnDetailsButtonClicked);
        wishlistToggle.onValueChanged.AddListener(OnWishlistToggled);
    }

    private void OnDetailsButtonClicked()
    {
        manager.HandleLoaderPage(true);
        string id = showMarbleDetails.tileID.ToString();
        getMRMDetails.ViewMarbleDetails(id);
        
        Debug.Log($"Marble Details - Name: {showMarbleDetails.marbleName}");
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
        MarbleWithTextDetails m = new MarbleWithTextDetails();
        m.id = showMarbleDetails.tileID;
        m.m_name = showMarbleDetails.marbleName;
        //WishlistManager.Instance.AddMarble(marbleName);
    }

    private void RemoveFromWishlist()
    {
        Debug.Log(showMarbleDetails.marbleName + " removed from wishlist.");
       // WishlistManager.Instance.RemoveMarble(marbleName);
    }
}
