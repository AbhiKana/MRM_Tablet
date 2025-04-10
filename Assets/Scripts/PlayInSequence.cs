using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayInSequence : MonoBehaviour
{
    public bool getIniPos;
    [SerializeField] RectTransform[] inOut;
    [SerializeField] Vector2[] FromPos;
    [SerializeField] Vector2[] initialPos;
    [SerializeField] float duration;

    public UnityEvent animEvent;

    private void Awake()
    {
        if (getIniPos == true)
        {
            for (int i = 0; i < inOut.Length; ++i)
            {
                initialPos[i] = inOut[i].anchoredPosition;
            }
        }
    }

    public void StartAnim()
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < inOut.Length; ++i)
        {
            inOut[i].DOKill();
            sequence.Join(inOut[i].GetComponent<RectTransform>().DOAnchorPos(initialPos[i], duration * (i + 1)).From(FromPos[i]));
        }
        sequence.Play();
        sequence.OnComplete(() =>
        {
            animEvent?.Invoke();
        });
    }

    public void StartAnimReverse()
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < inOut.Length; ++i)
        {
            inOut[i].DOKill();
            sequence.Join(inOut[i].GetComponent<RectTransform>().DOAnchorPos(FromPos[i], duration * (i + 1)).From(initialPos[i]));
        }
        sequence.Play();
        sequence.OnComplete(() =>
        {
            animEvent?.Invoke();
        });
    }
}
