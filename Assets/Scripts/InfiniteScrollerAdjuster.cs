using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollerAdjuster : MonoBehaviour
{
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] GetAllMarbles getAllMarbles;
    [SerializeField] GameObject tilePrefab;

    [SerializeField] GameObject CategoryParent;
    [SerializeField] GameObject categoryPrefab;

    [SerializeField] float minSpacing = 50f;
    [SerializeField] float maxSpacing = 200f;

    RectTransform rectTransform;
    
    int numOfbox;
    float prefabWidth;
    float spacing;

    public static int totalImageCount = 0;
    public static int downlaodedImageCount = 0;

    bool IsAllImageDownloaded;

    [SerializeField] List<ShowMarbleDetails> listOfAllMarbles = new List<ShowMarbleDetails>();
    
    private void Start()
    {
        SetCircularList();

        if(getAllMarbles != null) 
            getAllMarbles.OnAllMarbleDataLoaded.AddListener(SetCircularList);
    }
    private void SetCircularList()
    {
        rectTransform = circularScrollingList.GetComponent<RectTransform>();
        numOfbox = getAllMarbles.allMarbles.marbleDetails.Count /*+7*/;
        circularScrollingList.BoxSetting._numOfBoxes = numOfbox;

        AdjustSpacing();
        AdjustPositionAndSize();
        circularScrollingList.GenerateBoxesAndArrange();

        if (numOfbox == 1)
            circularScrollingList.enabled = false;
    } 
    public void GetAllAvailableMarbles()
    {
        StartCoroutine(StoreMarblesInList());
    }
    IEnumerator StoreMarblesInList()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Transform transform in circularScrollingList.transform)
        {
            var showDetails = transform.GetComponent<ShowMarbleDetails>();
            if (!listOfAllMarbles.Contains(showDetails))
            {
                listOfAllMarbles.Add(showDetails);
            }
        }
        //getAllMarbles.SetMarbleDetails(listOfAllMarbles);
        
        SetMarbleDetails(listOfAllMarbles);
        SpawnCategoryList();

    }
    void AdjustSpacing()
    {
        spacing = Mathf.Lerp(maxSpacing, minSpacing, numOfbox / 10f);
        prefabWidth = tilePrefab.GetComponent<RectTransform>().sizeDelta.x + spacing;
    }
    void AdjustPositionAndSize()
    {
        rectTransform.sizeDelta = new Vector2(prefabWidth * numOfbox, rectTransform.sizeDelta.y);

        if (numOfbox % 2 == 0 && numOfbox > 3)
        {
            float shift = prefabWidth / 2f;
            rectTransform.anchoredPosition = new Vector2(shift, rectTransform.anchoredPosition.y);
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(0f, rectTransform.anchoredPosition.y);
        }
    }
    public void SetMarbleDetails(List<ShowMarbleDetails> showMarbleDetails)
    {
        if (!IsAllImageDownloaded)
        {
            var mDetails = getAllMarbles.allMarbles.marbleDetails;
            for (int i = 0; i < showMarbleDetails.Count; i++)
            {
                showMarbleDetails[i].marbleDetailsWithCategoryID = mDetails[i];
                showMarbleDetails[i].SetData();
            }
        }
    }

    public void OnDataLoadedSucessfully()
    {
        IsAllImageDownloaded = true;
        downlaodedImageCount = 0;
    }

    bool isCategorySpawn = false;
    public void SpawnCategoryList()
    {
        if (!isCategorySpawn)
        {
            var category = getAllMarbles.allMarbles.category;
            for (int i = 0; i < category.Count; i++)
            {
                GameObject g = Instantiate(categoryPrefab, CategoryParent.transform.position, Quaternion.identity);
                g.transform.SetParent(CategoryParent.transform);
                g.transform.localScale = Vector3.one;

                g.GetComponentInChildren<TextMeshProUGUI>().text = category[i].category_name;
            }
            isCategorySpawn = true;
        }
    }
    /*void GetImage(string url, RawImage image)
    {
        getAllMarbles.requestTC.GetTexture(url, (str, rawTex, isSucess) =>
        {
            if (isSucess)
            {
                image.texture = rawTex;
                downlaodedImageCount++;

                if (totalImageCount == downlaodedImageCount)
                {
                    IsAllImageDownloaded = true;
                    //getAllMarbles.OnDataLoaded?.Invoke();
                    Debug.Log("All Image downloaded");
                    downlaodedImageCount = 0;
                }
            }
            else
                Debug.Log("Couldn't fetch image data");
        });
    }*/

    public void GetTotalImageCount()
    {
        totalImageCount = getAllMarbles.allMarbles.marbleDetails.Count;
        Debug.Log($"Total image count: {totalImageCount}");
    }
}
