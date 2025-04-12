using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionTileDetails : MonoBehaviour
{
    [SerializeField] UI_Manager manager;
    [SerializeField] GameObject TileDetailsList;

    [SerializeField] TileDetailsList selectionPanelController;

    [SerializeField] RawImage tileImage;
    [SerializeField] TextMeshProUGUI tileName;

    [SerializeField] Button viewButton;
    [SerializeField] Button DeleteButton;

    private void Awake()
    {
        manager = FindObjectOfType<UI_Manager>();
        TileDetailsList = GameObject.Find("TileDetailsList");

        selectionPanelController = TileDetailsList.GetComponent<TileDetailsList>();
    }

    private void Start()
    {
        viewButton.onClick.AddListener(() =>
        {
            EnableMrMDetaqilsObject();
        });
    }

    private void EnableMrMDetaqilsObject()
    {
        manager.OpenPage(6);
        //selectionPanelController.SpawnMarbleDetailsList();
    }

    public void SetTileDetails(Texture tileImg, string name)
    {
        tileImage.texture = tileImg;
        tileName.text = name;
    }
}
