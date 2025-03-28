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
        ScrollPosAdjuster();
        ScrollSizeAdjuster();

        if (numOfbox == 1)
            circularScrollingList.enabled = false;
    }

    private void ScrollSizeAdjuster()
    {
        circularScrollingList.BoxSetting._numOfBoxes = numOfbox;
        prefabWidth = tilePrefab.GetComponent<RectTransform>().sizeDelta.x + space;
        rectTransform.sizeDelta = new Vector2(prefabWidth * numOfbox, rectTransform.sizeDelta.y);
        circularScrollingList.GenerateBoxesAndArrange();
    }

    private void ScrollPosAdjuster()
    {
        rectTransform = circularScrollingList.GetComponent<RectTransform>();
        if (numOfbox % 2 == 0 && numOfbox > 2)
        {
            rectTransform.anchoredPosition = new Vector3(300f, rectTransform.anchoredPosition.y);
        }
    }
}
