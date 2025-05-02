using System.Collections.Generic;
using UnityEngine;

public class ListOfMarblesInventory : MonoBehaviour
{
    [SerializeField] MarbleLoader marbleLoader;
    [SerializeField] GetAllMarbles getMarbles;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parentObjectToSpawn;

    //Marble list
    public List<ShowMarbleDetails> availableTile;

    private void Start()
    {
        getMarbles.OnDataLoaded.AddListener(GetSelectedMarbleList);
    }

    public void GetSelectedMarbleList()
    {
        availableTile = marbleLoader.listOfAllMarbles;
        //MarbleLoader 
        var noof_Marbles = marbleLoader.listOfAllMarbles;
        //var noof_marble = getMarbles.allMarbles.marbleDetails;
        for (int i = 0; i < noof_Marbles.Count; i++)
        {
            GameObject marbleObj = Instantiate(tilePrefab).gameObject;
            marbleObj.transform.SetParent(parentObjectToSpawn);
            marbleObj.transform.localScale = Vector3.one;
            ShowMarbleDetails marble = marbleObj.GetComponent<ShowMarbleDetails>();
            marble.image.texture = availableTile[i].image.texture;
            marble.tileName = noof_Marbles[i].tileName;
            marble.price = noof_Marbles[i].price;
            marble.ShowData();
        }
    }
}
