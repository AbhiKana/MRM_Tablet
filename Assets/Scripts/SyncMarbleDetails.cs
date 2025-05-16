using System.Collections.Generic;
using UnityEngine;

public class SyncMarbleDetails : MonoBehaviour
{
    [SerializeField] GetAllMarbles getAllMarbles;
    [SerializeField] FetchQRData fetchQRData;

    [SerializeField] private ShowMarbleDetails[] marqueeMarbles;
    [SerializeField] private ShowMarbleDetails[] gridMarbles;

    [SerializeField] private GameObject gridMarblesParent;
    [SerializeField] private GameObject marqueeMarblesParent;

    //[SerializeField] Inventory inventory;

    private void Start()
    {
        if(getAllMarbles != null)
            getAllMarbles.OnDataLoaded.AddListener(StoreMarblesInList);
    }

    void StoreMarblesInList()
    {
        marqueeMarbles = marqueeMarblesParent.GetComponentsInChildren<ShowMarbleDetails>(true);
        Invoke(nameof(GetGridMarble), 0.5f);
    }

    void GetGridMarble()
    {
        gridMarbles = gridMarblesParent.GetComponentsInChildren<ShowMarbleDetails>(true);
        SyncDetails();
    }

    private void SyncDetails()
    {
        fetchQRData.OnDataLoaded.AddListener(() =>
        {
            for (int i = 0; i < marqueeMarbles.Length; i++)
            {
                var id = fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails.id;
                if(id == marqueeMarbles[i].tileID)
                {
                    var data = marqueeMarbles[i].DataToSend();
                    gridMarbles[i].StoreRoomTextures(data);

                    return;
                }
            }
        });

        fetchQRData.OnDataLoaded.AddListener(() =>
        {
            for (int i = 0; i < gridMarbles.Length; i++)
            {
                var id = fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails.id;
                if (id == gridMarbles[i].tileID)
                {
                    var data = gridMarbles[i].DataToSend();
                    marqueeMarbles[i].StoreRoomTextures(data);

                    return;
                }
            }
        });
    }

    /*private void SyncMarbleData(ShowMarbleDetails source, ShowMarbleDetails target)
    {
        // If target is missing details, copy from source
        if (target.texture == null)
            target.texture = source.texture;

        if (target.m_Textures == null || target.m_Textures.Length == 0)
            target.m_Textures = source.m_Textures;

        if (string.IsNullOrEmpty(target.marbleName))
            target.marbleName = source.marbleName;

        if (string.IsNullOrEmpty(target.price))
            target.price = source.price;

        if (target.tileID == 0)
            target.tileID = source.tileID;

        if (target.categoryID == 0)
            target.categoryID = source.categoryID;

        if (!target.IsWishlisted)
            target.IsWishlisted = source.IsWishlisted;

        Debug.Log($"Marble {source.marbleName} synced between Grid and Marquee");
    }*/
}
