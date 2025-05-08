using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(TMP_InputField))]
public class DimensionInputHandler : MonoBehaviour
{
    public TMP_InputField _inputDimension;
    private string _previousValidText = "";
    private bool _isFormatting = false;
    //private bool _isDeleting = false;

    public UnityEvent DimensionSuccess;
    public UnityEvent ErrorInSuccess;

    void Start()
    {
        // Auto-get reference if not set in inspector
        if (_inputDimension == null)
            _inputDimension = GetComponent<TMP_InputField>();
                
    }

    public void HandleDimensionInput()
    {
        if (_isFormatting) return;

        _isFormatting = true;

        // Work directly with the current text
        string currentText = _inputDimension.text;

        // Skip formatting if user is deleting
        if (!IsUserDeleting(currentText))
        {
            string formattedText = FormatDimensionInput(currentText);

            if (IsPartialInputValid(formattedText))
            {
                _inputDimension.text = formattedText;
                //_previousValidText = formattedText;
                CheckdimensionsEntry();
            }
            else
            {
                _inputDimension.text = _previousValidText;
            }
        }
               
        _isFormatting = false;
    }
    private bool IsUserDeleting(string currentText)
    {
        return currentText.Length < _previousValidText.Length;
    }


    private string FormatDimensionInput(string input)
    {
        string cleaned = Regex.Replace(input, @"[^\d.x]", "");

        // Auto-insert 'x' logic
        if (!cleaned.Contains("x") && cleaned.Length > 0 &&
            !(cleaned.Contains(".") && cleaned.EndsWith(".")))
        {
            int insertPos = cleaned.Contains(".") ?
                Mathf.Min(cleaned.IndexOf('.') + 2, cleaned.Length) :
                cleaned.Length;
            cleaned = cleaned.Insert(insertPos, "x");
        }

        return cleaned;
    }

    private bool IsPartialInputValid(string input)
    {
        // Allow empty string for backspace handling and partial inputs, but not "x" alone
        if (string.IsNullOrEmpty(input)) return true;

        string partialPattern = @"^(?!(x)$)(\d+)?(\.\d*)?(x(\d+)?(\.\d*)?)?$";
        return Regex.IsMatch(input, partialPattern);
    }

    public bool ValidateDimensionFormatWithDecimals(string input)
    {
        string pattern = @"^[1-9]\d*(\.\d+)?x[1-9]\d*(\.\d+)?$";
        return Regex.IsMatch(input, pattern);
    }
    public void CheckdimensionsEntry()
    {
        if (string.IsNullOrEmpty(_inputDimension.text))
        {
            DimensionValidate();
        }
        else if (ValidateDimensionFormatWithDecimals(_inputDimension.text))
        {
            if (BigScreenRoomsControl.Bigroom._isMarbleDimension)
            {
                // enable continuebtn
            }
            else
            {
                RoomsManager.RoomInstance.CurrentDimension = _inputDimension.text + "ft";
            }
            DimensionSuccess?.Invoke();
            ResetPlaceholderColor();
        }
        else
        {
            DimensionValidate();
        }
    }

    public void DimensionValidate()
    {
        _inputDimension.placeholder.GetComponent<TMP_Text>().color = Color.red;
        ErrorInSuccess?.Invoke();
    }

    private void ResetPlaceholderColor()
    {
        _inputDimension.placeholder.GetComponent<TMP_Text>().color =
            new Color(0.196f, 0.196f, 0.196f); // Default TMPro color
    }
}