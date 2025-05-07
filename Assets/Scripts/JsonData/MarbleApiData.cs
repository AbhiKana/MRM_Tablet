using System.Collections.Generic;
/*
    public class MarbleData : ScriptableObject
    {
        public string success;
        public Categories[] categories;
        public MarbleDetails[] marbleDetails;
    }*/
//public class MarbleApiData : MonoBehaviour
//{
//    [System.Serializable]
//    public class Categories
//    {
//        public int id;
//        public string name;
//    }

//    [System.Serializable]
//    public class MarbleDetails
//    {
//        public int id;
//        public string marble_name;
//        public string img;
//    }
//}


[System.Serializable]
public class MarbleApiData
{
    public bool success;
    public Marbledetail marbleDetails;
}

[System.Serializable]
public class GetMarblesList
{
    public bool success;
    public List<MarbleDetail> marbleDetails;
}

[System.Serializable]
public class Marbledetail
{
    public int id;
    public string marble_name;
    public string description;
    public string dimension;
    public string material;
    public string finish;
    public string price;
    public int availability;
    public int category_id;
    public string main_img;
    public List<string> texture_img;
}

[System.Serializable]
public class MarbleDetail
{
    public int id;
    public string marble_name;
    public string description;
    public string dimension;
    public string material;
    public string finish;
    public string price;
    public int availability;
    public int category_id;
    public string main_img;
    public List<string> texture_img;
}

