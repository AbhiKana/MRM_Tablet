using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MarbleInvoice
{
    public List<Room> rooms = new();
    public PriceSummary price_summary = new();
}

[Serializable]
public class Marble
{
    public int id;
    public string name;
    public string dimension;
    public float price;
    public int amount;
    public string imgurl;
}

[Serializable]
public class Room
{
    public int id;
    public string name;
    public string dimension;
    public List<Marble> marbles = new ();
}

[Serializable]
public class PriceSummary
{
    public int subtotal;
    public float wastage_fee;
    public float installation;
    public float transport;
    public float tax;
    public int total;
}

