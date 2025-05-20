using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SelectionTileDetails : MonoBehaviour
{
    [SerializeField] UI_Manager manager;
    
    [SerializeField] RawImage tileImage;
    [SerializeField] TextMeshProUGUI tileName;

    [SerializeField] Button viewButton;
    [SerializeField] Button DeleteButton;

    public string tileNameStr => tileName.text;

   public int pageNum;

    private void Awake()
    {
        manager = FindObjectOfType<UI_Manager>();
    }

    private void Start()
    {
        viewButton.onClick.AddListener(() =>
        {
            EnableMrMDetailsObject();
        });

        DeleteButton.onClick.AddListener(() =>
        {
            RemoveMarble();
        });
    }

    private void EnableMrMDetailsObject()
    {
        manager.OpenPage(6);
        
        if(manager.scrollSnap != null)
            manager.scrollSnap.ChangePage(pageNum);
    }

    private void RemoveMarble()
    {
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
                if(mRM_Details != null && mRM_Details.MarbleName.text == name)
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
