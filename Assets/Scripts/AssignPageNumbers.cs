using UnityEngine;

public class AssignPageNumbers : MonoBehaviour
{
    public void UpdatePageNum()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            SelectionTileDetails item = child.GetComponent<SelectionTileDetails>();

            if (item != null)
            {
                item.pageNum = i;
            }
        }
    }
}
