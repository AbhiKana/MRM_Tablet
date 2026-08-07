using AirFishLab.ScrollingList;
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
    [SerializeField] Transform inventoryContainer; // Assign the parent of all marble items
    [SerializeField] bool isCircularList = false;

    public UnityEvent<int> OnCategorySelection = new UnityEvent<int>();
    public UnityEvent OnCategoryAlreadySelected = new UnityEvent();

    public void FilterByCategory(int selectedCategory)
    {
        CircularScrollingList csList = inventoryContainer.GetComponent<CircularScrollingList>();

        if (csList != null && csList.ListBoxes.Length < getAllMarbles.allMarbles.getMarblesList.marbleDetails.Count)
        {
            OnCategoryAlreadySelected?.Invoke();
            Debug.Log("Category Already Selected");
            SetParenting(MarbleFilterType.Category, selectedCategory);
        }
        else
        {
            Debug.Log("Category Selection First time");
            SetParenting(MarbleFilterType.Category, selectedCategory);
        }
    }

    private void SetParenting(MarbleFilterType type, int selectedCategory)
    {
        foreach (var child in marbleLoader.listOfMarblesInventory.listOfAllMarbles)
        {
            SetActiveStatus(type, selectedCategory, child);
        }

        if (isCircularList)
            OnCategorySelection.Invoke(selectedCategory);
    }

    private void SetActiveStatus(MarbleFilterType type, int selectedCategory, ShowMarbleDetails child)
    {
        Debug.Log(child.name);
        if (child != null)
        {
            switch (type)
            {
                case MarbleFilterType.Category:
                    if (selectedCategory == 0)
                    {
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.gameObject.SetActive(child.categoryID == selectedCategory);
                    }
                    break;
                case MarbleFilterType.Tile:
                    Debug.Log(child.name + child.tileID + (child.tileID == selectedCategory));
                    child.gameObject.SetActive(child.tileID == selectedCategory);
                    break;
            }
        }
    }

    public void SearchMarbleByName(TMPro.TMP_InputField tMP_Input)
    {
        string name = tMP_Input.text;
        if (string.IsNullOrWhiteSpace(name))
        {
            SetParenting(MarbleFilterType.Category, 0);
            return;
        }
        marbleLoader.listOfMarblesInventory.listOfAllMarbles.ForEach(m => {m.gameObject.SetActive(false); });
        var myKeys = FuzzyMatcher.SearchKeysByValue(marbleLoader.marbleKeyValue, name, maxTyposPerWord: 2);
        if (myKeys == null || myKeys.Count <= 0) return;
        foreach (var key in myKeys)
        {          
            var child = marbleLoader.listOfMarblesInventory.listOfAllMarbles.FirstOrDefault(x => x.tileID == key);           
            SetActiveStatus(MarbleFilterType.Tile, key, child);
        }
    }
}
