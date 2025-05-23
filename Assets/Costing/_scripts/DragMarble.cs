using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DragMarble : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{ // Start is called before the first frame update

    public RawImage rawImage;
    public TextMeshProUGUI _Mname;

    private RectTransform thisRect;
    public Canvas canvas;
    public bool isDroptarget = false;
    public int DragMarbleid = 0;
    public Button RemoveBtn;
    public GameObject ThisDragObject;
    //  public GameObject DragImageHolder;
    public GameObject parentPanel;
    public CanvasGroup Dragcanvasgroup;
    void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        parentPanel = GameObject.FindGameObjectWithTag("DragPanel");
        canvas = GameObject.FindGameObjectWithTag("ControlCanvas").GetComponent<Canvas>();
        Dragcanvasgroup = GetComponent<CanvasGroup>();

    }
    void Start()
    {
        RemoveBtn.gameObject.SetActive(false);

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDroptarget)
        {          
            // thisRect.GetComponent<Image>().raycastTarget = false;
            GameObject go = Instantiate(ThisDragObject, eventData.position, Quaternion.identity);
            go.transform.SetParent(parentPanel.transform, false);
            eventData.pointerDrag = go; //assign instantiated element  
        }
        else
        {
            eventData.pointerDrag = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDroptarget)
        {
            Dragcanvasgroup.blocksRaycasts = false;
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, eventData.position, canvas.worldCamera, out position);
            eventData.pointerDrag.transform.position = canvas.transform.TransformPoint(position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Dragcanvasgroup.blocksRaycasts = true;
        // eventData.pointerDrag.GetComponent<Image>().raycastTarget = true;
        if (!isDroptarget)
        {           
            Destroy(this.gameObject);
        }
    }


    public void OnRemoveClick()
    {
        // print(thisRect.name);
        // to set parent back to drag
        thisRect.transform.parent.GetComponent<Drop2D>().OnDropOnce = false;
        //  Dragcanvasgroup.blocksRaycasts = false;
        BigScreenRoomsControl.Bigroom.RemoveMarbleDataInRoom(DragMarbleid);
        // RemoveBtn.gameObject.SetActive(false);      
        Destroy(this.gameObject);
        Destroy(this.gameObject.transform.parent.gameObject);
    }

  
}
