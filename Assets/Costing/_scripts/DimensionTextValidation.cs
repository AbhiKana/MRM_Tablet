using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DimensionTextValidation : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField _inputDimension;
    public int Marbleid = 0;
    public GameObject _droppedMarble;
    private string _previousValidText = "";
    private bool _isFormatting = false;

    public UnityEvent DimensionSuccess;
    public UnityEvent ErrorInSuccess;
    void Start()
    {
        _inputDimension = this.GetComponent<TMP_InputField>();
        // Debug.Log(ValidateDimensionFormatWithDecimals("10x20 ft"));
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
        // string pattern = @"^\d+(\.\d+)?x\d+(\.\d+)?\s*ft$"; // ft
        //  string pattern = @"^\d+(\.\d+)?x\d+(\.\d+)?$"; // allows 0
        string pattern = @"^(?!.*\b0(\.0+)?\b)[1-9]\d*(\.\d+)?x[1-9]\d*(\.\d+)?$";
        return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
    }

    public void CheckdimensionsEntry()
    {
        if (string.IsNullOrEmpty(_inputDimension.text))
        {
            DimensionValidate();
        }
        else
        {

            if (ValidateDimensionFormatWithDecimals(_inputDimension.text))
            {
                ResetPlaceholderColor();
                if (BigScreenRoomsControl.Bigroom._isMarbleDimension)
                {
                    // enable continuebtn

                }
                else
                {
                    RoomsManager.RoomInstance.CurrentDimension = _inputDimension.text + "ft";
                }
                DimensionSuccess?.Invoke();

            }
            else
            {
                DimensionValidate();
            }
        }
    }
    private void ResetPlaceholderColor()
    {
        _inputDimension.placeholder.GetComponent<TMP_Text>().color =
            new Color(0.196f, 0.196f, 0.196f); // Default TMPro color
    }
    public void DimensionValidate()
    {
        //_inputDimension.text = string.Empty;
        _inputDimension.placeholder.GetComponent<TMP_Text>().color = Color.red;
        ErrorInSuccess?.Invoke();
    }

    // on click of continue button of marble dimesion overlay 
    public void OnclickContinueDimesion()
    { //send marble id and dimesions entered to save marble data in room
        BigScreenRoomsControl.Bigroom.SaveMarbleDataInRoom(Marbleid, _inputDimension.text);
        BigScreenRoomsControl.Bigroom.MarbleDimensionOverlay.gameObject.SetActive(false);
        BigScreenRoomsControl.Bigroom._isMarbleDimension = false;
    }

    // close button will be always on
    // on click of close button of marble dimesion overlay 
    public void OnclickonCloseDimesion()
    {
        //destroy added marble and off the marble overlay panel
        _droppedMarble.transform.parent.GetComponent<Drop2D>().OnDropOnce = false;
        _droppedMarble.transform.parent.GetComponent<Drop2D>().AddDropListner();
        Destroy(_droppedMarble);
        BigScreenRoomsControl.Bigroom.MarbleDimensionOverlay.gameObject.SetActive(false);
        // BigScreenRoomsControl.Bigroom.RemoveMarbleDataInRoom(Marbleid);
        BigScreenRoomsControl.Bigroom._isMarbleDimension = false;
    }
    // _inputDimension.placeholder.GetComponent<TMP_Text>().color = new Color(0.196f, 0.196f, 0.196f); // Default TMPro placeholder color
}
