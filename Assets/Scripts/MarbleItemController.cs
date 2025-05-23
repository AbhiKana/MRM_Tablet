using UnityEngine;
using UnityEngine.UI;

public class MarbleItemController : MonoBehaviour
{
    [SerializeField] private Button viewButton;
    [SerializeField] private Toggle wishlistToggle;
    
    private ShowMarbleDetails showMarbleDetails;
    LoadImageInBG loadImageInBG;

    UI_Manager manager;
    GetMRMDetails getMRMDetails;
    FetchQRData fetchQrData;

    private void OnEnable()
    {
        GetObjectComponent();
        SetToggle();
    }
    private void Start()
    {
        if (viewButton != null)
            viewButton.onClick.AddListener(OnDetailsButtonClicked);

        if (wishlistToggle != null)
        {
            wishlistToggle.onValueChanged.AddListener((isOn) =>
            {
                OnWishlistToggled(isOn);
            });
        }

        fetchQrData.OnDataLoaded.AddListener(() =>
        {
            if (showMarbleDetails.tileID == getMRMDetails.storeMarbleDetails.marbleDetails.id)
            {
                getMRMDetails.storeMarbleDetails.OnShowMarbleDisable(showMarbleDetails.IsWishlisted);
            }
        });
    }
    private void GetObjectComponent()
    {
        if(showMarbleDetails == null) 
            showMarbleDetails = GetComponent<ShowMarbleDetails>(); 
        
        if(loadImageInBG == null)
            loadImageInBG = GetComponent<LoadImageInBG>();
        
        if (manager == null) 
            manager = FindAnyObjectByType<UI_Manager>();

        if(getMRMDetails == null)
            getMRMDetails = FindObjectOfType<GetMRMDetails>();

        if(fetchQrData == null) 
            fetchQrData = FindObjectOfType<FetchQRData>();
    }
    private void SetToggle()
    {
        if (showMarbleDetails.IsWishlisted && wishlistToggle != null)
        {
            //Debug.Log("Wishlist value: "+ showMarbleDetails.IsWishlisted);
            wishlistToggle.isOn = true;
        }
        else
        {
            //Debug.Log("Wishlist value in else : " + showMarbleDetails.IsWishlisted);
            if (wishlistToggle != null)
                wishlistToggle.isOn = false;
        }
    }

    public void OnDetailsButtonClicked()
    {
        if (showMarbleDetails.texture != null && showMarbleDetails.m_Textures != null && showMarbleDetails.m_Textures.Length > 0)
        {
            Debug.LogError("Load Second Time");
            MarbleDetail current_detail = AssignCurrentMarbleDetails();
            Debug.LogError("Check marble name: " + current_detail.marble_name);
            fetchQrData.LoadMarbleTextData(current_detail.marble_name, current_detail.description, current_detail.dimension, current_detail.material, current_detail.finish);
            fetchQrData.LoadedMarbleImageData(showMarbleDetails.texture, showMarbleDetails.m_Textures[0], showMarbleDetails.m_Textures);
            getMRMDetails.storeMarbleDetails.SetToggleValue(showMarbleDetails.IsWishlisted);

            //showMarbleDetails.IsWishlisted = fetchQrData.
            manager.OpenPage(3);
        }
        else
        {
            Debug.LogError("Load First Time");
            manager.HandleLoaderPage(true);
            string id = showMarbleDetails.tileID.ToString();
            getMRMDetails.ViewMarbleDetails(id);
        }
        Debug.Log($"Marble Details - Name: {showMarbleDetails.marbleName}");
    }
    
    // Method to handle wishlist toggle
    private void OnWishlistToggled(bool isOn)
    {
        showMarbleDetails.syncMarbleDetails.SyncMarbleWishlistedValue(showMarbleDetails, showMarbleDetails.syncMarbleDetails.marqueeMarbles);
        //showMarbleDetails.syncMarbleDetails.SyncMarbleArrayTexture(showMarbleDetails.tileID);
        if (isOn)
        {
            AddToWishlist();
        }
        else
        {
            RemoveFromWishlist();
        }
    }
    private MarbleDetail AssignCurrentMarbleDetails()
    {
        var current_detail = showMarbleDetails.marbleDetailsWithCategoryID;
        var current_marbleDetails = getMRMDetails.storeMarbleDetails.marbleDetails;

        if (current_marbleDetails != null)
        {
            current_marbleDetails.id = current_detail.id;
            current_marbleDetails.marble_name = current_detail.marble_name;
            current_marbleDetails.description = current_detail.description;
            current_marbleDetails.dimension = current_detail.dimension;
            current_marbleDetails.material = current_detail.material;
            current_marbleDetails.finish = current_detail.finish;
            current_marbleDetails.price = current_detail.price;
            current_marbleDetails.mainTexture = showMarbleDetails.texture;
            current_marbleDetails.circleImg = showMarbleDetails.m_Textures[0];
            current_marbleDetails.textures = showMarbleDetails.m_Textures;
            current_marbleDetails.isSelected = showMarbleDetails.IsWishlisted;
            Debug.Log("Before wishlist check: " + showMarbleDetails.IsWishlisted);
            return current_detail;
        }
        return null;
    }
    private void AddToWishlist()
    {
        showMarbleDetails.IsWishlisted = true;
        if (showMarbleDetails.m_Textures != null && showMarbleDetails.m_Textures.Length == 0)
        {
            Debug.Log("M_texture is null");
            loadImageInBG.LoadMarbleImageData();
        }
        AddMarbleIntoList();
        Debug.Log(showMarbleDetails.marbleName + " added to wishlist.");
        
        MarbleWithTextDetails m = new MarbleWithTextDetails();
        m.id = showMarbleDetails.tileID;
        m.m_name = showMarbleDetails.marbleName;
    }
    private void RemoveFromWishlist()
    {
        showMarbleDetails.IsWishlisted = false;
        getMRMDetails.storeMarbleDetails.RemoveSelectedMarble(showMarbleDetails.tileID);

        Debug.Log(showMarbleDetails.marbleName + " removed from wishlist.");
       // WishlistManager.Instance.RemoveMarble(marbleName);
    }
    private void AddMarbleIntoList()
    {
        //LoadMarbleImageData();
        SpecificMarbleDetails specificMarbleDetails = new SpecificMarbleDetails();
        var marble = showMarbleDetails.marbleDetailsWithCategoryID;
        var detailViewer = getMRMDetails.storeMarbleDetails;

        specificMarbleDetails.id = marble.id;

        //getMRMDetails.LoadMarbleDetails(marble.id.ToString());
        specificMarbleDetails.category_id= marble.category_id;
        specificMarbleDetails.availability = marble.availability;
        specificMarbleDetails.marble_name = marble.marble_name;
        specificMarbleDetails.description = marble.description;
        specificMarbleDetails.dimension = marble.dimension;
        specificMarbleDetails.material = marble.material;
        specificMarbleDetails.finish = marble.finish;
        specificMarbleDetails.price = marble.price;
        specificMarbleDetails.mainTexture = showMarbleDetails.image.texture;
        specificMarbleDetails.isSelected = showMarbleDetails.IsWishlisted;
        specificMarbleDetails.url = marble.main_img;

        if (specificMarbleDetails.textures == null)
            specificMarbleDetails.textures = new Texture[5];

        if (!detailViewer.AlreadyExists(marble.id, detailViewer.list))
        {
            detailViewer.list.Add(specificMarbleDetails);
        }
    }

    /*void LoadMarbleImageData()
    {
        var marbleDet = showMarbleDetails.marbleDetailsWithCategoryID;
        if (marbleDet.texture_img.Count > 0)
        {
            GetImage(marbleDet.texture_img[0], 0); // Using Texture instead of RawImage

            for (int i = 0; i < marbleDet.texture_img.Count; i++)
            {
                GetImage(marbleDet.texture_img[i], i);
            }
        }
    }
    WWWRequestTC requestTC;

    void GetImage(string url, int textureIndex)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                TextureScale.Bilinear(rawTex, 200, 200);
                showMarbleDetails.m_Textures[textureIndex] = rawTex;
            }
            else
            {
                Debug.Log("<color=red>Couldn't fetch image data</color>");
            }
        });
    }*/
}
