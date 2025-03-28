using UnityEngine;

public class SelectionPanelController : MonoBehaviour
{
    [SerializeField] int noof_marble;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parentObjectToSpawn;

    public void GetSelectedMarbleList(GameObject prefabTile)
    {
        for (int i = 0; i < noof_marble; i++) 
        {
            GameObject marble = Instantiate(tilePrefab);
            marble.transform.SetParent(parentObjectToSpawn);
            marble.transform.localScale = Vector3.one;
        }
    }
}
