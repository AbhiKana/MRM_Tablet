using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MarbleWithTextDetails
{
    public int id;
    public string m_name;
}

public class WishlistManager : MonoBehaviour
{
    public static WishlistManager Instance;
    public List<MarbleWithTextDetails> marbleListWithTexts;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddMarble(MarbleWithTextDetails m)
    {
        if (!CheckMarblePresent(m.id))
            marbleListWithTexts.Add(m);
    }

    public void RemoveMarble(MarbleWithTextDetails m)
    {
        if(CheckMarblePresent(m.id))
            marbleListWithTexts.Remove(m);
    }

    public bool CheckMarblePresent(int id)
    {
        if (marbleListWithTexts.Count > 0)
        {
            foreach (var marble in marbleListWithTexts)
            {
                if (marble.id == id)
                    return true;
            }   
        }
        return false;
    }
}
