using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    [SerializeField] Button left, right;
    [SerializeField] ScrollRect scrollRect;

    [SerializeField] int noofclicks;
    float offset;
    [SerializeField] float duration;

    [SerializeField] float minX, maxX;
    bool IsScrolling;

    private void Start()
    {
        offset = 1 / (float)noofclicks;
        InitialScrollPos();
    }

    public void InitialScrollPos()
    {
        left.interactable = false;
        right.interactable = true;
        scrollRect.normalizedPosition = Vector2.zero;
        RightButtonInteraction();
    }

    public void RightButtonInteraction()
    {
        if (scrollRect.horizontalNormalizedPosition >= maxX)
            right.interactable = false;
        else
            right.interactable = true;
    }
    public void LeftButtonInteraction()
    {
        if (scrollRect.horizontalNormalizedPosition <= minX)
            left.interactable = false;
        else
            left.interactable = true;
    }
    public void ScrollRight()
    {
        if (scrollRect.normalizedPosition.x < 1 && !IsScrolling)
        {
            Vector2 startPos = scrollRect.normalizedPosition;
            Vector2 targetPos = startPos + Vector2.one * offset;
            StartCoroutine(SmoothScroll(duration, startPos,targetPos));
        }
    }

    public void ScrollLeft()
    {
        if (scrollRect.normalizedPosition.x > 0 && !IsScrolling)
        {
            Vector2 startPos = scrollRect.normalizedPosition;
            Vector2 targetPos = startPos - Vector2.one * offset;
            StartCoroutine(SmoothScroll(duration, startPos, targetPos));
        }
    }
    private IEnumerator SmoothScroll(float duration, Vector2 startPos, Vector2 targetPos)
    {
        IsScrolling = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scrollRect.normalizedPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }
        scrollRect.normalizedPosition = targetPos;
        IsScrolling = false;
    }
}

