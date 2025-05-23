using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Drawing;

public class PopUPTween : MonoBehaviour
{
    public CanvasGroup ImageCanvas;
    // Start is called before the first frame update
    void Awake()
    {
        ImageCanvas = GetComponent <CanvasGroup>();
    }

    public void ObjectfadeIn()
    {
        ImageCanvas.interactable = true;
        ImageCanvas.DOFade(1, 0.5f).OnComplete(Objectfadeout);
    }

    public void Objectfadeout()
    {
        ImageCanvas.DOFade(0, 0.5f).SetDelay(0.7f).OnComplete(() => { ImageCanvas.interactable = false; });
    }
}
