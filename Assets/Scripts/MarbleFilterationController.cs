using AirFishLab.ScrollingList;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum MarbleFilterType
{
    Category,
    Tile
}

public class MarbleFilterationController : MonoBehaviour
{
    [SerializeField] private GetAllMarbles getAllMarbles;
    [SerializeField] private MarbleLoader marbleLoader;
    [SerializeField] private VirtualMarbleGrid virtualGrid;

    [SerializeField] bool isCircularList = false;

    public UnityEvent<int> OnCategorySelection = new UnityEvent<int>();
    public UnityEvent OnCategoryAlreadySelected = new UnityEvent();

    public void FilterByCategory(int selectedCategory)
    {
        CircularScrollingList csList = FindObjectOfType<CircularScrollingList>();
        if (csList != null && getAllMarbles != null && getAllMarbles.allMarbles.getMarblesList != null)
        {
            if (csList.ListBoxes.Length < getAllMarbles.allMarbles.getMarblesList.marbleDetails.Count)
            {
                OnCategoryAlreadySelected?.Invoke();
            }
        }

        ApplyFilter(MarbleFilterType.Category, selectedCategory);

        if (isCircularList)
            OnCategorySelection.Invoke(selectedCategory);
    }

    private void ApplyFilter(MarbleFilterType type, int selectedValue)
    {
        // 1. Safety check: Make sure the data and the grid actually exist before filtering
        if (getAllMarbles == null || getAllMarbles.allMarbles == null || getAllMarbles.allMarbles.getMarblesList == null || virtualGrid == null)
        {
            Debug.LogWarning("Filter called before data or grid was ready!");
            return;
        }

        var originalData = getAllMarbles.allMarbles.getMarblesList.marbleDetails;
        List<MarbleDetail> filteredData = new List<MarbleDetail>();

        if (type == MarbleFilterType.Category)
        {
            if (selectedValue == 0)
            {
                filteredData = new List<MarbleDetail>(originalData);
            }
            else
            {
                foreach (var marble in originalData)
                {
                    if (marble.category_id == selectedValue)
                        filteredData.Add(marble);
                }
            }
        }
        else if (type == MarbleFilterType.Tile)
        {
            foreach (var marble in originalData)
            {
                if (marble.id == selectedValue)
                    filteredData.Add(marble);
            }
        }

        virtualGrid.InitializeGrid(filteredData);
    }

    public void SearchMarbleByName(TMPro.TMP_InputField tMP_Input)
    {
        string name = tMP_Input.text;

        if (string.IsNullOrWhiteSpace(name))
        {
            ApplyFilter(MarbleFilterType.Category, 0);
            return;
        }

        var originalData = getAllMarbles.allMarbles.getMarblesList.marbleDetails;
        List<MarbleDetail> filteredData = new List<MarbleDetail>();

        var myKeys = FuzzyMatcher.SearchKeysByValue(marbleLoader.marbleKeyValue, name, maxTyposPerWord: 2);

        if (myKeys != null && myKeys.Count > 0)
        {
            foreach (var marble in originalData)
            {
                if (myKeys.Contains(marble.id))
                    filteredData.Add(marble);
            }
        }

        virtualGrid.InitializeGrid(filteredData);
    }
}