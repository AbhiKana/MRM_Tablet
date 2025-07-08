using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    [SerializeField] GameObject[] childObjects;

    public void SetState(int index)
    {
        for (int i = 0; i < childObjects.Length; i++)
        {
            if(i == index)
                childObjects[i].SetActive(true);
            else
                childObjects[i].SetActive(false);
        }
    }
}
