using System.Collections.Generic;
using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.Demo;
using Unity.VisualScripting;
using UnityEngine;

public class CircularMarbleFilterization : MonoBehaviour
{
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] LateInitialization lateInitialization;
    [SerializeField] InfiniteScrollerAdjuster infiniteScrollerAdjuster;
    [SerializeField] MarbleFilterationController controller;

    [SerializeField] Transform UnfilteredObjectParent;
    /*public void FilterByCategory(int selectedCategory)
    {
        foreach (Transform child in inventoryContainer)
        {
            SetActiveStatus(selectedCategory, child);
        }
    }

    private void SetActiveStatus(int selectedCategory, Transform child)
    {
        ShowMarbleDetails item = child.GetComponent<ShowMarbleDetails>();
        if (item != null)
        {
            if (selectedCategory == 0)
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(item.categoryID == selectedCategory);
        }
    }*/

    private void Start()
    {
        controller.OnCategorySelection.AddListener(ModifyCircularList);
        controller.OnCategoryAlreadySelected.AddListener(ResetParent);
    }

    public void  ModifyCircularList(int selectedCategory)
    {
        List<ListBox> tempList = new List<ListBox>();
        //int abc = 0;
        foreach (Transform child in circularScrollingList.transform)
        {
            //abc++;
            //Debug.Log(abc);
            if (!child.gameObject.activeSelf)
            {
                ListBox l = child.GetComponent<ListBox>();
                tempList.Add(l);
            }
        }


        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i].transform.SetParent(UnfilteredObjectParent, false);
            circularScrollingList._listBoxes.Remove(tempList[i]);
        }

        ReSize();
        //Invoke(nameof(ReSize), 1f);
    }

    void ReSize()
    {

        infiniteScrollerAdjuster.ModifyCircularList();
        lateInitialization._numOfBoxes = circularScrollingList.ListBoxes.Length;
        lateInitialization.InitializeTheList();
    }
    /*private void ChnageChildParent(List<ListBox> tempList, Transform UnfilterParent)
    {
        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i].transform.SetParent(UnfilterParent, false);
            circularScrollingList._listBoxes.Remove(tempList[i]);
        }
    }*/

    private void ResetParent()
    {
        List<ListBox> tempList = new List<ListBox>();
        foreach (Transform child in UnfilteredObjectParent.transform)
        {
            ListBox l = child.GetComponent<ListBox>();
            tempList.Add(l);
            circularScrollingList._listBoxes.Add(l);
        }

        for(int i = 0;i < tempList.Count;i++)
        {
            tempList[i].transform.SetParent(circularScrollingList.transform, true);
            tempList[i].gameObject.SetActive(true);
        }
    }

    private void ListResizing(int selectedCategory)
    {
        if (selectedCategory == 0)
        {
            foreach (Transform child in UnfilteredObjectParent.transform)
            {
                ListBox l = child.GetComponent<ListBox>();
                if (l != null)
                {
                    circularScrollingList._listBoxes.Add(l);
                    child.SetParent(circularScrollingList.transform, true);
                    child.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            // Step 3: Filter only matching categoryID and restore them
            foreach (Transform child in UnfilteredObjectParent.transform)
            {
                ShowMarbleDetails s = child.GetComponent<ShowMarbleDetails>();
                ListBox l = child.GetComponent<ListBox>();

                if (s.categoryID == selectedCategory)
                {
                    circularScrollingList._listBoxes.Add(l);
                    child.SetParent(circularScrollingList.transform, true);
                    child.gameObject.SetActive(true);
                }
            }
        }

        infiniteScrollerAdjuster.ModifyCircularList();
        lateInitialization._numOfBoxes = circularScrollingList.ListBoxes.Length;
        lateInitialization.InitializeTheList();
    }
}
