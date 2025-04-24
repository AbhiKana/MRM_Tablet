using System.Collections.Generic;
using UnityEngine;

public class SelectionPanelController : MonoBehaviour
{

    [SerializeField] StoreMarbleDetails storeMarbleDetails;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parentObjectToSpawn;

    [SerializeField] int noof_marble;

    List<SelectionTileDetails> previousSelectedList = new List<SelectionTileDetails>();

    //Marble list
    public void GetSelectedMarbleList()
    {
        foreach (Transform child in parentObjectToSpawn)
        {
            Destroy(child.gameObject);
        }

        noof_marble = storeMarbleDetails.WishListMarble.Count;
        for (int i = 0; i < noof_marble; i++)
        {
            GameObject marbleObj = Instantiate(tilePrefab).gameObject;
            SelectionTileDetails marble = marbleObj.GetComponent<SelectionTileDetails>();

            if (marble != null)
            {
                marble.transform.SetParent(parentObjectToSpawn);
                marble.transform.localScale = Vector3.one;
                var tileDetail = storeMarbleDetails.WishListMarble[i];
                marble.SetTileDetails(tileDetail.mainTexture, tileDetail.marble_name);
            }
        }
    }
}
