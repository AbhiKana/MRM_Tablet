using System;
using System.Collections.Generic;

[Serializable]
public class Marble
{
    public int id;
    public string marble_name;
    public string img;
}

[Serializable]
public class AllMarbles
{
    public bool success;
    public List<Marble> marbleDetails;
}