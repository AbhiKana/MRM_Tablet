using System;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ThreadedImageDownloader : MonoBehaviour
{
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
                            //CheckAllImageLoaded();
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
    public void DownloadAndProcessImage(string url, RawImage targetImage, Action<bool> onComplete = null, Action onAllImagesLoaded = null)
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            // STAGE 1: Download (background thread)
            byte[] imageData = DownloadWithWebClient(url);
            if (imageData == null) return;
            bool success = false;
            // Prepare data for main thread
            var creationData = new TextureCreationData
            {
                rawData = imageData,
                targetWidth = 200,
                targetHeight = 200,
                targetImage = targetImage,
                callback = onComplete
            };
            // STAGE 3: Final creation and assignment (main thread)
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                CreateAndAssignTexture(creationData, onAllImagesLoaded);
            });
        });
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
}