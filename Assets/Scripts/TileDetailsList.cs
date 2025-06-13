using System.Linq;
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

    /*public void RemoveLoadedMarbleFromList(int id)
    {
        foreach (LoadImageInBG show in loadImageInBGs)
        {
            if (show.showMarbleDetails.tileID == id)
            {
                loadImageInBGs.Remove(show);
                break;
            }
        }
    }*/
    private void Start()
    {
        uI_Manager = FindObjectOfType<UI_Manager>();
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
        noof_marble = storeMarbleDetails.list.Count;
        noof_imagedownload = 0;

        for (int i = 0; i < noof_marble; i++)
        {
            // Find corresponding marble in listOfMarbleDetails by name
            string currentMarbleName = storeMarbleDetails.list[i].marble_name;
            var marbleDetail = storeMarbleDetails.listOfMarbleDetails.FirstOrDefault(
                m => m.marbleName.Equals(currentMarbleName)
            );

            if (marbleDetail == null)
            {
                Debug.LogError($"Marble {currentMarbleName} not found in details list");
                continue;
            }

            Debug.Log("Processing marble: " + currentMarbleName);

            var details = storeMarbleDetails.list[i].textures;
            if (details == null || details.Length == 0)
            {
                storeMarbleDetails.loader.gameObject.SetActive(true);
                LoadImageInBG loadImageInBG = marbleDetail.GetComponent<LoadImageInBG>();
                loadImageInBG.LoadMarbleImageData();
                Debug.Log("Textures not loaded - starting download");
            }
            else
            {
                noof_imagedownload++;
                Debug.Log("Textures already loaded");
            }

            var imageLoader = marbleDetail.GetComponent<LoadImageInBG>();
            imageLoader.OnBGImageDownload.AddListener(HandleLoading);
        }

        if (!isNewImageAdded)
            SpawnMarbleDetails();
    }
    private void HandleLoading()
    {
        noof_imagedownload++;

        if (noof_imagedownload == noof_marble)
        {
            storeMarbleDetails.loader.gameObject.SetActive(false);
            isNewImageAdded = false;
            SpawnMarbleDetails();
            //RemoveLoadedMarbleFromList();
        }
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
