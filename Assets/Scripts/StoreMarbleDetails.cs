using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpecificMarbleDetails
{
    public string name;
    public Texture texture;
    public bool isSelected;
}

public class StoreMarbleDetails : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] RawImage topMarbleData;
    [SerializeField] FetchQRData fetchQRData;

    [SerializeField] private SpecificMarbleDetails marbleDetails;

    [SerializeField] List<SpecificMarbleDetails> list;

    private void Start()
    {
        //ResetToggle();
        OnMarbleScan();
        FetchQRData.OnDataLoaded.AddListener(StoreMarbleData);
    }

    private void OnEnable()
    {
        ResetToggle();
    }

    public void StoreMarbleData()
    {
        var data = fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails;
        marbleDetails.name = data.marble_name;
        marbleDetails.texture = topMarbleData.texture;
    }
    public void OnMarbleScan()
    {
        toggle.onValueChanged.AddListener((isOn) => 
        {
            if (isOn)
            {
                /*var data = fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails;
                marbleDetails.isSelected = isOn;
                marbleDetails.name = data.marble_name;
                marbleDetails.texture = topMarbleData.texture;*/
                //StoreMarbleData();
                StoreSelectedMarble(isOn);
            }
        });
    }


    public void StoreSelectedMarble(bool val)
    {
        if (list.Count == 0)
            list.Add(marbleDetails);
        else
        {
            var data = fetchQRData._marbleQrDatascritable._marbleApiData.marbleDetails;
            foreach (SpecificMarbleDetails m in list)
            {
                Debug.LogError(marbleDetails.name + " MD: "+ m.name);
                if (marbleDetails.name != m.name)
                {
                    Debug.Log("Add: " + val);
                    list.Add(marbleDetails);
                }
                /*else
                {
                    Debug.Log("Remove: " + val);
                    list.Remove(marbleDetails);
                }*/
            }
        }
    }

    public void ResetToggle()
    {
        toggle.isOn = false;
    }
}
