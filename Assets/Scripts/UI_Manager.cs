using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class UI_Manager : MonoBehaviour
{
    public HorizontalScrollSnap scrollSnap;
    public StoreMarbleDetails marbleDetails;
    public GameObject HeaderPage;
    public GameObject[] listOfObjects;
    [SerializeField] private List<GameObject> pageHistory = new List<GameObject>();

    void Start()
    {
        DisbaleObjects();
    }

    public void DisbaleObjects()
    {
        foreach (GameObject go in listOfObjects)
        {
            go.SetActive(false);
        }
    }


    public void OpenPage(int index)
    {
        if (pageHistory.Count > 0)
        {
            GameObject currentPage = pageHistory[pageHistory.Count - 1];
            currentPage.SetActive(false);
        }

        listOfObjects[index].SetActive(true);
        pageHistory.Add(listOfObjects[index]);
    }
    public void OpenPage(GameObject newPage)
    {
        if (pageHistory.Count > 0)
        {
            GameObject currentPage = pageHistory[pageHistory.Count - 1];
            currentPage.SetActive(false);
        }

        newPage.SetActive(true);
        //pageHistory.Add(newPage);
    }

    public void GoBack()
    {
        if (pageHistory.Count <=2 )
        {
            Debug.Log("disbale header");
            HeaderPage.SetActive(false);
        }

        if (pageHistory.Count > 1)
        {
            GameObject currentPage = pageHistory[pageHistory.Count - 1];
            currentPage.SetActive(false);
            pageHistory.RemoveAt(pageHistory.Count - 1);

            GameObject previousPage = pageHistory[pageHistory.Count - 1];
            previousPage.SetActive(true);
        }
    }

    public void AddPageHistory(GameObject objToAdd)
    {
        objToAdd.SetActive(true);
        pageHistory.Add(objToAdd);
    }
}
