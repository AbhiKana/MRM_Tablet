using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class UI_Manager : MonoBehaviour
{
    public HorizontalScrollSnap scrollSnap;
    public StoreMarbleDetails marbleDetails;

    public GameObject HeaderPage;
    public GameObject LoaderPage;

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

        GameObject newPage = listOfObjects[index];
        newPage.SetActive(true);

        AddPageHistory(newPage); // use the improved dynamic version
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

    public void HandleLoaderPage(bool val)
    {
        LoaderPage.SetActive(val);
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

        yield return new WaitForSeconds(1f);
        Debug.Log("Child count after refresh: " + scrollSnap.ChildObjects.Length);
        int previousIndex = listOfObjects[6].transform.GetSiblingIndex();
        listOfObjects[6].transform.SetSiblingIndex(0);
        listOfObjects[6].SetActive(true);
        listOfObjects[6].transform.SetSiblingIndex(previousIndex);
        listOfObjects[6].SetActive(false);
        //StartCoroutine(RefreshOldData);
    }
    public void ClearOldList()
    {
        List<GameObject> gameObjectsList = new List<GameObject>(scrollSnap.ChildObjects);
        foreach (GameObject obj in scrollSnap.ChildObjects)
        {
            if (obj != null)
            {
                Debug.Log("CLEAR child obj: " + obj.name);
                Destroy(obj);
            }
        }

        gameObjectsList.Clear();
        scrollSnap.ChildObjects = gameObjectsList.ToArray();
        Debug.Log("Child count: " + scrollSnap.ChildObjects.Length);
    }

    public void ResetchildObjLength()
    {
        //scrollSnap.ChildObjects = new GameObject[0];
        scrollSnap.ChildObjects = null;
    }
}
