using System;
using System.Collections.Generic;
using UnityEngine;

public class TileDetailsList : MonoBehaviour
{
    UI_Manager uI_Manager;
    [SerializeField] StoreMarbleDetails storeMarbleDetails;
    [SerializeField] Transform parentObjectToSpawn;
    [SerializeField] GameObject tilePrefab;

    [SerializeField] int noof_marble;
    public int noof_imagedownload;
    [SerializeField] bool isNewImageAdded; 

    [SerializeField] List<LoadImageInBG> loadImageInBGs = new List<LoadImageInBG>();

    private void Start()
    {
        uI_Manager = FindObjectOfType<UI_Manager>();
    }

    private void HandleLoading()
    {
        noof_imagedownload++;

        if(noof_imagedownload == noof_marble)
        {
            storeMarbleDetails.loader.gameObject.SetActive(false);
            isNewImageAdded = false;
            SpawnMarbleDetails();
            //RemoveLoadedMarbleFromList();
        }
    }

    public void RemoveLoadedMarbleFromList(int id)
    {
        foreach (LoadImageInBG show in loadImageInBGs)
        {
            if (show.showMarbleDetails.tileID == id)
            {
                loadImageInBGs.Remove(show);
                break;
            }
        }
    }

    private void SpawnMarbleDetails()
    {
        for (int i = 0; i < noof_marble; i++)
        {
            var mrmDet = storeMarbleDetails.list[i];
            AssignData_OnLoad(mrmDet);
        }

        uI_Manager.RefreshList();
    }

    public void SpawnMarbleDetailsList()
    {
        Debug.Log("Spawn list of marbles");
        noof_marble = storeMarbleDetails.list.Count;

        //if(noof_imagedownload != 0)
        //noof_imagedownload = 0;
        for (int i = 0; i < noof_marble; i++)
        {
            var details = storeMarbleDetails.list[i].textures;
            if (details != null && details.Length == 0)
            {
                storeMarbleDetails.loader.gameObject.SetActive(true);
                LoadImageInBG loadImageInBG = storeMarbleDetails.listOfMarbleDetails[i].GetComponent<LoadImageInBG>();
                loadImageInBG.LoadMarbleImageData();
                Debug.Log("list of textures are not loaded");
            }
            /*else
            {
                noof_imagedownload++;
            }*/

            var imageLoader = storeMarbleDetails.listOfMarbleDetails[i].GetComponent<LoadImageInBG>();
            if (!loadImageInBGs.Contains(imageLoader))
            {
                isNewImageAdded = true;
                loadImageInBGs.Add(imageLoader);
                loadImageInBGs[i].OnBGImageDownload.AddListener(HandleLoading);
            }
        }

        if(!isNewImageAdded)
            SpawnMarbleDetails();
    }

    private void AssignData_OnLoad(SpecificMarbleDetails mrmDet)
    {
        GameObject marbleDetails = Instantiate(tilePrefab).gameObject;
        MRM_Details mRM_Details = marbleDetails.GetComponent<MRM_Details>();

        if (mRM_Details != null)
        {
            mRM_Details.transform.SetParent(parentObjectToSpawn);
            mRM_Details.transform.localScale = Vector3.one;

            mRM_Details.MarbleName.text = mrmDet.marble_name;
            mRM_Details.MarbleDescription.text = mrmDet.description;
            mRM_Details.MarbleDimension.text = mrmDet.dimension;
            mRM_Details.MarbleMaterial.text = mrmDet.material;
            mRM_Details.MarbleFinish.text = mrmDet.finish;
            mRM_Details.MarbleAvailability.text = mrmDet.availability.ToString();
            mRM_Details.MarblePrice.text = mrmDet.price + " sq/ft";
            mRM_Details.isSelected.isOn = mrmDet.isSelected;

            if (mrmDet.mainTexture != null)
                mRM_Details.TopMarbleImage.texture = mrmDet.mainTexture;

            if (mrmDet.textures != null && mrmDet.textures.Length > 0)
            {
                if (mrmDet.textures[0] != null)
                    mRM_Details.CircleImage.texture = mrmDet.textures[0];
            }

            if (mrmDet.textures != null && mrmDet.textures.Length > 0)
            {
                for (int j = 0; j < mRM_Details.BgImages.Length; j++)
                {
                    mRM_Details.BgImages[j].texture = mrmDet.textures[j];
                }
            }
        }
    }
}
