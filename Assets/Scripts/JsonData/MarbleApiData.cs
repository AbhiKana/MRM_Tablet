using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    public class MarbleData : ScriptableObject
    {
        public string success;
        public Categories[] categories;
        public MarbleDetails[] marbleDetails;
    }*/
public class MarbleApiData : MonoBehaviour
{
    [System.Serializable]
    public class Categories
    {
        public int id;
        public string name;
    }

    [System.Serializable]
    public class MarbleDetails
    {
        public int id;
        public string marble_name;
        public string img;
    }
}
