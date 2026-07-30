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
        //if (!IsAllImageDownloaded)
        //{
        //    Loader.Instance.LoaderActivation(true);
        //}
        yield return new WaitForSeconds(0.5f);
        foreach (Transform transform in circularScrollingList.transform)
        {
            var showDetails = transform.GetComponent<ShowMarbleDetails>();
            //showDetails.IsWishlisted = 
            if (!listOfAllMarbles.Contains(showDetails))
            {
                listOfAllMarbles.Add(showDetails);
            }
        }
        SpawnCategoryList();
        SetMarbleDetails(listOfAllMarbles);
        getAllMarbles.OnDataLoaded?.Invoke();
    }

    bool IsCategorySpawn = false;
    public void SpawnCategoryList()
    {
        if (!IsCategorySpawn)
        {
            //Loader.Instance.LoaderActivation(true);
            var category = getAllMarbles.categories.category;
            for (int i = 0; i < category.Count; i++)
            {
                GameObject g = Instantiate(categoryPrefab, CategoryParent.transform.position, Quaternion.identity);
                g.transform.SetParent(CategoryParent.transform);
                g.transform.localScale = Vector3.one;

                Toggle toggle = g.GetComponent<Toggle>();
                toggle.group = CategoryParent.GetComponent<ToggleGroup>();
                toggle.GetComponentInChildren<TextMeshProUGUI>().text = category[i].category_name;

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

