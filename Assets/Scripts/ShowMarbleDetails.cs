using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMarbleDetails : MonoBehaviour
{
    StoreMarbleDetails storeMarbleDetails;

    public MarbleDetail marbleDetailsWithCategoryID;
    public Texture[] m_Textures;
    
    FetchQRData fetchQRData;
    WWWRequestTC requestTC;
    public RawImage image;

    public Texture texture;
    public string marbleName;
    public string price;

    public int categoryID;
    public int tileID;

    public bool IsWishlisted;

    [SerializeField] TextMeshProUGUI marbleNameText;
    [SerializeField] TextMeshProUGUI priceText;


    void SetWishlist(bool val)
    {
        Debug.Log("Before Assign check name: " + marbleName);
        IsWishlisted = val;
    }    
    private void Start()
    {
        requestTC = new WWWRequestTC();
        fetchQRData = FindObjectOfType<FetchQRData>(true);
        storeMarbleDetails = FindObjectOfType<StoreMarbleDetails>(true);

        
        storeMarbleDetails.OnToggleSet.AddListener((isOn) =>
        {
            var marble = storeMarbleDetails.marbleDetails;
            if (marble.id == marbleDetailsWithCategoryID.id)
                SetWishlist(isOn);
        });

        fetchQRData.OnDataLoaded.AddListener(() =>
        {
            var marble = fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails;
            //Debug.LogError("Data loaded");
            if (marble.id == marbleDetailsWithCategoryID.id)
            {
                Debug.LogError("ID match: "+ gameObject.name);
                texture = fetchQRData.TopMarbleImage.texture;
                m_Textures = fetchQRData.boxImageTexture;
            }
        });
    }
    public void SetImage(Texture t)
    {
        image.texture = t;
    }

    public void SetValues(Texture t)
    {
        image.texture = t;
    }

    public void SetData()
    {
        GetImage(marbleDetailsWithCategoryID.main_img, image);
        MarbleTextDetails();
    }

    public void MarbleTextDetails()
    {
        marbleName = marbleDetailsWithCategoryID.marble_name;
        price = marbleDetailsWithCategoryID.price;
        tileID = marbleDetailsWithCategoryID.id;
        categoryID = marbleDetailsWithCategoryID.category_id;
    }

    public void ShowData()
    {
        marbleNameText.text = marbleName;
        priceText.text = price +" sq/ft";
    }

    void GetImage(string url, RawImage image)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                CheckAllImageLoaded();
                image.texture = rawTex;
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }

    private static void CheckAllImageLoaded()
    {
        var d_image = MarbleLoader.downlaodedImageCount;
        var t_image = MarbleLoader.totalImageCount;
        MarbleLoader.downlaodedImageCount++;
        
        if(MarbleLoader.totalImageCount == MarbleLoader.downlaodedImageCount)
        {
            GetAllMarbles getAllMarbles = FindObjectOfType<GetAllMarbles>();
            getAllMarbles.OnDataLoaded?.Invoke();
        }
    }

    void GetImageInBG(string url, Texture image)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                /*Texture texture = rawTex;
                image =texture;*/
                Debug.Log("Download texture");
                image = rawTex;
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }
    public void StoreRoomTextures()
    {
    }
}
