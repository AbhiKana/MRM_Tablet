using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RectangleTween : MonoBehaviour
{
    public float Duration = 0.5f;
   // public float BoxDelay = 0.1f;
    public float yvalue = 0f;
    // Start is called before the first frame update
    public RectTransform RectImage;
    public Vector3 Initialpos;
    void Awake()
    {
        RectImage = this.GetComponent<RectTransform>();
        Initialpos = this.GetComponent<RectTransform>().anchoredPosition3D;
    }
    public void RectMoveUp()
    {
        RectImage.DOAnchorPos3DY(yvalue, Duration);
    }

    public void RectMoveDown()
    {
        RectImage.DOAnchorPos3D(Initialpos, Duration);       
    }
}
