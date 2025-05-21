using UnityEngine;

public class SyncMarbleDetails : MonoBehaviour
{
    [SerializeField] GetAllMarbles getAllMarbles;
    [SerializeField] FetchQRData fetchQRData;

    public ShowMarbleDetails[] marqueeMarbles;
    public ShowMarbleDetails[] gridMarbles;

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
    public void SyncMarbleWishlistedValue(ShowMarbleDetails source, ShowMarbleDetails[] marbles)
    {
        Debug.Log("Iswishlist to be on: "+ source.ToString());

        foreach (ShowMarbleDetails show in marbles)
        {
            if(show.tileID==  source.tileID)
                show.IsWishlisted = source.IsWishlisted;
        }
        Debug.Log($"Marble {source.marbleName} synced between Grid and Marquee");
    }

    public void SyncMarbleArrayTexture(ShowMarbleDetails source, ShowMarbleDetails[] marbles)
    {
        foreach (ShowMarbleDetails show in marbles)
        {
            if (show.tileID == source.tileID)
            {
                Debug.Log("Data found");
                var data = source.DataToSend();
                show.StoreRoomTextures(data);
                //show.IsWishlisted = source.IsWishlisted;
            }
        }
        

        /*if (marqueeMarbles[i].m_Textures.Length < 1)
        {
            Debug.Log("Set Marquee array list");
            marqueeMarbles[i].m_Textures = new Texture[5];
        }
        marqueeMarbles[i].StoreRoomTextures(data);*/
    }

    public void SetWishlistValue(ShowMarbleDetails showMarbleDetails, bool val)
    {
        showMarbleDetails.IsWishlisted = val;
    }
}
