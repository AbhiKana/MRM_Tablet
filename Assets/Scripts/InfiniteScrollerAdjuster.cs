using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using UnityEngine;

public class InfiniteScrollerAdjuster : MonoBehaviour
{
    [SerializeField] CircularScrollingList circularScrollingList;
    [SerializeField] GetAllMarbles getAllMarbles;
    [SerializeField] GameObject tilePrefab;

    [SerializeField] float minSpacing = 50f;
    [SerializeField] float maxSpacing = 200f;

    RectTransform rectTransform;
    
    int numOfbox;
    float prefabWidth;
    float spacing;

    [SerializeField] List<ShowMarbleDetails> listOfAllMarbles = new List<ShowMarbleDetails>();
    
    private void Start()
    {
        SetCircularList();

        if(getAllMarbles != null) 
            getAllMarbles.OnAllMarbleDataLoaded.AddListener(SetCircularList);
    }

    private void SetCircularList()
    {
        rectTransform = circularScrollingList.GetComponent<RectTransform>();
        numOfbox = getAllMarbles.allMarbles.marbleDetails.Count /*+7*/;
        circularScrollingList.BoxSetting._numOfBoxes = numOfbox;

        AdjustSpacing();
        AdjustPositionAndSize();
        circularScrollingList.GenerateBoxesAndArrange();

        if (numOfbox == 1)
            circularScrollingList.enabled = false;
    } 

    public void GetAllAvailableMarbles()
    {
        StartCoroutine(StoreMarblesInList());
    }

    IEnumerator StoreMarblesInList()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Transform transform in circularScrollingList.transform)
        {
            var showDetails = transform.GetComponent<ShowMarbleDetails>();
            if (!listOfAllMarbles.Contains(showDetails))
            {
                listOfAllMarbles.Add(showDetails);
            }
        }
        getAllMarbles.SetMarbleDetails(listOfAllMarbles);
    }
    void AdjustSpacing()
    {
        spacing = Mathf.Lerp(maxSpacing, minSpacing, numOfbox / 10f);
        prefabWidth = tilePrefab.GetComponent<RectTransform>().sizeDelta.x + spacing;
    }
    void AdjustPositionAndSize()
    {
        rectTransform.sizeDelta = new Vector2(prefabWidth * numOfbox, rectTransform.sizeDelta.y);

        if (numOfbox % 2 == 0 && numOfbox > 3)
        {
            float shift = prefabWidth / 2f;
            rectTransform.anchoredPosition = new Vector2(shift, rectTransform.anchoredPosition.y);
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(0f, rectTransform.anchoredPosition.y);
        }
    }
}
