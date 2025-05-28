using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarbleFilterationController : MonoBehaviour
{
    [SerializeField] Transform inventoryContainer; // Assign the parent of all marble items
    [SerializeField] Transform CarouselMenu;
    public void FilterByCategory(int selectedCategory)
    {
        foreach (Transform child in inventoryContainer)
        {
            SetActiveStatus(selectedCategory, child);
        }

        foreach (Transform child in CarouselMenu)
        {
            SetActiveStatus(selectedCategory, child);
        }
    }

    private static void SetActiveStatus(int selectedCategory, Transform child)
    {
        ShowMarbleDetails item = child.GetComponent<ShowMarbleDetails>();
        if (item != null)
        {
            if (selectedCategory == 0)
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(item.categoryID == selectedCategory);
        }
    }
}
