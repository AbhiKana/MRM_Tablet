using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public class WWWRequestTC
{
    public class TempWebRequest : MonoBehaviour { }

    private static TempWebRequest tempWebRequest;
    private Coroutine signInPostCoroutine;
    private Coroutine signInGetCoroutine;
    private Coroutine GetTextureCoroutine;

    public WWWRequestTC()
    {
        if (tempWebRequest == null)
            tempWebRequest = new GameObject("TempWebRequest_").AddComponent<TempWebRequest>();
    }

    public void Post(WWWForm _form, string _url, Action<string, bool> _Callback)
    {
        if (signInPostCoroutine != null)
            tempWebRequest.StopCoroutine(signInPostCoroutine);

        signInPostCoroutine = tempWebRequest.StartCoroutine(LogIn(_url, _form, (responseJson, isSuccess) =>
        {
            _Callback.Invoke(responseJson, isSuccess);
        }));
    }

    private IEnumerator LogIn(string _url, WWWForm _form, Action<string, bool> _callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(_url, _form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                _callback.Invoke(www.error, false);
            }
            else
            {
                _callback.Invoke(www.downloadHandler.text, true);
            }
        }
    }

    public void Get(string _url, Action<string, bool> _Callback)
    {
        if (signInGetCoroutine != null)
            tempWebRequest.StopCoroutine(signInGetCoroutine);

        signInGetCoroutine = tempWebRequest.StartCoroutine(LogIn(_url, (responseJson, isSuccess) =>
        {
            _Callback.Invoke(responseJson, isSuccess);
        }));
    }

    private IEnumerator LogIn(string _url, Action<string, bool> _callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(_url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                _callback.Invoke(www.error, false);
            }
            else
            {
                _callback.Invoke(www.downloadHandler.text, true);
            }
        }
    }

    /*public void GetTexture(string _url, Action<string, Texture2D, bool> _Callback)
    {
        //if (GetTextureCoroutine != null)
        //    tempWebRequest.StopCoroutine(GetTextureCoroutine);

        //GetTextureCoroutine = tempWebRequest.StartCoroutine(LogIn(_url, (responseJson, texture, isSuccess) =>
        //{
        //    _Callback.Invoke(responseJson, texture, isSuccess);
        //}));

        GetTextureCoroutine = tempWebRequest.StartCoroutine(LogIn(_url, (responseJson, texture, isSuccess) =>
        {
            _Callback.Invoke(responseJson, texture, isSuccess);
        }));
    }*/

    public async void GetTexture(string _url, Action<string, Texture2D, bool> _Callback)
    {
        await LogIn(_url, (responseJson, texture, isSuccess) =>
        {
            _Callback.Invoke(responseJson, texture, isSuccess);
        });
    }

    private async Task LogIn(string _url, Action<string, Texture2D, bool> _callback)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                _callback.Invoke(www.error, null, false);
            }
            else
            {
                _callback.Invoke(www.downloadHandler.text, DownloadHandlerTexture.GetContent(www), true);
            }
        }
    }
}
