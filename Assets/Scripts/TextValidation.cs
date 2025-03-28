using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Events;


public enum Textype
{
    Name,
    Email
};
public class TextValidation : MonoBehaviour
{
    private TMP_InputField Inputtext;
    public UnityEvent ValidationSuccess;
    public UnityEvent ErrorInSuccess;

    public Textype _textype;
    // Start is called before the first frame update
    void Start()
    {       
        Inputtext = this.GetComponent<TMP_InputField>();       
    }

    public void CheckTextValidation()
    {
        //  print(" --- "+(int)_textype);

        if (_textype == Textype.Name)
        {
            Inputtext.contentType = TMP_InputField.ContentType.Name;
            if (string.IsNullOrEmpty(Inputtext.text) || string.IsNullOrWhiteSpace(Inputtext.text))
            {
                Inputtext.placeholder.GetComponent<TMP_Text>().text = "Enter Name";
                TextColorValidate();
                ErrorInSuccess?.Invoke();
            }
            else
            {
                //  Debug.Log("valid Name Do next step");
                ValidationSuccess?.Invoke();
            }

        }
        else if (_textype == Textype.Email)
        {
            Inputtext.contentType = TMP_InputField.ContentType.EmailAddress;

            if (Regex.IsMatch(Inputtext.text, @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$"))
            {
                //  Debug.Log("valid Email Do next step");
                ValidationSuccess?.Invoke();
            }
            else
            {
                Inputtext.placeholder.GetComponent<TMP_Text>().text = "Enter Valid Email";
                TextColorValidate();
                ErrorInSuccess?.Invoke();
            }
        }
    }


    public void TextColorValidate()
    {
        Inputtext.placeholder.GetComponent<TMP_Text>().color = Color.red;
        // Inputtext.Select();
        // Inputtext.ActivateInputField();
    }

    public void RegexCheck()
    {       
        if (!Regex.IsMatch(Inputtext.text, @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$"))
        {            
            Inputtext.text = string.Empty;
            Inputtext.placeholder.GetComponent<TMP_Text>().text = "Enter Valid Email";
            TextColorValidate();
        }
    }
}