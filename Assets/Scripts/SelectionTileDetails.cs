using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class SelectionTileDetails : MonoBehaviour
{
    TileDetailsList tileDetailsList;
    [SerializeField] AssignPageNumbers assignPageNumbers;

    [SerializeField] UI_Manager manager;

    [SerializeField] RawImage tileImage;
    [SerializeField] TextMeshProUGUI tileName;

    [SerializeField] Button viewButton;
    [SerializeField] Button DeleteButton;

    public string tileNameStr => tileName.text;
    public int pageNum;

    public UnityEvent OnDelete = new UnityEvent();

    private void Start()
    {
        assignPageNumbers = transform.parent.GetComponent<AssignPageNumbers>();
        
        manager = FindObjectOfType<UI_Manager>();
        tileDetailsList = FindObjectOfType<TileDetailsList>();

        OnDelete.AddListener(updateImageDownloadCount);

        viewButton.onClick.AddListener(() =>
        {
            UpdatePage();
            EnableMrMDetailsObject();
        });

        DeleteButton.onClick.AddListener(() =>
        {
            RemoveMarble();
        });
    }

    void updateImageDownloadCount()
    {
        if(tileDetailsList.noof_imagedownload > 0)
            tileDetailsList.noof_imagedownload--;
    }

    void UpdatePage()
    {
        if (assignPageNumbers != null)
            assignPageNumbers.UpdatePageNum();
    }
    private void EnableMrMDetailsObject()
    {
        manager.OpenPage(6);

        if (manager.scrollSnap != null)
            manager.scrollSnap.ChangePage(pageNum);
    }

    private void RemoveMarble()
    {
        OnDelete?.Invoke();
        MarbleManager.RemoveMarbleFromWishlist(tileName.text);
        MarbleManager.RemoveMarbleFromSelectionMenu(tileName.text);
        RemoveObjectFromArray(tileName.text);
        Destroy(this.gameObject);
    }


    private void RemoveObjectFromArray(string name)
    {
        if (manager.scrollSnap.ChildObjects.Length < 0)
        {
            Debug.Log("ADD child obj");
        }
        else
        {
            List<GameObject> gameObjectsList = new List<GameObject>(manager.scrollSnap.ChildObjects);

            foreach (GameObject obj in gameObjectsList)
            {
                MRM_Details mRM_Details = obj.GetComponent<MRM_Details>();
                if (mRM_Details != null && mRM_Details.MarbleName.text == name)
                {
                    Debug.Log("Remvoe child object: " + mRM_Details.MarbleName.text);
                    Destroy(obj);
                    gameObjectsList.Remove(obj);
                    break;
                }
            }
            manager.scrollSnap.ChildObjects = gameObjectsList.ToArray();
        }
    }


    public void SetTileDetails(Texture tileImg, string name)
    {
        tileImage.texture = tileImg;
        tileName.text = name;
    }
}
