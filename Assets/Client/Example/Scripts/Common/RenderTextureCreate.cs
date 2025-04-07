using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class RenderTextureCreate : MonoBehaviour
{
	public bool isUrl;
	public bool isSizedelta;
	public bool isStart;
	public VideoPlayer playerBounds;
    //public Color BgvColor;
    // Start is called before the first frame update

   // public void Start()
   // {
   //     if (isStart)
   //     {
			//CreateOnStart();
   //     }
   // }

    public void Start()
	{
		if (playerBounds != null)
		{
			playerBounds.GetComponent<RawImage>().color = Color.clear;
            if (isUrl)
				InitVideoFiles();
			playerBounds.Prepare();
			playerBounds.prepareCompleted += PlayerBounds_prepareCompleted;
		}
	}

	private void PlayerBounds_prepareCompleted(VideoPlayer source)
	{
		RenderTexture texture1 = new RenderTexture((int)source.width, (int)source.height, 32, RenderTextureFormat.ARGB32);
		texture1.Create();
		playerBounds.GetComponent<RawImage>().texture = texture1;
        playerBounds.GetComponent<RawImage>().color = Color.white;
        if (isSizedelta)
		{
			playerBounds.GetComponent<RectTransform>().sizeDelta = new Vector2((int)source.width, (int)source.height);
			playerBounds.GetComponent<RawImage>().SetNativeSize();
		}
		playerBounds.targetTexture = texture1;
	}

	public void InitVideoFiles()
	{
		string Path = Application.streamingAssetsPath;
		string[] VideoFiles = Directory.GetFiles(Path);
		playerBounds.url = VideoFiles[0];
	}
}
