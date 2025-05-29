using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;


public class RoomDimensionInput : MonoBehaviour
{
    public TMP_InputField _input1;
    public TMP_InputField _input2;
    public TMP_InputField _input3;
    public UnityEvent DimensionSuccess;
    public UnityEvent ErrorInSuccess;
    public string TextDimesion;
    // Start is called before the first frame update
    void Start()
    {
        TextDimesion = "";
        // _input1 = _input1.GetComponent<TMP_InputField>();
        //  _input1.Select();
    }

    private void OnEnable()
    {
        _input1.text = "";
        _input2.text = "";
        _input3.text = "";
        _input1.Select();
        ResetgreyColor(_input1);
        ResetgreyColor(_input2);
        ResetgreyColor(_input3);
    }

    public void ResetgreyColor(TMP_InputField _intext)
    {
        _intext.placeholder.GetComponent<TMP_Text>().color =
          new Color(0.196f, 0.196f, 0.196f); // Default TMPro color
    }

    // for each boxes check not empty and only float or integer 
    // keep  content text type is decimal
    // then combine both values with x like 12 x 12
    public static bool ValidateInput(TMP_InputField _inputtext)
    {
        if ((int.TryParse(_inputtext.text, out int number) && number > 0) || (float.TryParse(_inputtext.text, out float number2) && number2 > 0f))
        {
            //go ahead

            return true;
        }
        else
        {
            _inputtext.placeholder.GetComponent<TMP_Text>().color = Color.red;
            return false;
        }

    }


    public void CheckAlltextEntry()
    {
        if (ValidateInput(_input1) && ValidateInput(_input2) && ValidateInput(_input3))
        {
            TextDimesion = _input1.text + "x" + _input2.text + "x" + _input3.text;

            RoomsManager.RoomInstance.CurrentDimension = TextDimesion + "ft";

            DimensionSuccess?.Invoke();
        }
        else
        {
            ErrorInSuccess?.Invoke();
        }
    }


}

