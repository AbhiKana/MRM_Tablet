using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class ListOfMarblesInventory : MonoBehaviour
{
    [SerializeField] MarbleLoader marbleLoader;
    [SerializeField] GetAllMarbles getMarbles;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parentObjectToSpawn;

    public List<ShowMarbleDetails> listOfAllMarbles = new List<ShowMarbleDetails>();

    // Unity's Official Object Pool
    private ObjectPool<GameObject> tilePool;
    private List<GameObject> activeObjects = new List<GameObject>();

    private void Awake()
    {
        // Initialize the pool
        tilePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(tilePrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: 2000
        );
    }

    private void Start()
    {
        // Listen to the data loaded event to trigger grid spawning
        getMarbles.OnDataLoaded.AddListener(GetMarble);
    }

    void GetMarble()
    {
        GetSelectedMarbleList();
    }

    public void GetSelectedMarbleList()
    {
        // Start async spawning to prevent hang
        SpawnMarblesAsync().Forget();
    }

    private async UniTaskVoid SpawnMarblesAsync()
    {
        Debug.Log("Instantiate Marbles via Object Pool");

        // Read directly from the JSON data, NOT from the Marquee boxes
        var mDetails = getMarbles.allMarbles.getMarblesList.marbleDetails;

        int chunkSize = 5; // Spawn 5 items per frame to prevent hang
        for (int i = 0; i < mDetails.Count; i++)
        {
            GridPrefabInstantiate(mDetails[i]);

            if (i % chunkSize == 0 && i > 0)
            {
                await UniTask.Yield();
            }
        }
    }

    // Now accepts MarbleDetail (JSON data) instead of ShowMarbleDetails
    public void GridPrefabInstantiate(MarbleDetail marbleDetail)
    {
        GameObject marbleObj = tilePool.Get();
        marbleObj.transform.SetParent(parentObjectToSpawn, false);
        marbleObj.transform.localScale = Vector3.one;

        ShowMarbleDetails marble = marbleObj.GetComponent<ShowMarbleDetails>();

        marbleObj.name = marble.marbleName = marbleDetail.marble_name;
        marble.price = marbleDetail.price;
        marble.tileID = marbleDetail.id;
        marble.categoryID = marbleDetail.category_id;
        marble.marbleDetailsWithCategoryID = marbleDetail;

        // IMPORTANT CHANGE: Call SetData() so the Grid tile downloads its own image.
        // Previously, the Marquee was downloading images and syncing them, 
        // but since the Marquee is now virtualized to 7 boxes, the Grid must fetch its own.
        marble.SetData();
        marble.ShowData();

        listOfAllMarbles.Add(marble);
        activeObjects.Add(marbleObj);
    }

    // Call this if you ever need to clear the grid (e.g., changing categories)
    public void ClearGrid()
    {
        foreach (var obj in activeObjects)
        {
            tilePool.Release(obj);
        }
        activeObjects.Clear();
        listOfAllMarbles.Clear();
    }

    public void SetMarbleDetails(List<ShowMarbleDetails> showMarbleDetails)
    {
        // This remains unchanged
        foreach (Transform transform in parentObjectToSpawn.transform)
        {
            var showDetails = transform.GetComponent<ShowMarbleDetails>();
            if (!listOfAllMarbles.Contains(showDetails))
            {
                listOfAllMarbles.Add(showDetails);
                showDetails.gameObject.name = showDetails.marbleName;
            }
        }
    }
}