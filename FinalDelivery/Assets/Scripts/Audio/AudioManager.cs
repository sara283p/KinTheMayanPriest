using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

// Code taken by Brackeys.
public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	// public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	private bool _isBackgroundPlaying;
	private float _deltaFade;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			// s.source.outputAudioMixerGroup = mixerGroup;
		}
		
		_isBackgroundPlaying = true;
		_deltaFade = 0.01f;
	}

	private void Start()
	{
		if(!_isBackgroundPlaying)
		{
			Play();
		}
	}

	public void Play()
	{
		if (!_isBackgroundPlaying)
		{
			StopAllCoroutines();
			StartCoroutine(FadeOut(GetSound("MenuMusic")));
			StartCoroutine(FadeIn(GetSound("BackgroundMusic")));
			
			//StopPlaying("MenuMusic");
			//Play("BackgroundMusic");
		}

		_isBackgroundPlaying = true;
	}

	public void Stop()
	{
		if (_isBackgroundPlaying)
		{
			StopAllCoroutines();
			StartCoroutine(FadeOut(GetSound("BackgroundMusic")));
			StartCoroutine(FadeIn(GetSound("MenuMusic")));
			
			//StopPlaying("BackgroundMusic");
			//Play("MenuMusic");
		}

		_isBackgroundPlaying = false;
	}

	private IEnumerator FadeOut(Sound sound)
	{
		while (sound.source.volume > 0)
		{
			sound.source.volume -= _deltaFade;
			yield return null;
		}

		sound.source.volume = 0;
		sound.source.Stop();
	}

	private IEnumerator FadeIn(Sound sound)
	{
		sound.source.volume = 0;
		sound.source.Play();

		while (sound.source.volume < sound.volume)
		{
			sound.source.volume += _deltaFade;
			yield return null;
		}
	}

	public void Play(string sound)
	{
		Sound s = GetSound(sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}
	
	public void StopPlaying (string sound)
	{
		Sound s = GetSound(sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		
		s.source.Stop ();
	}

	private void SetVolume(String sound, float volume)
	{
		Sound s = GetSound(sound);
		if (s == null)
		{
			Debug.LogWarning("Sound " + name + "not found!");
			return;
		}

		s.volume = volume;
	}

	private Sound GetSound(String sound)
	{
		return Array.Find(sounds, item => item.name == sound);
	}

}
