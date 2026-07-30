using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class HeaderDataClass
{
    public string headerName;
    public string headerstring;
}
public class WWWRequestTC
{
    public class TempWebRequest : MonoBehaviour { }

    private static TempWebRequest tempWebRequest;
    private Coroutine signInPostCoroutine;

    public WWWRequestTC()
    {
        if (tempWebRequest == null)
            tempWebRequest = new GameObject("TempWebRequest").AddComponent<TempWebRequest>();
    }

    public async UniTask Get(string _url, HeaderDataClass[] headers, Action<string, bool> _Callback, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            _Callback.Invoke("request cancel", false);
            return;
        }

        try
        {
            await LogIn(_url, headers, (responseJson, isSuccess) =>
            {
                _Callback.Invoke(responseJson, isSuccess);
            }, token);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Operation failed but handled: {ex.Message}");
            _Callback?.Invoke("operation cancel", false);
        }
    }
    private async UniTask LogIn(string _url, HeaderDataClass[] headers, Action<string, bool> _callback, CancellationToken token = default)
    {
        Debug.Log($"[DEBUG] URL: {_url}");
        Debug.Log($"[DEBUG] Headers Count: {headers?.Length ?? 0}");
        for (int i = 0; i < headers?.Length; i++)
        {
            Debug.Log($"[DEBUG] Header[{i}]: {headers[i].headerName} = {headers[i].headerstring}");
        }

        using (UnityWebRequest www = UnityWebRequest.Get(_url))
        {
            Debug.Log($"[DEBUG] Method: {www.method}");
            Debug.Log($"[DEBUG] Timeout: {www.timeout}s");
            Debug.Log($"[DEBUG] WebRequest URI: {www.uri}");
            Debug.Log($"[DEBUG] WebRequest URL: {www.url}");

            for (int i = 0; i < headers.Length; i++)
            {
                www.SetRequestHeader(headers[i].headerName, headers[i].headerstring);
            }

            // Read back the custom headers we just set to confirm they’re stored
            for (int i = 0; i < headers.Length; i++)
            {
                Debug.Log($"[DEBUG] Verify Header[{i}] {headers[i].headerName} = '{www.GetRequestHeader(headers[i].headerName)}'");
            }

            var operation = www.SendWebRequest().ToUniTask(cancellationToken: token);

            try
            {
                await operation;

                Debug.Log($"[DEBUG] Result: {www.result}");
                Debug.Log($"[DEBUG] HTTP Code: {www.responseCode}");
                Debug.Log($"[DEBUG] Error: {www.error}");

                var responseHeaders = www.GetResponseHeaders();
                Debug.Log($"[DEBUG] Response Headers Count: {responseHeaders?.Count ?? 0}");
                if (responseHeaders != null)
                {
                    foreach (var h in responseHeaders)
                    {
                        Debug.Log($"[DEBUG]   {h.Key}: {h.Value}");
                    }
                }

                Debug.Log($"[DEBUG] Response Body Length: {www.downloadHandler?.text?.Length ?? 0}");
                Debug.Log($"[DEBUG] Response Body: {www.downloadHandler?.text}");

                if (www.result != UnityWebRequest.Result.Success)
                {
                    _callback.Invoke(www.error, false);
                    Debug.LogError($"[DEBUG] Result at Failure: {www.error}");
                }
                else
                {
                    _callback.Invoke(www.downloadHandler.text, true);
                    Debug.Log($"[DEBUG] Result at Success: {www.result}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[DEBUG] Exception Type: {e.GetType().Name}");
                Debug.LogError($"[DEBUG] Exception Message: {e.Message}");
                Debug.LogError($"[DEBUG] Exception StackTrace: {e.StackTrace}");
                Debug.LogError($"[DEBUG] InnerException: {e.InnerException?.Message}");
                Debug.LogError($"[DEBUG] HTTP Code at Exception: {www.responseCode}");
                Debug.LogError($"[DEBUG] Error at Exception: {www.error}");
                Debug.LogError($"[DEBUG] Result at Exception: {www.result}");

                www.Abort();
                throw e;
            }
        }
    }

    public async UniTask Post<T>(string _url, T _form, HeaderDataClass[] headers, Action<string, bool> _Callback, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            _Callback.Invoke("request cancel", false);
            return;
        }

        try
        {
            await LogIn(_url, _form, headers, (responseJson, isSuccess) =>
            {
                _Callback.Invoke(responseJson, isSuccess);
            }, token);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Operation failed but handled: {ex.Message}");
            _Callback?.Invoke("operation cancel", false);
        }
    }

    private async UniTask LogIn<T>(string _url, T _form, HeaderDataClass[] headers, Action<string, bool> _callback, CancellationToken token)
    {
        using (UnityWebRequest www = CreateRequest(_url, _form))
        {
            for (int i = 0; i < headers.Length; i++)
            {
                www.SetRequestHeader(headers[i].headerName, headers[i].headerstring);
            }

            // Await the web request and pass the cancellation token.
            // If canceled, this throws OperationCanceledException which is caught by the Post method above.
            await www.SendWebRequest().ToUniTask(cancellationToken: token);

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

    private UnityWebRequest CreateRequest<T>(string url, T formData)
    {
        if (formData is WWWForm wwwForm)
        {
            return UnityWebRequest.Post(url, wwwForm);
        }
        else
        {
            string jsonData = JsonUtility.ToJson(formData);
            UnityWebRequest www = UnityWebRequest.Post(url, jsonData, "application/json");
            //www.SetRequestHeader("Content-Type", "application/json");
            //www.SetRequestHeader("Accept", "application/json");
            return www;
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
    }
    */

    public async UniTask GetTexture(string url, Action<string, Texture2D, bool> cb, CancellationToken token = default)
    {
        var result = await GetTextureUniTask(url, token);
        cb?.Invoke(result.message, result.texture, result.success);
    }

    private readonly SemaphoreSlim _concurrencyGate = new SemaphoreSlim(8, 20);

    public async UniTask<(string message, Texture2D texture, bool success)> GetTextureUniTask(
    string url, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
            return ("Request canceled before starting.", null, false);

        bool acquired = false;
        try
        {
            await _concurrencyGate.WaitAsync(token);
            acquired = true;
            return await DownloadTextureAsync(url, token);
        }
        catch (OperationCanceledException)
        {
            return ("Canceled", null, false);
        }
        finally
        {
            if (acquired) _concurrencyGate.Release();
        }
    }

    private async UniTask<(string, Texture2D, bool)> DownloadTextureAsync(string url, CancellationToken token)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            UniTask task = www.SendWebRequest().ToUniTask(cancellationToken: token);
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                www.Abort();
                throw;
            }

            if (www.result != UnityWebRequest.Result.Success)
                return (www.error, null, false);

            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, token);

            Texture2D tex = DownloadHandlerTexture.GetContent(www);
            return ("Success", tex, true);
        }
    }
}
