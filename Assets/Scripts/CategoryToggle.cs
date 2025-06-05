using UnityEngine;
using UnityEngine.UI;

public class CategoryToggle : MonoBehaviour
{
    public int categoryId;
    [SerializeField] MarbleFilterationController filterManager;
    [SerializeField] MarbleFilterationController circularMarbleFilterization;

    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void Start()
    {
        filterManager = GameObject.FindGameObjectWithTag("FilterManager").GetComponent<MarbleFilterationController>();
        circularMarbleFilterization = GameObject.FindGameObjectWithTag("CircularFilter").GetComponent<MarbleFilterationController>();
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            filterManager.FilterByCategory(categoryId);
            circularMarbleFilterization.FilterByCategory(categoryId);
        }
    }
}
