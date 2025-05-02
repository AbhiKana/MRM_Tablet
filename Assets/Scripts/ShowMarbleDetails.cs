using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMarbleDetails : MonoBehaviour
{
    [HideInInspector]
    public Marble marbleDetailsWithCategoryID;
    public WWWRequestTC requestTC;
    
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
        GetImage(marbleDetailsWithCategoryID.img, image);
        tileName = marbleDetailsWithCategoryID.marble_name;
        price = marbleDetailsWithCategoryID.price;
        tileID = marbleDetailsWithCategoryID.id;
        categoryID = marbleDetailsWithCategoryID.category_id;
    }

    public void ShowData()
    {
        marbleNameText.text = tileName;
        priceText.text = price;
        //image.texture = texture; 
        Debug.LogError("Add texture on: "+ gameObject.name);
    }

    void GetImage(string url, RawImage image)
    {
        requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                CheckAllImageLoaded();
                image.texture = rawTex;
                Debug.Log("All Image downloaded");
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
            Debug.Log("DOWNLOAD IMAGE " + d_image);
            GetAllMarbles getAllMarbles = FindObjectOfType<GetAllMarbles>();
            getAllMarbles.OnDataLoaded?.Invoke();
        }
    }
}
