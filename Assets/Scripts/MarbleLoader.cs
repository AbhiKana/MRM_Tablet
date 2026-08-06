using AirFishLab.ScrollingList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MarbleLoader : MonoBehaviour
{
    public StoreMarbleDetails storeMarbleDetails;
    public SyncMarbleDetails syncMarbleDetails;

    public ListOfMarblesInventory listOfMarblesInventory;

    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] GetAllMarbles getAllMarbles;

    [SerializeField] GameObject CategoryParent;
    [SerializeField] GameObject categoryPrefab;

    public static int totalImageCount = 0;
    public static int downlaodedImageCount = 0;

    bool IsAllImageDownloaded;

    public List<ShowMarbleDetails> listOfAllMarbles = new List<ShowMarbleDetails>();
    public static UnityEvent OnMarbleLoaded;

    public Dictionary<int, string> marbleKeyValue = new Dictionary<int, string>();

    bool IsCategorySpawn = false;

    public void GetAllAvailableMarbles()
    {
        foreach (var listbox in circularScrollingList._listBoxes)
        {
            var showDetails = listbox.GetComponent<ShowMarbleDetails>();
            if (!listOfAllMarbles.Contains(showDetails))
            {
                listOfAllMarbles.Add(showDetails);
            }
        }
        SpawnCategoryList();
        SetMarbleDetails(listOfAllMarbles);
        getAllMarbles.OnDataLoaded?.Invoke();
    }

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
            int chunk = 8;
            for (int i = 0; i < showMarbleDetails.Count; i += chunk)
            {
                int end = Math.Min(i + chunk, showMarbleDetails.Count);
                for (int j = i; j < end; j++)
                    SingleShowMarbleWithIndex(showMarbleDetails, mDetails, j);
            }
        }
    }

    private void SingleShowMarbleWithIndex(List<ShowMarbleDetails> showMarbleDetails, List<MarbleDetail> mDetails, int i)
    {
        marbleKeyValue[mDetails[i].id] = mDetails[i].marble_name;
        showMarbleDetails[i].storeMarbleDetails = storeMarbleDetails;
        showMarbleDetails[i].syncMarbleDetails = syncMarbleDetails;
        showMarbleDetails[i].marbleDetailsWithCategoryID = mDetails[i];
        showMarbleDetails[i].SetData();
        listOfMarblesInventory.GridPrefabInstantiate(showMarbleDetails[i]);
    }

    public void OnDataLoadedSucessfully()
    {
        IsAllImageDownloaded = true;
        //downlaodedImageCount = 0;
    }


    //Assgined to GetAllMarbles.cs on event OnAllMarbleDataLoaded
    public async void GetTotalImageCount()
    {
        totalImageCount = getAllMarbles.allMarbles.getMarblesList.marbleDetails.Count;
        Debug.Log($"Total image count: {totalImageCount}");
        while (!circularScrollingList.CheckIsInitialized())
        {
            await Task.Yield();
        }
        GetAllAvailableMarbles();
    }   
}

