using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] private SpecificMarbleDetails marbleDetails;

    public List<SpecificMarbleDetails> list;
    public List<SpecificMarbleDetails> loadedMarbles;
    [HideInInspector] public List<MarbleDetails> costCalculatorList;

    private bool ignoreToggleEvent = false;

    #endregion
    private void Start()
    {
        OnToggleClick();

        //SelectionTileDetails.OnMarbleDeselected.AddListener(RemoveMarble);
        fetchQRData.OnDataLoaded.AddListener(() =>
        {
            ResetToggle();
            ShowMarbleData();
        });
    }

    public void ShowMarbleData()
    {
        Debug.Log("Present Details");
        var data = _marbleQrDatascritable._marbleApiData.marbleDetails;
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

        if (!AlreadyExists(marbleDetails.id, loadedMarbles))
        {
            loadedMarbles.Add(StoreInCache(marbleDetails, false));
        }

        SpecificMarbleDetails loadedMarble = loadedMarbles.Find(m => m.marble_name == marbleDetails.marble_name);

        if (loadedMarble != null)
        {
            ignoreToggleEvent = true;
            toggle.isOn = loadedMarble.isSelected;
            ignoreToggleEvent = false;
        }

       /* Debug.Log("Present Details");
        var data = _marbleQrDatascritable._marbleApiData.marbleDetails;
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

        if (AlreadyExists(marbleDetails.marble_name, list))
        {
            ignoreToggleEvent = true;
            toggle.isOn = true;
            ignoreToggleEvent = false;
        }

        SpecificMarbleDetails newDetails = StoreInCache(marbleDetails, toggle.isOn);
        if (!AlreadyExists(newDetails.marble_name, loadedMarbles))
        {
            Debug.LogError("Add new marble into loaded list");
            loadedMarbles.Add(newDetails);
        }*/
    }

    public void ShowLoadedMarbleData(string id)
    {
        foreach (var data in loadedMarbles)
        {
            if (data.id.ToString() == id)
            {
                StoreDataInSriptable(data);
                marbleDetails = data;
                Debug.Log("Show loaded marbles");
                fetchQRData._specificMarbleDetails = data;

                fetchQRData.LoadedData(id);

                if (AlreadyExists(fetchQRData._specificMarbleDetails.id, list))
                {
                    toggle.isOn = true;
                }
                else
                {
                    ignoreToggleEvent = true;
                    toggle.isOn = false;
                    ignoreToggleEvent = false;
                }
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
        /*toggle.onValueChanged.AddListener((isOn) => 
        {
            if(ignoreToggleEvent) return;

            string currentName = marbleDetails.marble_name;
            if (isOn == true && !AlreadyExists(currentName, list))
            {
                SpecificMarbleDetails newDetail = StoreInCache(marbleDetails, isOn);
                StoreSelectedMarble(newDetail);
            }
            else
            {
                Debug.Log("Remove from list");
                if (AlreadyExists(currentName, list))
                {
                    //RemoveSelectedMarble(currentName);
                }
            }
        });*/

        toggle.onValueChanged.AddListener((isOn) =>
        {
            if (ignoreToggleEvent) return;
            //StoreDataInSriptable()
            int currentID = marbleDetails.id;
            SpecificMarbleDetails loadedMarble = loadedMarbles.Find(m => m.id == currentID);

            if (loadedMarble != null)
            {
                Debug.Log("Marble Found");
                loadedMarble.isSelected = isOn;
                if (isOn && !AlreadyExists(currentID, list))
                {
                    Debug.Log("It should be addded");
                    list.Add(loadedMarble);
                }
                else if (!isOn && AlreadyExists(currentID, list))
                {
                    Debug.Log("It should be removed");
                    RemoveSelectedMarble(currentID);
                }
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

    bool AlreadyExists(int id, List<SpecificMarbleDetails> list)
    {
        foreach (SpecificMarbleDetails m in list)
        {
            if (m.id == id)
                return true;
        }
        return false;
    }
    public void StoreSelectedMarble(SpecificMarbleDetails specificMarbleDetails)
    {
        if (!AlreadyExists(specificMarbleDetails.id, list))
        {
            list.Add(specificMarbleDetails);
        }
        //list.Add(specificMarbleDetails);
    }
    public void RemoveSelectedMarble(string name)
    {
        /*foreach (SpecificMarbleDetails m in list)
        {
            if (m.marble_name == name)
            {
                list.Remove(m);
                break;
            }
        }*/
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
        list.Remove(list[index]);  
    }
    public void ResetToggle()
    {
        ignoreToggleEvent = true;
        toggle.isOn = false;
        ignoreToggleEvent = false;
    }
}
