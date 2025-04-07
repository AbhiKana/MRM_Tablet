using UnityEngine;
using UnityEngine.Events;

public class ObjectActiveController : MonoBehaviour
{
    [SerializeField] UnityEvent onEnableEvent;
    [SerializeField] UnityEvent onDisableEvent;

    private void OnEnable()
    {
        onEnableEvent?.Invoke();
    }

    private void OnDisable()
    {
        onDisableEvent?.Invoke();
    }
}
