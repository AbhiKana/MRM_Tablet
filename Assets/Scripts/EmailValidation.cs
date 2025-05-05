using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class EmailValidation : MonoBehaviour
{
    public TMP_InputField NameinputField;
    public TMP_InputField EmailinputField;
    public UnityEvent EmailSuccess;
    public bool isCredentialsEntered;
    public enum Textype{
        Name,
        Email
    };

    Textype _textype;
    public void CheckEmailValidation()
    {
        if (string.IsNullOrEmpty(EmailinputField.text) || string.IsNullOrWhiteSpace(EmailinputField.text))
        {
            EmailinputField.placeholder.GetComponent<TMP_Text>().text = "Enter Valid Email";
            EmailValidate();        

        }
        else if (!Regex.IsMatch(EmailinputField.text, @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$"))
        {
            EmailinputField.placeholder.GetComponent<TMP_Text>().text = "Enter Valid Email";
           
            EmailValidate();
        }
        else
        {
            Debug.Log("valid Email Do next step");
            EmailSuccess?.Invoke();
            isCredentialsEntered = true;
        }

    }

    public void EmailValidate()
    {
        EmailinputField.text = string.Empty;
        EmailinputField.placeholder.GetComponent<TMP_Text>().color = Color.red;
    }

    public void EmailDone()
    {
        /*bool checkVal = true;
        Debug.Log(checkVal);*/
    }    
}
