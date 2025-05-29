using UnityEngine;

[System.Serializable]
public class StoreUserData
{
    public bool success;
    public bool already_register;
    public string data;
    public string user_id;
}

[System.Serializable]
public class DataSendToConfig
{
    public string mail;
    public string user_id;
}


public class UserData : MonoBehaviour
{
    public StoreUserData storeUserData;
}