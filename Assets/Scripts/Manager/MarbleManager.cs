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

        List<SpecificMarbleDetails> wishList = storeMarbleDetails.list;

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

        UpdateToggleValue(marbleName);
    }

    public static void UpdateToggleValue(string marbleName)
    {
        SyncMarbleDetails syncMarbleDetails = Object.FindObjectOfType<SyncMarbleDetails>();
        for (int i = 0; i < syncMarbleDetails.gridMarbles.Length; i++)
        {

            if (syncMarbleDetails.gridMarbles[i].marbleName == marbleName)
            {
                syncMarbleDetails.SetWishlistValue(syncMarbleDetails.gridMarbles[i],false);
                syncMarbleDetails.SetWishlistValue(syncMarbleDetails.marqueeMarbles[i], false);
            }
        }
        //syncMarbleDetails.SyncMarbleData();
    }
}
