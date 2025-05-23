using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSelectedMarble : MonoBehaviour
{
    [SerializeField] SceneSwitchManager sceneSwitchManager;
    [SerializeField] StoreMarbleDetails storeMarbleDetails;
    private void Awake()
    {
        sceneSwitchManager = FindObjectOfType<SceneSwitchManager>();
        storeMarbleDetails = FindObjectOfType<StoreMarbleDetails>();
    }
    private void Start()
    {
        sceneSwitchManager.sceneChangeEvent.AddListener(() =>
        {
            SetMarbleList();
            BigScreenRoomsControl.Bigroom.AddSelectedMarblelistToScroll();
        });
    }

    void SetMarbleList()
    {
        foreach(SpecificMarbleDetails s in storeMarbleDetails.list)
        {
            MarbleDetails marbleDetails = new MarbleDetails();
            marbleDetails.id = s.id;
            marbleDetails.marble_name = s.marble_name;
            marbleDetails._Texture = s.mainTexture;
            marbleDetails.PriceMarble = s.price;
            marbleDetails.imgUrl = s.url;
            BigScreenRoomsControl.Bigroom.dumyselectedMarbleList.Add(marbleDetails);
        }
    }
}
