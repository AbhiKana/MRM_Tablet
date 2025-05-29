using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DimensionTextValidation : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField _inputDimension1;
    public TMP_InputField _inputDimension2;
    public int Marbleid = 0;
    public GameObject _droppedMarble;
   
    public UnityEvent DimensionSuccess;
    public UnityEvent ErrorInSuccess;

    // text dimesion of marble 
    public string TextDimesion2;

  
    void Start()
    {
        TextDimesion2 = "";
    }
    private void OnEnable()
    {
        _inputDimension1.text = "";
        _inputDimension2.text = "";
        _inputDimension1.Select();
        _inputDimension1.placeholder.GetComponent<TMP_Text>().color = new Color(0.196f, 0.196f, 0.196f);
        _inputDimension2.placeholder.GetComponent<TMP_Text>().color = new Color(0.196f, 0.196f, 0.196f);
    }

    // for each boxes check not empty and only float or integer 
    // keep  content text type is decimal
    // then combine both values with x like 12 x 12
    public static bool ValidateInputDimension(TMP_InputField _inputtext)
    {
        if ((int.TryParse(_inputtext.text, out int number) && number > 0) || (float.TryParse(_inputtext.text, out float number2) && number2 > 0f))
        {
            _inputtext.placeholder.GetComponent<TMP_Text>().color = new Color(0.196f, 0.196f, 0.196f); // Default TMPro placeholder color
            //go ahead
            return true;
        }
        else
        {
            _inputtext.placeholder.GetComponent<TMP_Text>().color = Color.red;
            return false;
        }

    }

    // check marble dimesion both  inputTextbox entry
    public void CheckBothtextEntry()
    {
        if (ValidateInputDimension(_inputDimension1) && ValidateInputDimension(_inputDimension2))
        {
            TextDimesion2 = _inputDimension1.text + "x" + _inputDimension2.text;
          
            DimensionSuccess?.Invoke();
        }
        else
        {
            ErrorInSuccess?.Invoke();
        }
    }     
    

    // on click of continue button of marble dimesion overlay 
    public void OnclickContinueDimesion()
    { //send marble id and dimesions entered to save marble data in room
        BigScreenRoomsControl.Bigroom.SaveMarbleDataInRoom(Marbleid, TextDimesion2);
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
  

         
}
