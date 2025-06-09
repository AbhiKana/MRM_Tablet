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


    public void DownloadImage(string url, RawImage targetImage)
    {
        new Thread(() =>
        {
            try
            {
                byte[] imageData = DownloadWithWebClient(url);

                if (imageData == null || imageData.Length == 0) return;

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    try
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            TextureScale.Bilinear(texture, 200, 200);
                            targetImage.texture = texture;
                            CheckAllImageLoaded();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Texture creation failed: {e.Message}");
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Download failed: {e.Message}");
            }
        }).Start();
    }

    private byte[] DownloadWithWebClient(string url)
    {
        try
        {
            Debug.Log($"Download thread: {(System.Threading.Thread.CurrentThread.ManagedThreadId == 1 ? "MAIN" : "BACKGROUND")} " +  $"(Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId})");

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
    private static void CheckAllImageLoaded()
    {
        var d_image = MarbleLoader.downlaodedImageCount;
        var t_image = MarbleLoader.totalImageCount;
        MarbleLoader.downlaodedImageCount++;

        if (MarbleLoader.totalImageCount == MarbleLoader.downlaodedImageCount)
        {
            GetAllMarbles getAllMarbles = FindObjectOfType<GetAllMarbles>();

            getAllMarbles.OnDataLoaded?.Invoke();

            ShowMarbleDetails instance = new ShowMarbleDetails();
            instance.OnDataLoadOnce?.Invoke();
        }
    }
}