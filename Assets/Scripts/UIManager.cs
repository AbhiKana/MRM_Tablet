using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject[] panels;

    private GameObject activePanel = null;

    private void Start()
    {
        foreach (GameObject p in panels)
        {
            p.SetActive(false);
        }
        panels[0].gameObject.SetActive(true);
    }

    public void TogglePanel(GameObject panel)
    {
        if (activePanel == panel)
        {
            //panel.SetActive(false);
            activePanel = null;
            return;
        }

        foreach (GameObject p in panels)
        {
            p.SetActive(false);
        }

        panel.SetActive(true);
        activePanel = panel;
    }
}
