using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using UnityEngine;

public class InfiniteScrollerAdjuster : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] int space;
    [SerializeField] float prefabWidth;
    [SerializeField] int numOfbox;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (numOfbox%2 == 0)
        {
            rectTransform.anchoredPosition = new Vector3(300f, rectTransform.anchoredPosition.y);
        }

        circularScrollingList.BoxSetting._numOfBoxes = numOfbox;
        prefabWidth = tilePrefab.GetComponent<RectTransform>().sizeDelta.x + space;
        
        rectTransform.sizeDelta = new Vector2(prefabWidth * numOfbox, rectTransform.sizeDelta.y);
        //rectTransform.rect.width = prefabWidth * numOfbox;
    
        circularScrollingList.GenerateBoxesAndArrange();
    }
}
