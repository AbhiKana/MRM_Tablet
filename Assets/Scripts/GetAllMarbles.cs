using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

public class GetAllMarbles : MonoBehaviour
{
    public MarbleList allMarbles;
    public Catergories categories;

    public UnityEvent OnAllMarbleDataLoaded;
    public UnityEvent OnDataLoaded;

    private EntityManager entityManager;

    private async void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.IsCreated) return;

        entityManager = world.EntityManager;

        await GetCategoryList();
        await GetAllMarbleList();
    }

    public async UniTask GetCategoryList()
    {
        string url = Url.apiUrl + Url.marbleApi;
        Debug.Log(url);
        var (json, success) = await WWWRequestTC.Get(url, new HeaderDataClass[0]);
        if (success) categories = JsonUtility.FromJson<Catergories>(json);
    }

    public async UniTask GetAllMarbleList()
    {
        WWWForm form = new WWWForm();
        string url = Url.apiUrl + Url.marbleDetails;
        var (Data, isSucess) = await WWWRequestTC.Post(url, form, new HeaderDataClass[0]);
        if (isSucess)
        {
            allMarbles.getMarblesList = JsonUtility.FromJson<GetMarblesList>(Data);
            CreateMarbleEntities();
            OnAllMarbleDataLoaded?.Invoke();
        }
    }

    private void CreateMarbleEntities()
    {
        // Only put unmanaged (struct) components in the archetype
        var archetype = entityManager.CreateArchetype(typeof(MarbleStateData));

        foreach (var detail in allMarbles.getMarblesList.marbleDetails)
        {
            // 1. Create the entity
            Entity entity = entityManager.CreateEntity(archetype);

            // 2. Set the unmanaged data (SetComponent, not SetComponentData)
            entityManager.SetComponentData(entity, new MarbleStateData
            {
                MarbleId = detail.id,
                CategoryId = detail.category_id,
                IsWishlisted = false,
                IsImageDownloaded = false
            });

            // 3. Add the managed component (MarbleGameObjectLink) dynamically.
            // We leave the View null for now; it gets assigned in MarbleLoader.SingleShowMarbleWithIndex
            entityManager.AddComponentObject(entity, new MarbleGameObjectLink { View = null });
        }
    }

}