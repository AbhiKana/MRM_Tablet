using System.Collections.Generic;
using UnityEngine;

public class ListOfMarblesInventory : MonoBehaviour
{
    [SerializeField] MarbleLoader marbleLoader;
    [SerializeField] GetAllMarbles getMarbles;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parentObjectToSpawn;

    public List<ShowMarbleDetails> listOfAllMarbles = new List<ShowMarbleDetails>();

    private void Start()
    {
        getMarbles.OnDataLoaded.AddListener(GetSelectedMarbleList);
    }

    public void GetSelectedMarbleList()
    {
        Debug.Log("Instantiate Marbles");
        var noof_Marbles = marbleLoader.listOfAllMarbles;
        for (int i = 0; i < noof_Marbles.Count; i++)
        {
            GameObject marbleObj = Instantiate(tilePrefab).gameObject;
            marbleObj.transform.SetParent(parentObjectToSpawn);
            marbleObj.transform.localScale = Vector3.one;

            ShowMarbleDetails marble = marbleObj.GetComponent<ShowMarbleDetails>();
            marbleLoader.gameObject.name = marble.name;
            marble.image.texture = noof_Marbles[i].image.texture;
            marble.texture = noof_Marbles[i].image.texture;
            marble.marbleName = noof_Marbles[i].marbleName;
            Debug.Log("<color=yellow> Assign Image </color>");
            marble.price = noof_Marbles[i].price;
            marble.tileID = noof_Marbles[i].tileID;
            marble.categoryID = noof_Marbles[i].categoryID;
            marble.ShowData();
        }

        SetMarbleDetails(listOfAllMarbles);
    }

    public void SetMarbleDetails(List<ShowMarbleDetails> showMarbleDetails)
    {

        foreach (Transform transform in parentObjectToSpawn.transform)
        {
            var showDetails = transform.GetComponent<ShowMarbleDetails>();
            if (!listOfAllMarbles.Contains(showDetails))
            {
                listOfAllMarbles.Add(showDetails);
                showDetails.gameObject.name = showDetails.marbleName;
            }
        }

        var mDetails = getMarbles.allMarbles.getMarblesList.marbleDetails;
        for (int i = 0; i < showMarbleDetails.Count; i++)
        {
            showMarbleDetails[i].marbleDetailsWithCategoryID = mDetails[i];
            //showMarbleDetails[i].MarbleTextDetails();
        }
        
    }
}
