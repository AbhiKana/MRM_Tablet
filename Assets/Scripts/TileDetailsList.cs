using UnityEngine;

public class TileDetailsList : MonoBehaviour
{
    [SerializeField] StoreMarbleDetails storeMarbleDetails;
    [SerializeField] Transform parentObjectToSpawn;
    [SerializeField] GameObject tilePrefab;

    [SerializeField] int noof_marble;
    public void SpawnMarbleDetailsList()
    {
        noof_marble = storeMarbleDetails.list.Count;
        for (int i = 0; i < noof_marble; i++)
        {
            var mrmDet = storeMarbleDetails.list[i];
            AssignData_OnLoad(mrmDet);
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
            if (mrmDet.textures[0] != null)
                mRM_Details.CircleImage.texture = mrmDet.textures[0];

            if (mrmDet.textures != null)
            {
                for (int j = 0; j < mRM_Details.BgImages.Length; j++)
                {
                    mRM_Details.BgImages[j].texture = mrmDet.textures[j];
                }
            }
        }
    }
}
