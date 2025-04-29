using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
class MarbleDetailsWithCategoryID
{
    public int id;
    public string marble_name;
    public int marbleCategory_id;
    public Texture marble_img;
}

public class ShowMarbleDetails : MonoBehaviour
{
    [HideInInspector]
    public Marble marbleDetailsWithCategoryID;
    public WWWRequestTC requestTC;
    
    public RawImage image;
    
    public string tileName;

    public int categoryID;
    public int tileID;


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
        GetImage(marbleDetailsWithCategoryID.img, image/*, ref count*/);
        tileName = marbleDetailsWithCategoryID.marble_name;
        tileID = marbleDetailsWithCategoryID.id;
        categoryID = marbleDetailsWithCategoryID.category_id;
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
        var d_image = InfiniteScrollerAdjuster.downlaodedImageCount;
        var t_image = InfiniteScrollerAdjuster.totalImageCount;
        InfiniteScrollerAdjuster.downlaodedImageCount++;
        //d_image += d_image;
        
        if(InfiniteScrollerAdjuster.totalImageCount == InfiniteScrollerAdjuster.downlaodedImageCount)
        {
            Debug.Log("DOWNLOAD IMAEGE " + d_image);
            GetAllMarbles getAllMarbles = FindObjectOfType<GetAllMarbles>();
            getAllMarbles.OnDataLoaded?.Invoke();
        }
    }
}
