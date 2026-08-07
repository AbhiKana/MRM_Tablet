using Unity.Entities;
using Unity.Collections;

// Unmanaged component: Holds the fast data
public struct MarbleStateData : IComponentData
{
    public int MarbleId;
    public int CategoryId;
    public bool IsWishlisted;
    public bool IsImageDownloaded;
}

// Managed component: The bridge to your existing GameObject
public class MarbleGameObjectLink : IComponentData
{
    public ShowMarbleDetails View; // Reference to your UI script
}