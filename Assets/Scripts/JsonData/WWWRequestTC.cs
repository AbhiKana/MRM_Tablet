using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class HeaderDataClass
{
    public string headerName;
    public string headerstring;
}
public static class WWWRequestTC
{
    private static readonly SemaphoreSlim _concurrencyGate = new SemaphoreSlim(8, 20);

    public static async UniTask<(string responseJson, bool success)> Get(string _url, HeaderDataClass[] headers, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
            return ("request cancel", false);

        try
        {
            using (UnityWebRequest www = UnityWebRequest.Get(_url))
            {
                for (int i = 0; i < headers?.Length; i++)
                    www.SetRequestHeader(headers[i].headerName, headers[i].headerstring);

                await www.SendWebRequest().ToUniTask(cancellationToken: token);

                if (www.result != UnityWebRequest.Result.Success)
                    return (www.error, false);

                return (www.downloadHandler.text, true);
            }
        }
        catch (OperationCanceledException)
        {
            return ("operation cancel", false);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Operation failed but handled: {ex.Message}");
            return ("operation cancel", false);
        }
    }
    private static async UniTask LogIn(string _url, HeaderDataClass[] headers, Action<string, bool> _callback, CancellationToken token = default)
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

    public static async UniTask<(string responseJson, bool isSuccess)> Post<T>(string _url, T _form, HeaderDataClass[] headers, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
            return ("request cancel", false);

        try
        {
            using (UnityWebRequest www = CreateRequest(_url, _form))
            {
                for (int i = 0; i < headers?.Length; i++)
                    www.SetRequestHeader(headers[i].headerName, headers[i].headerstring);

                await www.SendWebRequest().ToUniTask(cancellationToken: token);

                if (www.result != UnityWebRequest.Result.Success)
                    return (www.error, false);

                return (www.downloadHandler.text, true);
            }
        }
        catch (OperationCanceledException)
        {
            return ("operation cancel", false);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Operation failed but handled: {ex.Message}");
            return ("operation cancel", false);
        }
    }

    private static async UniTask LogIn<T>(string _url, T _form, HeaderDataClass[] headers, Action<string, bool> _callback, CancellationToken token)
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

    private static UnityWebRequest CreateRequest<T>(string url, T formData)
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

    public static async UniTask GetTexture(string url, Action<string, Texture2D, bool> cb, CancellationToken token = default, bool isUrgent = false)
    {
        var result = await GetTextureUniTask(url, token, isUrgent);
        cb?.Invoke(result.message, result.texture, result.success);
    }

    public static async UniTask<(string message, Texture2D texture, bool success)> GetTextureUniTask(
    string url, CancellationToken token = default, bool isUrgent = false)
    {
        if (token.IsCancellationRequested)
            return ("Request canceled before starting.", null, false);

        bool acquired = false;
        try
        {
            if (isUrgent)
            {
                return await DownloadTextureAsync(url, token);
            }
            else
            {
                await _concurrencyGate.WaitAsync(token);
                acquired = true;
                return await DownloadTextureAsync(url, token);
            }
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

    private static async UniTask<(string, Texture2D, bool)> DownloadTextureAsync(string url, CancellationToken token)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // UniTask automatically aborts the www and throws OperationCanceledException 
            // if the token is triggered. No manual token.Register is needed!
            UniTask task = www.SendWebRequest().ToUniTask(cancellationToken: token);

            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                // Cancellation occurred, UniTask already aborted the request.
                // The 'using' block will safely Dispose() the www object.
                throw;
            }

            if (www.result != UnityWebRequest.Result.Success)
                return (www.error, null, false);

            // Optional: Yielding to ensure texture is fully readable on main thread
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, token);

            Texture2D tex = DownloadHandlerTexture.GetContent(www);
            return ("Success", tex, true);
        }
    }

    public static async UniTask<List<Texture2D>> DownloadBatchAsync(IReadOnlyList<string> urls, CancellationToken token = default)
    {
        if (urls == null || urls.Count == 0)
            return new List<Texture2D>();

        // Fire all requests at once. The SemaphoreSlim(8) inside GetTextureUniTask
        // limits actual in-flight downloads to 8; the rest queue at WaitAsync.
        var tasks = new UniTask<(string message, Texture2D texture, bool success)>[urls.Count];
        for (int i = 0; i < urls.Count; i++)
        {
            tasks[i] = GetTextureUniTask(urls[i], token);
        }

        // WhenAll preserves order: results[i] corresponds to urls[i].
        var results = await UniTask.WhenAll(tasks);

        var textures = new List<Texture2D>(results.Length);
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].success && results[i].texture != null)
                textures.Add(results[i].texture);
            else
                Debug.LogWarning($"Failed {urls[i]}: {results[i].message}");
        }
        return textures;
    }
}
