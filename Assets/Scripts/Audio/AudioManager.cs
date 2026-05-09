using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Audio
{
	public class AudioManager : Singleton<AudioManager>
	{
		// const
		private const int SFX_AUDIO_SOURCE_AMOUNT = 10;
		private const int LOOPING_SFX_AUDIO_SOURCE_AMOUNT = 10;

		// public

		// protected

		// private

		private AudioSource _musicAudioSource1 = null;
		private AudioSource _jingleAudioSource1 = null;
		private List<AudioSource> _sfxAudioSources = new List<AudioSource>();
		private List<AudioSource> _loopingSFXAudioSources = new List<AudioSource>();
		private Dictionary<string, AudioSource> _loopingSFXAudioSourceIds = new Dictionary<string, AudioSource>();

		private bool _musicIsPaused = false;
		private bool _jingleIsPlaying = false;

		// properties

		#region Unity Methods
		private void Awake()
		{
			_musicAudioSource1 = gameObject.AddComponent<AudioSource>();
			_jingleAudioSource1 = gameObject.AddComponent<AudioSource>();

			for (int i = 0; i < SFX_AUDIO_SOURCE_AMOUNT; ++i)
			{
				_sfxAudioSources.Add(gameObject.AddComponent<AudioSource>());
			}

			for (int i = 0; i < LOOPING_SFX_AUDIO_SOURCE_AMOUNT; ++i)
			{
				_loopingSFXAudioSources.Add(gameObject.AddComponent<AudioSource>());
			}

			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			if (_jingleIsPlaying && !_jingleAudioSource1.isPlaying)
			{
				StopJingle();
			}
		}
		#endregion

		#region Public Methods
		public void Initialize()
		{
			// Nothing to do here.
		}

		public void PlayMusic(AudioClip music, float volume = 1.0f)
		{
			if (music == null)
			{
				return;
			}

			if (_musicAudioSource1.clip != music)
			{
				_musicAudioSource1.time = 0.0f;
				_musicAudioSource1.clip = music;
				_musicAudioSource1.loop = true;
				_musicAudioSource1.volume = volume;
                _musicAudioSource1.panStereo = 0.0f;
                _musicAudioSource1.pitch = 1.0f;

                if (!_jingleIsPlaying)
				{
					_musicIsPaused = false;
					_musicAudioSource1.Play();
				}
			}
			else
			{
				_musicAudioSource1.loop = true;
				_musicAudioSource1.volume = volume;
                _musicAudioSource1.panStereo = 0.0f;
                _musicAudioSource1.pitch = 1.0f;
            }
		}

		public void PlayJingle(AudioClip jingle, float volume = 1.0f)
		{
			if (jingle == null)
			{
				return;
			}

			if (_musicAudioSource1.isPlaying)
			{
				_musicIsPaused = true;
				_musicAudioSource1.Pause();
			}

			_jingleIsPlaying = true;
			_jingleAudioSource1.clip = jingle;
			_jingleAudioSource1.volume = volume;
            _jingleAudioSource1.panStereo = 0.0f;
            _jingleAudioSource1.pitch = 1.0f;
            _jingleAudioSource1.Play();
		}

		public void StopJingle()
		{
			_jingleIsPlaying = false;
			_jingleAudioSource1.Stop();
			_jingleAudioSource1.clip = null;

			if (_musicIsPaused)
			{
				_musicIsPaused = false;
				if (_musicAudioSource1.clip != null)
				{
					_musicAudioSource1.UnPause();
				}
			}
			else if (_musicAudioSource1.clip != null && !_musicAudioSource1.isPlaying)
			{
				_musicAudioSource1.Play();
			}
		}

		public void StopMusic()
		{
			_musicIsPaused = false;
			_musicAudioSource1.Stop();
			_musicAudioSource1.clip = null;
		}

		public void PlaySFX(AudioClip sfx, float volume = 1.0f, bool canPlayMultiple = true, float panStereo = 0.0f, float randomPitchEffect = 0.0f)
		{
			if (sfx == null)
			{
				return;
			}

			if (!canPlayMultiple)
			{
				StopAllSFX(sfx);
			}

			AudioSource sfxSource = FindSFXAudioSource();
			if (sfxSource == null)
			{
				return;
			}

			sfxSource.clip = sfx;
			sfxSource.volume = volume;
			sfxSource.panStereo = panStereo;
			sfxSource.pitch = randomPitchEffect <= 0.0f ? 1.0f : 1.0f + Random.Range(-randomPitchEffect, randomPitchEffect);

            sfxSource.Play();

			// Last played is always on top.
			_sfxAudioSources.Remove(sfxSource);
			_sfxAudioSources.Insert(0, sfxSource);
		}

		public void PlaySFX(IList<AudioClip> sfxs, float volume = 1.0f, bool canPlayMultiple = true, float panStereo = 0.0f, float randomPitchEffect = 0.0f)
		{
			if (sfxs.Count > 0)
			{
				int index = Random.Range(0, sfxs.Count);
				AudioClip sfx = sfxs[index];

				if (!canPlayMultiple)
				{
					StopAllSFX(sfxs);
				}

				PlaySFX(sfx, volume, true, panStereo, randomPitchEffect);
			}
		}

		public void StopAllSFX()
		{
			for (int i = 0; i < _sfxAudioSources.Count; ++i)
			{
				if (_sfxAudioSources[i].isPlaying)
				{
					_sfxAudioSources[i].Stop();
					_sfxAudioSources[i].clip = null;
				}
			}
		}

		public void StopAllSFX(AudioClip sfx)
		{
			for (int i = 0; i < _sfxAudioSources.Count; ++i)
			{
				if (_sfxAudioSources[i].isPlaying && _sfxAudioSources[i].clip == sfx)
				{
					_sfxAudioSources[i].Stop();
				}
			}
		}

		public void StopAllSFX(IList<AudioClip> sfxs)
		{
			for (int i = 0; i < _sfxAudioSources.Count; ++i)
			{
				if (_sfxAudioSources[i].isPlaying && sfxs.Contains(_sfxAudioSources[i].clip))
				{
					_sfxAudioSources[i].Stop();
				}
			}
		}

		public void PlayLoopingSFX(string id, AudioClip sfx, float volume = 1.0f)
		{
			if (sfx == null)
			{
				return;
			}

			AudioSource sfxSource = null;
			if (_loopingSFXAudioSourceIds.ContainsKey(id) && _loopingSFXAudioSourceIds[id].clip != sfx)
			{
				sfxSource = _loopingSFXAudioSourceIds[id];
			}
			else
			{
				sfxSource = FindLoopingSFXAudioSource();
			}

			if (sfxSource == null)
			{
				return;
			}

			sfxSource.clip = sfx;
			sfxSource.loop = true;
			sfxSource.volume = volume;
			sfxSource.panStereo = 0.0f;
			sfxSource.pitch = 1.0f;
            sfxSource.Play();

			// Last played is always on top.
			_loopingSFXAudioSources.Remove(sfxSource);
			_loopingSFXAudioSources.Insert(0, sfxSource);

			if (_loopingSFXAudioSourceIds.ContainsKey(id))
			{
				_loopingSFXAudioSourceIds[id] = sfxSource;
			}
			else
			{
				_loopingSFXAudioSourceIds.Add(id, sfxSource);
			}
		}

		public void StopAllLoopingSFX()
		{
			for (int i = 0; i < _loopingSFXAudioSources.Count; ++i)
			{
				if (_loopingSFXAudioSources[i].isPlaying)
				{
					_loopingSFXAudioSources[i].Stop();
					_loopingSFXAudioSources[i].clip = null;
				}
			}

			_loopingSFXAudioSourceIds.Clear();
		}

		public void StopLoopingSFX(string id)
		{
			if (_loopingSFXAudioSourceIds.ContainsKey(id))
			{
				_loopingSFXAudioSourceIds[id].Stop();
				_loopingSFXAudioSourceIds[id].clip = null;

				_loopingSFXAudioSourceIds.Remove(id);
			}
		}
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		private AudioSource FindSFXAudioSource()
		{
			if (_sfxAudioSources.Count > 0)
			{
				for (int i = _sfxAudioSources.Count - 1; i >= 0; --i)
				{
					if (!_sfxAudioSources[i].isPlaying)
					{
						return _sfxAudioSources[i];
					}
				}

				return _sfxAudioSources[_sfxAudioSources.Count - 1];
			}

			return null;
		}

		private AudioSource FindLoopingSFXAudioSource()
		{
			if (_loopingSFXAudioSources.Count > 0)
			{
				for (int i = _loopingSFXAudioSources.Count - 1; i >= 0; --i)
				{
					if (!_loopingSFXAudioSources[i].isPlaying)
					{
						return _loopingSFXAudioSources[i];
					}
				}

				return _loopingSFXAudioSources[_loopingSFXAudioSources.Count - 1];
			}

			return null;
		}
		#endregion
	}
}
