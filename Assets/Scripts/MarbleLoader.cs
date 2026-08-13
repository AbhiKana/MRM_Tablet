using AirFishLab.ScrollingList;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Entities;
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

    public VirtualMarbleGrid virtualGrid;
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

        // Pass the JSON data directly to the Virtual Grid instead of spawning 1000 objects
        var allMarbleData = getAllMarbles.allMarbles.getMarblesList.marbleDetails;     
        //virtualGrid.InitializeGrid(allMarbleData);

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

    public async UniTask SetMarbleDetails(List<ShowMarbleDetails> showMarbleDetails)
    {
        var mDetails = getAllMarbles.allMarbles.getMarblesList.marbleDetails;
        int chunk = 8;
        for (int i = 0; i < showMarbleDetails.Count; i += chunk)
        {
            int end = Math.Min(i + chunk, showMarbleDetails.Count);
            for (int j = i; j < end; j++)
                SingleShowMarbleWithIndex(showMarbleDetails, mDetails, j);

            // Yield to the next frame. This stops the hang.
            await UniTask.Yield();
        }
    }

    private void SingleShowMarbleWithIndex(List<ShowMarbleDetails> showMarbleDetails, List<MarbleDetail> mDetails, int i)
    {
        //marbleKeyValue[mDetails[i].id] = mDetails[i].marble_name;
        showMarbleDetails[i].storeMarbleDetails = storeMarbleDetails;
        showMarbleDetails[i].syncMarbleDetails = syncMarbleDetails;
        showMarbleDetails[i].marbleDetailsWithCategoryID = mDetails[i];
        showMarbleDetails[i].SetData();
        //listOfMarblesInventory.GridPrefabInstantiate(showMarbleDetails[i]);

        var world = World.DefaultGameObjectInjectionWorld;
        if (world != null && world.IsCreated)
        {
            var em = world.EntityManager;
            var query = em.CreateEntityQuery(typeof(MarbleStateData));
            var states = query.ToComponentDataArray<MarbleStateData>(Allocator.TempJob);

            for (int e = 0; e < states.Length; e++)
            {
                if (states[e].MarbleId == mDetails[i].id)
                {
                    Entity entity = query.ToEntityArray(Allocator.TempJob)[e];
                    em.AddComponentObject(entity, new MarbleGameObjectLink { View = showMarbleDetails[i] });
                    break;
                }
            }

            states.Dispose();
            query.Dispose();
        }
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

