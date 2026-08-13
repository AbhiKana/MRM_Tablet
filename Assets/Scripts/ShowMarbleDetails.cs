using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShowMarbleDetails : MonoBehaviour
{
    public StoreMarbleDetails storeMarbleDetails;
    public SyncMarbleDetails syncMarbleDetails;

    public ShowMarbleDetails syncShowMarbleDetail;
    public MarbleDetail marbleDetailsWithCategoryID;
    //public Texture[] m_Textures;

    FetchQRData fetchQRData;
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

    private CancellationTokenSource textureToken = new CancellationTokenSource();

    private void Start()
    {
        fetchQRData = FindObjectOfType<FetchQRData>(true);
        storeMarbleDetails = FindObjectOfType<StoreMarbleDetails>(true);

        if (syncMarbleDetails == null)
            syncMarbleDetails = FindObjectOfType<SyncMarbleDetails>();

        storeMarbleDetails.OnToggleSet.AddListener((isOn) =>
        {
            var marble = storeMarbleDetails.marbleDetails;
            //Debug.Log("OnToggle Click: "+ marble.id + " " + marbleDetailsWithCategoryID.id);
            if (marble.id == marbleDetailsWithCategoryID.id)
                SetWishlist(isOn);
        });

        fetchQRData.OnDataLoaded.AddListener(() =>
        {
            var marble = fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails;
            if (marble.id == marbleDetailsWithCategoryID.id)
            {
                texture = fetchQRData.TopMarbleImage.texture;
                //m_Textures = fetchQRData.boxImageTexture;
            }
        });
    }

    void SetWishlist(bool val)
    {
        Debug.Log("Before Assign check name: " + gameObject.name);
        IsWishlisted = val;
        syncMarbleDetails.SyncMarbleWishlistedValue(this, syncMarbleDetails.gridMarbles, IsWishlisted);
    }

    public void SetImage(Texture t)
    {
        image.texture = t;
    }

    public void SetValues(Texture t)
    {
        image.texture = t;
    }

    // Add this to ShowMarbleDetails.cs
    public void UpdateData(MarbleDetail newDetails)
    {
        // 1. Cancel any ongoing image download from the previous marble this tile displayed
        textureToken?.Cancel();
        textureToken = new System.Threading.CancellationTokenSource();

        // 2. Update the data
        marbleDetailsWithCategoryID = newDetails;

        // 3. Update the UI text instantly
        MarbleTextDetails();
        ShowData();

        // 4. Reset the image to blank and start downloading the new image
        //image.texture = null;
        GetImage(newDetails.main_img, image).Forget();
    }

    public void SetData()
    {
        MarbleTextDetails();
        //Debug.Log(marbleDetailsWithCategoryID.main_img);
        GetImage(marbleDetailsWithCategoryID.main_img, image).Forget();
        //ThreadedImageDownloader.Instance.DownloadAndProcessImage
        //(
        //    marbleDetailsWithCategoryID.main_img,
        //    image,
        //    (success) =>
        //    {
        //        if (success) CheckAllImageLoaded();
        //    },
        //    () =>
        //    {
        //        CheckInWishList();
        //    }
        //);
    }

    private void CheckInWishList()
    {
        StoreMarbleDetails storeMarbleDetails = FindObjectOfType<StoreMarbleDetails>();

        var marbleDetail = storeMarbleDetails.list.Find(m => m.id == marbleDetailsWithCategoryID.id);

        if (marbleDetail != null)
        {
            //Debug.Log(marbleDetail.marble_name);
            IsWishlisted = marbleDetail.isSelected;
            //m_Textures = marbleDetail.textures;
            texture = marbleDetail.mainTexture;
        }

        //SpecificMarbleDetails marbleDetail = null;

        /*foreach (var m in storeMarbleDetails.list)
        {
            Debug.Log(m.marble_name);
            if (m.id == marbleDetailsWithCategoryID.id)
            {
                Debug.LogError(m.marble_name);
                IsWishlisted = marbleDetail.isSelected;
                marbleDetail = null;
            }
        }*/
    }

    public void MarbleTextDetails()
    {
        marbleName = marbleDetailsWithCategoryID.marble_name;
        price = marbleDetailsWithCategoryID.price;
        tileID = marbleDetailsWithCategoryID.id;
        categoryID = marbleDetailsWithCategoryID.category_id;
        gameObject.name = marbleName + 1;
    }

    public void ShowData()
    {
        marbleNameText.text = marbleName;
        priceText.text = price + " sq/ft";
    }

    private async UniTask GetImage(string url, RawImage image)
    {
        CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(textureToken.Token, this.GetCancellationTokenOnDestroy());
        await WWWRequestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (this == null || image == null) return;

            if (isSucess)
            {
                CheckAllImageLoaded();
                CheckInWishList();
                TextureScale.Bilinear(rawTex, 100, 100);
                image.texture = rawTex;
                texture = rawTex;
                //syncMarbleDetails.SyncMarbleArrayTexture(this, new ShowMarbleDetails[] { syncShowMarbleDetail });
            }
            else
                Debug.Log("Couldn't fetch image data");
        }, linkedCts.Token);
    }

    private void CheckAllImageLoaded()
    {
        var t_image = MarbleLoader.totalImageCount;
        var d_image = MarbleLoader.downlaodedImageCount;
        MarbleLoader.downlaodedImageCount++;
        //Debug.Log("totalImage: " + t_image + " dlImage: " + d_image);
        if (MarbleLoader.totalImageCount == MarbleLoader.downlaodedImageCount)
        {
            //StopLoader();
        }
    }

    //this is not being used right now
    public void StopLoader()
    {
        GetAllMarbles getAllMarbles = FindObjectOfType<GetAllMarbles>();

        getAllMarbles.OnDataLoaded?.Invoke();

        ShowMarbleDetails instance = new ShowMarbleDetails();
        instance.OnDataLoadOnce?.Invoke();
    }

    public void StoreRoomTextures(SpecificMarbleDetails specificMarbleDetails)
    {
        tileID = specificMarbleDetails.id;
        marbleName = specificMarbleDetails.marble_name;
        price = specificMarbleDetails.price;
        categoryID = specificMarbleDetails.category_id;
        image.texture = texture = specificMarbleDetails.mainTexture;
        //m_Textures = specificMarbleDetails.textures;
        IsWishlisted = specificMarbleDetails.isSelected;
        //Debug.Log("Current Object name: " + gameObject.name + " " + IsWishlisted);
    }

    public SpecificMarbleDetails DataToSend()
    {
        //Debug.Log("Data send to store " + gameObject.name);
        return new SpecificMarbleDetails
        {
            id = tileID,
            marble_name = marbleName,
            price = price,
            category_id = categoryID,
            mainTexture = texture,
            //textures = m_Textures,
            isSelected = IsWishlisted
        };
    }

    private void OnDestroy()
    {
        textureToken?.Cancel();
    }
}
