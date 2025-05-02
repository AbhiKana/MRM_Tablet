using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfMarblesInventory : MonoBehaviour
{
    [SerializeField] GetAllMarbles getMarbles;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parentObjectToSpawn;

    [SerializeField] int noof_marble;

    //Marble list
    public List<ShowMarbleDetails> availableTile;

    public void GetSelectedMarbleList()
    {
        /*foreach (Transform child in parentObjectToSpawn)
        {
            Destroy(child.gameObject);
        }*/
        //availableTile.Clear();
        
        noof_marble = getMarbles.allMarbles.marbleDetails.Count;
        
        for (int i = 0; i < noof_marble; i++)
        {
            GameObject marbleObj = Instantiate(tilePrefab).gameObject;
            ShowMarbleDetails marble = marbleObj.GetComponent<ShowMarbleDetails>();

            if (marble != null)
            {
                availableTile.Add(marble);
                //marble.SetData();
            }
        }

        //int previousCount 
    }
}
