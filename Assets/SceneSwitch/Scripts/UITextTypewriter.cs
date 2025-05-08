using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UITextTypewriter : MonoBehaviour
{
	[Header("Typing Settings")]
	public TextMeshProUGUI text;
	public bool playOnStart = true;
	public bool autoRepeat = false;
	public float delayToStart;
	public float delayBetweenChars = 0.125f;
	public float delayAfterPunctuation = 0.5f;
	public string trailingChar;
	public string story;
	private float originDelayBetweenChars;
	private bool lastCharPunctuation = false;
	private char charComma;
	private char charPeriod;
	private char charEmpty;
	[Header("Audio Settings")]
	[Tooltip("When true requires AudioSource on this object.")]
	public bool useAudio = true;
	[Range(0f, 2f)]
	public float volume = .3f;
	[Tooltip("GameObject with AudioSource component.")]
	public GameObject AudioTypping;
	private AudioSource TyppingFX;

	void Awake()
	{
		if (useAudio)
		{
			TyppingFX = GetComponent<AudioSource>();
			TyppingFX.clip = AudioTypping.GetComponent<AudioSource>().clip;
		}

		text = GetComponent<TextMeshProUGUI>();
		originDelayBetweenChars = delayBetweenChars;

		charComma = Convert.ToChar(44);
		charPeriod = Convert.ToChar(46);
		charEmpty = Convert.ToChar(" ");//Convert.ToChar(255);       
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(delayToStart);
		if (playOnStart)
		{
			ChangeText(text.text, 0f);
		}
	}

	//Update text and start typewriter effect
	public void ChangeText(string textContent, float delayBetweenChars = 0)
	{
		StopCoroutine(PlayText()); //stop Coroutime if exist
		story = textContent;
		text.text = ""; //clean text
		if (autoRepeat)
			InvokeRepeating(nameof(StartTypewriter), delayBetweenChars, 2); //Invoke effect
		else
			Invoke(nameof(Start_PlayText), delayBetweenChars); //Invoke effect
	}

	public void StartTypewriter()
	{
		StopCoroutine(PlayText()); //stop Coroutime if exist
		text.text = ""; //clean text
		Invoke(nameof(Start_PlayText), delayBetweenChars); //Invoke effect
	}

	void Start_PlayText()
	{
		StartCoroutine(PlayText());
	}

	IEnumerator PlayText()
	{
		foreach (char c in story)
		{
			delayBetweenChars = originDelayBetweenChars;

			if (lastCharPunctuation)  //If previous character was a comma/period, pause typing
			{
				if (useAudio) TyppingFX.Pause();
				yield return new WaitForSeconds(delayBetweenChars = delayAfterPunctuation);
				lastCharPunctuation = false;
			}

			if (c == charEmpty || c == charComma || c == charPeriod)
			{
				if (useAudio) TyppingFX.Pause();
				lastCharPunctuation = true;
			}

			if (useAudio) TyppingFX.PlayOneShot(TyppingFX.clip, volume);
			if (text.text.Length > 0)
				text.text = text.text[..^trailingChar.Length];
			text.text += c;
			text.text += trailingChar;
			yield return new WaitForSeconds(delayBetweenChars);
		}

		if (trailingChar != "")
			text.text = text.text[..^trailingChar.Length];
	}

	private void OnDisable()
	{
		CancelInvoke();
	}
}