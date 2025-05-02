using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using TMPro;
using UnityEngine;

public class MarbleLoader : MonoBehaviour
{
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] GetAllMarbles getAllMarbles;

    [SerializeField] GameObject CategoryParent;
    [SerializeField] GameObject categoryPrefab;

    public static int totalImageCount = 0;
    public static int downlaodedImageCount = 0;

    bool IsAllImageDownloaded;

    [SerializeField] List<ShowMarbleDetails> listOfAllMarbles = new List<ShowMarbleDetails>();

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
            var category = getAllMarbles.allMarbles.category;
            for (int i = 0; i < category.Count; i++)
            {
                GameObject g = Instantiate(categoryPrefab, CategoryParent.transform.position, Quaternion.identity);
                g.transform.SetParent(CategoryParent.transform);
                g.transform.localScale = Vector3.one;

                g.GetComponentInChildren<TextMeshProUGUI>().text = category[i].category_name;
            }
            IsCategorySpawn = true;
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

    public void GetTotalImageCount()
    {
        totalImageCount = getAllMarbles.allMarbles.marbleDetails.Count;
        Debug.Log($"Total image count: {totalImageCount}");
    }
}

