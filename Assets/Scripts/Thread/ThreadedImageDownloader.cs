using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ThreadedImageDownloader : MonoBehaviour
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static ThreadedImageDownloader _instance;
    public static ThreadedImageDownloader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("ThreadedImageDownloader").AddComponent<ThreadedImageDownloader>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    #region Method1
    public void DownloadImage(string url, Texture targetImage, Action<bool> onComplete = null, Action onAllImagesLoaded = null)
    {
        new Thread(() =>
        {
            try
            {
                byte[] imageData = DownloadWithWebClient(url);

                if (imageData == null || imageData.Length == 0) return;

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    bool success = false;
                    try
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            TextureScale.Bilinear(texture, 200, 200);
                            targetImage = texture;
                            success = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Texture creation failed: {e.Message}");
                    }

                    onComplete?.Invoke(success);
                    onAllImagesLoaded?.Invoke();
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Download failed: {e.Message}");
            }
        }).Start();
    }
    #endregion

    #region Method2
    public void DownloadAndProcessImage(string[] urls,RawImage targetImages,Action<bool> onComplete = null,Action onAllImagesLoaded = null)
    {
        if (urls == null || targetImages == null)
        {
            Debug.LogError("URLs and target images arrays must be non-null and of equal length");
            return;
        }

        int totalDownloads = urls.Length;
        int completedDownloads = 0;

        for (int i = 0; i < urls.Length; i++)
        {
            int index = i; // Capture current index for closure
            string url = urls[i];
            RawImage targetImage = targetImages;

            ThreadPool.QueueUserWorkItem(async _ =>
            {
                byte[] imageData = await _httpClient.GetByteArrayAsync(url);
                //byte[] imageData = DownloadWithWebClient(url);
                if (imageData == null)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                var creationData = new TextureCreationData
                {
                    rawData = imageData,
                    targetWidth = 200,
                    targetHeight = 200,
                    targetImage = targetImage,
                    callback = success => {
                        onComplete?.Invoke(success);
                        if (Interlocked.Increment(ref completedDownloads) == totalDownloads)
                        {
                            onAllImagesLoaded?.Invoke();
                        }
                    }
                };

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    CreateAndAssignTexture(creationData, onAllImagesLoaded);
                });
            });
        }
    }

    public void DownloadAndProcessImage_Single(string[] urls, RawImage targetImages, Action<bool> onComplete = null, Action onAllImagesLoaded = null)
    {
        if (urls == null || targetImages == null)
        {
            Debug.LogError("URLs and target images arrays must be non-null and of equal length");
            return;
        }

        int totalDownloads = urls.Length;
        int completedDownloads = 0;
        Debug.Log("totalDownloads: " + totalDownloads + " & completedDownloads: " + completedDownloads);
        for (int i = 0; i < urls.Length; i++)
        {
            int index = i; // Capture current index for closure
            string url = urls[i];
            RawImage targetImage = targetImages;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                byte[] imageData = _httpClient.GetByteArrayAsync(url).Result;
                //byte[] imageData = DownloadWithWebClient(url);
                if (imageData == null)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                var creationData = new TextureCreationData
                {
                    rawData = imageData,
                    targetWidth = 200,
                    targetHeight = 200,
                    targetImage = targetImage,
                    callback = success => {
                        onComplete?.Invoke(success);
                        if (Interlocked.Increment(ref completedDownloads) == totalDownloads)
                        {
                            onAllImagesLoaded?.Invoke();
                        }
                    }
                };

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    CreateAndAssignTexture(creationData, onAllImagesLoaded);
                });
            });
        }
        Debug.Log("totalDownloads: " + totalDownloads + " & completedDownloads: " + completedDownloads);
    }

    // Overload for single image download
    public void DownloadAndProcessImage(string url,RawImage targetImage,Action<bool> onComplete = null,Action onAllImagesLoaded = null)
    {
        DownloadAndProcessImage_Single(
            new string[] { url },
            targetImage,
            onComplete,
            onAllImagesLoaded);
    }

    private void CreateAndAssignTexture(TextureCreationData data, Action onAllImagesLoaded)
    {
        try
        {
            // 1. Create texture (must be on main thread)
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            // 2. Load image (must be on main thread)
            if (!texture.LoadImage(data.rawData))
            {
                data.callback?.Invoke(false);
                return;
            }

            // 3. Apply scaling (can be moved to background if you implement custom scaling)
            TextureScale.Bilinear(texture, data.targetWidth, data.targetHeight);

            // 4. Assign to UI
            data.targetImage.texture = texture;
            data.callback?.Invoke(true);
            onAllImagesLoaded?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Texture error: {e}");
            data.callback?.Invoke(false);
        }
    }
    private struct TextureCreationData
    {
        public byte[] rawData;
        public int targetWidth;
        public int targetHeight;
        public RawImage targetImage;
        public Action<bool> callback;
    }
    #endregion
    private byte[] DownloadWithWebClient(string url)
    {
        try
        {
            //Debug.Log($"Download thread: {(System.Threading.Thread.CurrentThread.ManagedThreadId == 1 ? "MAIN" : "BACKGROUND")} " +  $"(Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId})");
            // Using System.Net.WebClient for pure background downloading
            using (WebClient client = new WebClient())
            {
                return client.DownloadData(url);
            }
        }
        catch (WebException webEx)
        {
            Debug.LogError($"Web error: {webEx.Message}");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"Download error: {e.Message}");
            return null;
        }
    }


    /*private static readonly HttpClient _httpClient = new HttpClient();
    public  async byte[] DownloadSingleImage(string url, HttpClient _httpClient)
    {
        try
        {
            byte[] imageData = await _httpClient.GetByteArrayAsync(url);

            // 2. Process texture (background thread)
            var texture = _texturePool.Get();
            if (!await Task.Run(() => TryLoadTexture(imageData, texture)))
            {
                Debug.LogError($"Failed to load image: {url}");
                return;
            }

            // 3. Only final assignment on main thread
            _mainThreadActions.Enqueue(() =>
            {
                target.texture = texture;
                onComplete?.Invoke();
            });

            //return imageData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Download failed: {ex.Message}");
            return null;
        }
    }*/
}