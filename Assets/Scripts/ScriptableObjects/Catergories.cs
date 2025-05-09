using System;
using System.Collections.Generic;

[Serializable] 
public class Category
{
    public int category_id;
    public string category_name;
}

/*[Serializable]
public class Marble_Unused
{
    public int id;
    public string marble_name;
    public int category_id;
    public string img;
    public string price;
}*/

[Serializable]
public class Catergories
{
    public bool success;
    public List<Category> category;
    //public List<Marble> marbleDetails;
}

