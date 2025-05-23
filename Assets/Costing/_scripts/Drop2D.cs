using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop2D : MonoBehaviour, IDropHandler
{
    public bool OnDropOnce = false;
    // public Sprite BigRect;
    public Button DropButton;
    public enum AnchorPreset
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        Stretch
    }

    [Header("Anchor Settings")]
    public AnchorPreset anchorPreset = AnchorPreset.Stretch;
    public Vector2 positionOffset = Vector2.zero;
    void Start()
    {
        AddDropListner();
    }

    public void AddDropListner()
    {
        DropButton.onClick.AddListener(() => { BigScreenRoomsControl.Bigroom.ClickonAddMarble(); });
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.name != "Scroll View Horizontal")
        { // if drop once is false then drop only once
            if (!OnDropOnce)
            {
                print(OnDropOnce + " >> " + eventData.pointerDrag.name);
                int marbleid = droppedObject.GetComponent<DragMarble>().DragMarbleid;
                // check if marble id already added in one room or no
                if (!BigScreenRoomsControl.Bigroom.CheckMarbleExistsInRoom(marbleid))
                {   //eventData.pointerDrag.GetComponent<RectTransform>().transform.position = transform.position;
                    droppedObject.GetComponent<DragMarble>().isDroptarget = true;
                    //droppedObject.GetComponent<CanvasGroup>().enabled = false;
                    OnDropOnce = true;

                    droppedObject.transform.SetParent(transform);
                    // Get or add RectTransform
                    RectTransform droppedRT = droppedObject.GetComponent<RectTransform>();
                    droppedRT.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    if (droppedRT == null) return;
                    // Apply the selected anchor preset
                    ApplyAnchorPreset(droppedRT);
                    // Apply position offset
                    droppedRT.anchoredPosition = new Vector2(0, -40f);
                    //==============
                    droppedObject.GetComponent<DragMarble>().RemoveBtn.gameObject.SetActive(true);
                    //eventData.pointerDrag.GetComponent<Image>().sprite = BigRect;
                    BigScreenRoomsControl.Bigroom.ShowMarbleDimensionOverlay(marbleid, droppedObject);
                    // remove add listener
                    DropButton.onClick.RemoveAllListeners();
                }
            }
        }
        // Debug.Log("drop " + eventData.pointerDrag.name);
    }

    private void ApplyAnchorPreset(RectTransform rt)
    {
        switch (anchorPreset)
        {
            case AnchorPreset.TopLeft:
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                break;

            case AnchorPreset.TopCenter:
                rt.anchorMin = new Vector2(0.5f, 1);
                rt.anchorMax = new Vector2(0.5f, 1);
                rt.pivot = new Vector2(0.5f, 1);
                break;

            case AnchorPreset.TopRight:
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(1, 1);
                break;

            case AnchorPreset.MiddleLeft:
                rt.anchorMin = new Vector2(0, 0.5f);
                rt.anchorMax = new Vector2(0, 0.5f);
                rt.pivot = new Vector2(0, 0.5f);
                break;

            case AnchorPreset.MiddleCenter:
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                break;

            case AnchorPreset.MiddleRight:
                rt.anchorMin = new Vector2(1, 0.5f);
                rt.anchorMax = new Vector2(1, 0.5f);
                rt.pivot = new Vector2(1, 0.5f);
                break;

            case AnchorPreset.BottomLeft:
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 0);
                rt.pivot = new Vector2(0, 0);
                break;

            case AnchorPreset.BottomCenter:
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(0.5f, 0);
                rt.pivot = new Vector2(0.5f, 0);
                break;

            case AnchorPreset.BottomRight:
                rt.anchorMin = new Vector2(1, 0);
                rt.anchorMax = new Vector2(1, 0);
                rt.pivot = new Vector2(1, 0);
                break;

            case AnchorPreset.Stretch:
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = Vector2.zero;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                break;
        }
    }
}
