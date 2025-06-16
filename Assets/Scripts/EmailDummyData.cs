using UnityEngine;

public class EmailDummyData : MonoBehaviour
{
    [SerializeField] EmailValidation emailValidation;
    //[SerializeField] EmailValidation emailValidation_2;

    private const string EMAIL_KEY = "UserEmail";
    private const string NAME_KEY = "UserName";

    private void Start()
    {
        Invoke(nameof(LoadSavedEmail), 1f);
        emailValidation.EmailSuccess.AddListener(SaveEmailAndName);
    }

    public void SaveEmailAndName()
    {
        string email = emailValidation.EmailinputField.text;
        string name = emailValidation.NameinputField.text;

        PlayerPrefs.SetString(EMAIL_KEY, email);
        PlayerPrefs.SetString(NAME_KEY, name);
        PlayerPrefs.Save();
    }

    public void LoadSavedEmail()
    {
        if (PlayerPrefs.HasKey(EMAIL_KEY) && PlayerPrefs.HasKey(NAME_KEY))
        {
            string savedEmail = PlayerPrefs.GetString(EMAIL_KEY);
            string savedName = PlayerPrefs.GetString(NAME_KEY);
            Debug.Log(savedEmail+ " "+ savedName);
            
            emailValidation.NameinputField.text = savedName;
            emailValidation.EmailinputField.text = savedEmail;
            
            //emailValidation_2.NameinputField.text = savedName;
            //emailValidation_2.EmailinputField.text = savedEmail;
        }
    }
}