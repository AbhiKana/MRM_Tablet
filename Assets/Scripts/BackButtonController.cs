using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonController : MonoBehaviour
{
    public GameObject firstPage;
    public GameObject HeaderPage;
    [SerializeField] private List<GameObject> pageHistory = new List<GameObject>();

    void Start()
    {
        firstPage.SetActive(true);
        pageHistory.Add(firstPage);
    }

    public void OpenPage(GameObject newPage)
    {
        if (pageHistory.Count > 0)
        {
            GameObject currentPage = pageHistory[pageHistory.Count - 1];
            currentPage.SetActive(false);
        }

        newPage.SetActive(true);
        pageHistory.Add(newPage);
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
}
