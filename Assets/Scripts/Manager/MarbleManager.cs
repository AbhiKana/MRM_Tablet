using System.Collections.Generic;
using UnityEngine;

public static class MarbleManager
{
    public static void RemoveMarbleFromWishlist(string marbleName)
    {
        StoreMarbleDetails storeMarbleDetails = Object.FindObjectOfType<StoreMarbleDetails>();
        if (storeMarbleDetails == null)
        {
            Debug.LogWarning("StoreMarbleDetails not found.");
            return;
        }

        List<SpecificMarbleDetails> wishList = storeMarbleDetails.WishListMarble;

        foreach (SpecificMarbleDetails details in wishList)
        {
            if (details.marble_name == marbleName)
            {
                Debug.Log($"Removing marble object: {details.marble_name}");
                storeMarbleDetails.list.Remove(details);
                break;
            }
        }
    }

    public static void RemoveMarbleFromSelectionMenu(string marbleName)
    {
        SelectionPanelController selectionPanelController = Object.FindObjectOfType<SelectionPanelController>();
        if (selectionPanelController == null)
        {
            Debug.LogWarning("Selection Panel Controller not found.");
            return;
        }

        List<SelectionTileDetails> selectionTileDetails = selectionPanelController.availableTile;
        foreach (SelectionTileDetails details in selectionTileDetails)
        {
            if (details.tileNameStr == marbleName)
            {
                Debug.Log($"Removing marble object: {details.tileNameStr}");
                selectionPanelController.availableTile.Remove(details);
                break;
            }
        }
    }
}
