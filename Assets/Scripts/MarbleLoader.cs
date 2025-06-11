using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using AirFishLab.ScrollingList;
using System.Collections.Generic;

public class MarbleLoader : MonoBehaviour
{
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] GetAllMarbles getAllMarbles;

    [SerializeField] GameObject CategoryParent;
    [SerializeField] GameObject categoryPrefab;

    public static int totalImageCount = 0;
    public static int downlaodedImageCount = 0;

    bool IsAllImageDownloaded;

    public List<ShowMarbleDetails> listOfAllMarbles = new List<ShowMarbleDetails>();
    public static UnityEvent OnMarbleLoaded;
    
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
        SetMarbleDetails(listOfAllMarbles);
        SpawnCategoryList();
    }

    bool IsCategorySpawn = false;
    public void SpawnCategoryList()
    {
        if (!IsCategorySpawn)
        {
            var category = getAllMarbles.categories.category;
            for (int i = 0; i < category.Count; i++)
            {
                GameObject g = Instantiate(categoryPrefab, CategoryParent.transform.position, Quaternion.identity);
                g.transform.SetParent(CategoryParent.transform);
                g.transform.localScale = Vector3.one;

                Toggle toggle = g.GetComponent<Toggle>();
                toggle.group = CategoryParent.GetComponent<ToggleGroup>();
                toggle.GetComponentInChildren<TextMeshProUGUI>().text = category[i].category_name;
                
                /*if (i == 0)
                    toggle.isOn = true;
                else 
                    toggle.isOn = false;*/
                ToggleSpriteChange t = toggle.GetComponent<ToggleSpriteChange>();
                t.OnToggleClicked();
                
                t.GetComponent<CategoryToggle>().categoryId = category[i].category_id;
            }
            IsCategorySpawn = true;
        }
    }

    public void SetMarbleDetails(List<ShowMarbleDetails> showMarbleDetails)
    {
        List<string> url = new List<string>();
        if (!IsAllImageDownloaded)
        {
            var mDetails = getAllMarbles.allMarbles.getMarblesList.marbleDetails;
            url.Clear();
            for (int i = 0; i < showMarbleDetails.Count; i++)
            {
                showMarbleDetails[i].marbleDetailsWithCategoryID = mDetails[i];
                url.Add(showMarbleDetails[i].marbleDetailsWithCategoryID.main_img);
                showMarbleDetails[i].SetData();
            }

            //GetImageUsingthread(url.ToArray()); 
        }
    }

    /*void GetImageUsingthread(string[] url)
    {
        int count = 0;
        ThreadedImageDownloader.Instance.DownloadAndProcessImage
        (
            url,
            dummy,
            (success) =>
            {
                showMarbleDetails.m_Textures[count] = dummy.texture;
                if (success)
                    CheckAllImageLoaded();
            },
            () => { count++; }
        );
    }*/
    public void OnDataLoadedSucessfully()
    {
        IsAllImageDownloaded = true;
        downlaodedImageCount = 0;
    }

    public void GetTotalImageCount()
    {
        totalImageCount = getAllMarbles.allMarbles.getMarblesList.marbleDetails.Count;
        Debug.Log($"Total image count: {totalImageCount}");
    }
}

