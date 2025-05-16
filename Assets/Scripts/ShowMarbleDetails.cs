using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent OnDataLoadOnce;

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
            if (marble.id == marbleDetailsWithCategoryID.id)
            {
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
        //Debug.Log("Set Image");
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
        priceText.text = price + " sq/ft";
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

        if (MarbleLoader.totalImageCount == MarbleLoader.downlaodedImageCount)
        {
            GetAllMarbles getAllMarbles = FindObjectOfType<GetAllMarbles>();
            getAllMarbles.OnDataLoaded?.Invoke();

            ShowMarbleDetails instance = new ShowMarbleDetails();
            instance.OnDataLoadOnce?.Invoke();
            Debug.LogError("Loaded once");
        }
    }

    public void StoreRoomTextures(SpecificMarbleDetails specificMarbleDetails)
    {
        Debug.Log("Current Object name: " + gameObject.name);
        tileID = specificMarbleDetails.id;
        marbleName = specificMarbleDetails.marble_name;
        price = specificMarbleDetails.price;
        categoryID = specificMarbleDetails.category_id;
        texture = specificMarbleDetails.mainTexture;
        m_Textures = specificMarbleDetails.textures;
        IsWishlisted = specificMarbleDetails.isSelected;
    }

    public SpecificMarbleDetails DataToSend()
    {
        Debug.Log("Data send to store " + gameObject.name);
        return new SpecificMarbleDetails
        {
            id = tileID,
            marble_name = marbleName,
            price = price,
            category_id = categoryID,
            mainTexture = texture,
            textures = m_Textures,
            isSelected = IsWishlisted
        };
    }
}
