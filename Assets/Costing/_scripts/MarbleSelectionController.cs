using UnityEngine;
using UnityEngine.Events;

public class MarbleSelectionController : MonoBehaviour
{
    [SerializeField] GameObject marbleSelectionPrompt;
    [SerializeField] StoreMarbleDetails storeMarbleDetails;
    public UnityEvent OnMarbleSelectionClicked;

    public void GoToMarbleSelection()
    {
        if(storeMarbleDetails.listOfMarbleDetails.Count > 0)
        {
            OnMarbleSelectionClicked?.Invoke();
        }
        else
        {
            marbleSelectionPrompt.SetActive(true);
        }
    }
}
