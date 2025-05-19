using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class SpecificMarbleDetails
{
    public int id;
    public string marble_name;
    public string description;
    public string dimension;
    public string material;
    public string finish;
    public string price;
    public string created_at;
    public string modify_at;

    public Texture mainTexture, circleImg;
    public Texture[] textures;

    public int availability;
    public int category_id;

    public bool isSelected;
}

public class StoreMarbleDetails : MonoBehaviour
{
    #region Variables
    public MarbleQRDATA _marbleQrDatascritable;
    [SerializeField] FetchQRData fetchQRData;
    [SerializeField] Toggle toggle;
    [SerializeField] RawImage topMarbleData;

    public SpecificMarbleDetails marbleDetails = new SpecificMarbleDetails();

    public List<SpecificMarbleDetails> list;
    public List<SpecificMarbleDetails> loadedMarbles;
    [HideInInspector] public List<MarbleDetails> costCalculatorList;

    private bool ignoreToggleEvent = false;

    public UnityEvent<bool> OnToggleSet;

    #endregion
    private void Start()
    {
        OnToggleClick();

        //SelectionTileDetails.OnMarbleDeselected.AddListener(RemoveMarble);
        fetchQRData.OnDataLoaded.AddListener(() =>
        {
            ShowMarbleData();
        });
    }

    public void ShowMarbleData()
    {
        var data = _marbleQrDatascritable._marbleApiData.marbleDetails;

        Debug.Log("Present Details: " + data.marble_name);

        marbleDetails.marble_name = data.marble_name;
        marbleDetails.description = data.description;
        marbleDetails.dimension = data.dimension;
        marbleDetails.material = data.material;
        marbleDetails.finish = data.finish;
        marbleDetails.price = data.price;

        marbleDetails.availability = data.availability;
        marbleDetails.category_id = data.category_id;
        marbleDetails.id = data.id;

        marbleDetails.mainTexture = fetchQRData.TopMarbleImage.texture;
        marbleDetails.circleImg = fetchQRData.CircleImage.texture;
        marbleDetails.textures = fetchQRData.boxImageTexture;


        if (AlreadyExists(marbleDetails.id, list))
        {
            SetWishList();
            StoreTexturesInList(marbleDetails.id);
        }
    }

    private void SetWishList()
    {
        ignoreToggleEvent = true;
        toggle.isOn = true;
        marbleDetails.isSelected = true;
        ignoreToggleEvent = false;
    }

    public void StoreTexturesInList(int id)
    {
        foreach (var item in list)
        {
            if (item.id == id)
            {
                Debug.Log("Load Image");
                item.textures = null;
                
                if (item.mainTexture == null)
                    item.mainTexture = marbleDetails.mainTexture;

                if (item.circleImg == null)
                    item.circleImg = marbleDetails.circleImg;

                if (item.textures == null)
                    item.textures = marbleDetails.textures;
            }
        }
        //if(list)
    }

    public void StoreTexturesInList(int id, ShowMarbleDetails showMarble)
    {
        foreach (var item in list)
        {
            if (item.id == id)
            {
                Debug.Log("Load Image");
                item.textures = null;

                if (item.mainTexture == null)
                    item.mainTexture = showMarble.image.texture;

                if (item.circleImg == null)
                    item.circleImg = showMarble.m_Textures[0];

                if (item.textures == null)
                    item.textures = showMarble.m_Textures;
            }
        }
    }


    public void StoreDataInSriptable(SpecificMarbleDetails s)
    {
        var details = _marbleQrDatascritable._marbleApiData.marbleDetails;
        details.id = s.id;
        details.marble_name = s.marble_name;
        details.price = s.price;
        details.description = s.description;
        details.material = s.material;
    }
    public void OnToggleClick()
    {
        toggle.onValueChanged.AddListener((isOn) =>
        {
            if (ignoreToggleEvent) return;

            Debug.Log("Value chnage to: " + isOn);
            OnToggleSet?.Invoke(isOn);
            int currentID = marbleDetails.id;

            Debug.Log("Marble Found: " + isOn);
            marbleDetails.isSelected = isOn;

            if (isOn && !AlreadyExists(currentID, list))
            {
                Debug.Log("It should be addded");
                SpecificMarbleDetails newDetail = StoreInCache(marbleDetails, isOn);
                list.Add(newDetail);
            }
            else if (!isOn && AlreadyExists(currentID, list))
            {
                Debug.Log("It should be removed");
                RemoveSelectedMarble(currentID);
            }
        });

    }
    private SpecificMarbleDetails StoreInCache(SpecificMarbleDetails marbleDetails, bool val)
    {
        return new SpecificMarbleDetails
        {
            marble_name = marbleDetails.marble_name,
            mainTexture = marbleDetails.mainTexture,
            circleImg = marbleDetails.circleImg,
            textures = marbleDetails.textures,

            dimension = marbleDetails.dimension,
            material = marbleDetails.material,
            finish = marbleDetails.finish,
            price = marbleDetails.price,
            created_at = marbleDetails.created_at,
            modify_at = marbleDetails.modify_at,

            availability = marbleDetails.availability,
            category_id = marbleDetails.category_id,
            id = marbleDetails.id,
            isSelected = val
        };
    }

    public bool AlreadyExists(int id, List<SpecificMarbleDetails> list)
    {
        foreach (SpecificMarbleDetails m in list)
        {
            if (m.id == id)
                return true;
        }
        return false;
    }

    public void RemoveSelectedMarble(string name)
    {
        Debug.Log("Marble to be removed");
        list.RemoveAll(m => m.marble_name == name);
    }
    public void RemoveSelectedMarble(int id)
    {
        foreach (SpecificMarbleDetails m in list)
        {
            if (m.id == id)
            {
                list.Remove(m);
                break;
            }
        }
    }
    public void RemoveMarble(int index)
    {
        //list.Remove(list[index]);  
    }
    public void ResetToggle()
    {
        ignoreToggleEvent = true;
        toggle.isOn = false;
        ignoreToggleEvent = false;
    }

    public void SetToggleValue(bool val)
    {
        Debug.Log("check toggle status: " + val);
        toggle.isOn = val;
    }

    public void OnShowMarbleDisable()
    {
        ignoreToggleEvent = true;
        SetToggleValue(false);
        marbleDetails.isSelected = false;
        ignoreToggleEvent = false;
    }

    public void OnShowMarbleDisable(bool value)
    {
        ignoreToggleEvent = true;
        SetToggleValue(value);
        ignoreToggleEvent = false;
    }

    public void EmptyMarbleDetails()
    {
        marbleDetails.id = 0;
        marbleDetails.marble_name = string.Empty;
        marbleDetails.description = string.Empty;
        marbleDetails.dimension = string.Empty;
        marbleDetails.material = string.Empty;
        marbleDetails.finish = string.Empty;
        marbleDetails.price = string.Empty;
        marbleDetails.created_at = string.Empty;
        marbleDetails.modify_at = string.Empty;

        marbleDetails.mainTexture = null;
        marbleDetails.circleImg = null;

        marbleDetails.availability = 0;
        marbleDetails.category_id = 0;
        marbleDetails.isSelected = false;
    }
}
