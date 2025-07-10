using System.Collections;
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
        Debug.Log("Open Page: "+ index);
        if (pageHistory.Count > 0)
        {
            GameObject currentPage = pageHistory[pageHistory.Count - 1];
            currentPage.SetActive(false);
        }

        GameObject newPage = listOfObjects[index];
        newPage.SetActive(true);

        AddPageHistory(newPage); // use the improved dynamic version
    }


    public void OpenPage(GameObject newPage)
    {
        Debug.Log("Open Page: "+ newPage.name);
        if (pageHistory.Count > 0)
        {
            GameObject currentPage = pageHistory[pageHistory.Count - 1];
            currentPage.SetActive(false);
        }

        newPage.SetActive(true);
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

        if (pageHistory.Contains(objToAdd))
        {
            pageHistory.Remove(objToAdd);
        }

        pageHistory.Add(objToAdd);
    }

    public void OnMarbleSelection()
    {
        ClearOldList();
        RefreshList();
    }

    public void RefreshList()
    {
        StartCoroutine("RefreshListData");
    }

    public IEnumerator RefreshListData()
    {
        if (scrollSnap.ChildObjects.Length > 0)
        {
            ClearOldList();
        }

        yield return new WaitForSeconds(0.4f);
        Debug.Log("Child count after refresh: " + scrollSnap.ChildObjects.Length);
        int previousIndex = listOfObjects[6].transform.GetSiblingIndex();
        listOfObjects[6].transform.SetSiblingIndex(0);
        listOfObjects[6].SetActive(true);
        listOfObjects[6].transform.SetSiblingIndex(previousIndex);
        listOfObjects[6].SetActive(false);
    }
    public void ClearOldList()
    {
        List<GameObject> gameObjectsList = new List<GameObject>(scrollSnap.ChildObjects);
        foreach (GameObject obj in scrollSnap.ChildObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        gameObjectsList.Clear();
        scrollSnap.ChildObjects = gameObjectsList.ToArray();
    }
    public void ResetchildObjLength()
    {
        scrollSnap.ChildObjects = null;
    }
}
