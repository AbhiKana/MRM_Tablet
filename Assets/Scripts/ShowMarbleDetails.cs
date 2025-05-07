using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMarbleDetails : MonoBehaviour
{
    [SerializeField] List<Texture> m_Textures; 

    [HideInInspector]
    public MarbleDetail marbleDetailsWithCategoryID;
    
    WWWRequestTC requestTC;
    public RawImage image;

    public Texture texture;
    public string tileName;
    public string price;

    public int categoryID;
    public int tileID;

    [SerializeField] TextMeshProUGUI marbleNameText;
    [SerializeField] TextMeshProUGUI priceText;


    private void Start()
    {
        requestTC = new WWWRequestTC();
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
        tileName = marbleDetailsWithCategoryID.marble_name;
        price = marbleDetailsWithCategoryID.price;
        tileID = marbleDetailsWithCategoryID.id;
        categoryID = marbleDetailsWithCategoryID.category_id;

        //StoreRoomTextures();
    }

    public void ShowData()
    {
        marbleNameText.text = tileName;
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

    /*void GetImageInBG(string url, Texture image)
    {
        Debug.Log("Download texture");
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                image = rawTex;
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }

    public void StoreRoomTextures()
    {
        for (int i = 0; i < marbleDetailsWithCategoryID.texture_img.Count; i++)
        {
            GetImageInBG(marbleDetailsWithCategoryID.texture_img[i], m_Textures[i]);
        }
    }*/
}
