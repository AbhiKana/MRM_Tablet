using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadRawImage : MonoBehaviour
{
	private RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
		rawImage = GetComponent<RawImage>();

	}



	IEnumerator LoadImage(string ImageUrl)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(ImageUrl);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log(request.error);
		}
		else
		{
			yield return new WaitUntil(() => request.isDone);

			rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
			//_Displayimage.rectTransform.sizeDelta = CardImageSizeToParent(_Displayimage);

		}
		request.Dispose();
	}
}
