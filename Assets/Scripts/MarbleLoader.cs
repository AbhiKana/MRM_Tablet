using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        if (!IsAllImageDownloaded)
        {
            var mDetails = getAllMarbles.allMarbles.getMarblesList.marbleDetails;
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

    public void GetTotalImageCount()
    {
        totalImageCount = getAllMarbles.allMarbles.getMarblesList.marbleDetails.Count;
        Debug.Log($"Total image count: {totalImageCount}");
    }
}

