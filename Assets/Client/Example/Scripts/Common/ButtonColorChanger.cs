using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    
    public bool IsSelected = false;
    [SerializeField] Color selectedColor;
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeColor);
    }

    public void ChangeColor()
    {
        IsSelected = !IsSelected;
        if (IsSelected)
        {
            button.targetGraphic.color = selectedColor;
        }
        else
        {
            button.targetGraphic.color = Color.white;
        }

    }
}
