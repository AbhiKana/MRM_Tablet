using UnityEngine;
using UnityEngine.UI;

public class CategoryToggle : MonoBehaviour
{
   public int categoryId; // Set this in the Inspector
    public MarbleFilterationController filterManager;

    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void Start()
    {
        filterManager = FindAnyObjectByType<MarbleFilterationController>();
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            filterManager.FilterByCategory(categoryId);
        }
    }
}
