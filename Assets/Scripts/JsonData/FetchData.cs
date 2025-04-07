using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MarbleApiData;
using UnityEngine.Networking;

public class FetchData : MonoBehaviour
{
    public void GetDataFrom(string url, string id)
    {
        StartCoroutine(PostRequest(url,id));
    }

    IEnumerator PostRequest(string url, string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
