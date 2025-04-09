using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuBar : MonoBehaviour
{
    [Header("Menu Settings")]
    [Tooltip("The speed at which the menu opens/closes.")]
    public float animationDuration = 0.5f;

    [Tooltip("The button that toggles the menu.")]
    public Button menuButton, closeButton;

    [SerializeField] private RectTransform rectTransform;
    private float closedPositionX;
    private float openPositionX;
    private bool isOpen = false;

    [Tooltip("The easing type for the animation.")]
    public Ease easeType = Ease.OutQuad;

    private void Awake()
    {
        closedPositionX = -rectTransform.rect.width;
        openPositionX = 0;

        rectTransform.anchoredPosition = new Vector2(closedPositionX, rectTransform.anchoredPosition.y);

        if (menuButton != null)
            menuButton.onClick.AddListener(OpenMenu);

        if(closeButton != null)
            closeButton.onClick.AddListener(CloseMenu);
    }

    public void ToggleMenu()
    {
        if (isOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (isOpen) return;

        rectTransform.DOAnchorPosX(openPositionX, animationDuration)
            .SetEase(easeType)
            .OnComplete(() => isOpen = true);
    }

    public void CloseMenu()
    {
        if (!isOpen) return;

        rectTransform.DOAnchorPosX(closedPositionX, animationDuration)
            .SetEase(easeType)
            .OnComplete(() => isOpen = false);
    }

    public void ForceCloseMenu()
    {
        if (!isOpen) return;

        // Immediately close the menu without animation
        rectTransform.anchoredPosition = new Vector2(closedPositionX, rectTransform.anchoredPosition.y);
        isOpen = false;
    }
}
