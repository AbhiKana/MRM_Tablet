using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMarbleDetails : MonoBehaviour
{
    public MarbleDetail marbleDetailsWithCategoryID;
    [SerializeField] Texture[] m_Textures;
    
    FetchQRData fetchQRData;
    WWWRequestTC requestTC;
    public RawImage image;

    public Texture texture;
    public string marbleName;
    public string price;

    public int categoryID;
    public int tileID;

    [SerializeField] TextMeshProUGUI marbleNameText;
    [SerializeField] TextMeshProUGUI priceText;


    private void Start()
    {
        requestTC = new WWWRequestTC();
        fetchQRData = FindObjectOfType<FetchQRData>(true);

        fetchQRData.OnDataLoaded.AddListener(() =>
        {
            if (fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails.id == marbleDetailsWithCategoryID.id)
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
        //Debug.Log("gameobject name: "+ this.gameObject.name);
        GetImage(marbleDetailsWithCategoryID.main_img, image);
        marbleName = marbleDetailsWithCategoryID.marble_name;
        price = marbleDetailsWithCategoryID.price;
        tileID = marbleDetailsWithCategoryID.id;
        categoryID = marbleDetailsWithCategoryID.category_id;

        //StoreRoomTextures();
    }

    public void ShowData()
    {
        marbleNameText.text = marbleName;
        priceText.text = price +" sq/ft";
    }

    void GetImage(string url, RawImage image)
    {
        //Debug.Log("Download texture");
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                CheckAllImageLoaded();
                image.texture = rawTex;
                //Debug.Log("All Image downloaded");
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
        
        //Debug.Log("Total Image: "+ t_image + " =====> " + d_image);
        if(MarbleLoader.totalImageCount == MarbleLoader.downlaodedImageCount)
        {
            //Debug.Log("DOWNLOAD IMAGE " + d_image + "   Total Image: " + t_image + " =====> " + d_image);
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
        /*GetImageInBG(marbleDetailsWithCategoryID.texture_img[0], m_Textures[0]);
        GetImageInBG(marbleDetailsWithCategoryID.texture_img[1], m_Textures[1]);
        GetImageInBG(marbleDetailsWithCategoryID.texture_img[2], m_Textures[2]);
        GetImageInBG(marbleDetailsWithCategoryID.texture_img[3], m_Textures[3]);
        GetImageInBG(marbleDetailsWithCategoryID.texture_img[4], m_Textures[4]);*/
    }
}
