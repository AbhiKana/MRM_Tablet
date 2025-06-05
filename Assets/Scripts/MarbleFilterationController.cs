using System.Threading.Tasks;
using AirFishLab.ScrollingList;
using UnityEngine;
using UnityEngine.Events;

public class MarbleFilterationController : MonoBehaviour
{
    GetAllMarbles getAllMarbles;
    [SerializeField] Transform inventoryContainer; // Assign the parent of all marble items
    [SerializeField] bool isCircularList = false;

    public UnityEvent<int>  OnCategorySelection = new UnityEvent<int>();
    public UnityEvent OnCategoryAlreadySelected = new UnityEvent();

    private void Start()
    {
        getAllMarbles = FindObjectOfType<GetAllMarbles>();
    }
    public void FilterByCategory(int selectedCategory)
    {
        CircularScrollingList csList = inventoryContainer.GetComponent<CircularScrollingList>();

        if (csList != null && csList.ListBoxes.Length < getAllMarbles.allMarbles.getMarblesList.marbleDetails.Count)
        {
            OnCategoryAlreadySelected?.Invoke();
            Debug.Log("Category Already Selected");
            SetParenting(selectedCategory);
        }
        else
        {
            SetParenting(selectedCategory);
        }
    }

    private void SetParenting(int selectedCategory)
    {
        foreach (Transform child in inventoryContainer)
        {
            SetActiveStatus(selectedCategory, child);
        }

        if (isCircularList)
            OnCategorySelection.Invoke(selectedCategory);
    }

    private void SetActiveStatus(int selectedCategory, Transform child)
    {
        ShowMarbleDetails item = child.GetComponent<ShowMarbleDetails>();
        if (item != null)
        {
            if (selectedCategory == 0)
            {
                //Debug.LogError(item.gameObject.name);
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(item.categoryID == selectedCategory);
            }
        }
    }
}
