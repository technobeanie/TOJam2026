using Common.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Audio
{
	public class AudioHook : MonoBehaviour
	{
		// const

		// public

		// protected

		// private
		[Header("Clip")]
		[SerializeField] private AudioClip _sfx = null;
		[SerializeField] private AudioClip _loopingSfx = null;
		[SerializeField] private List<AudioClip> _randomSfx = new List<AudioClip>();
		[SerializeField] private AudioClip _jingle = null;
		[SerializeField] private AudioClip _music = null;

		[Header("Attribute")]
		[SerializeField] [Range(0.0f, 1.0f)] private float _volume = 1.0f;
		[SerializeField] [Range(-1.0f, 1.0f)] private float _panStereo = 0.0f;
		[SerializeField] [Range(0f, 1.0f)] private float _randomPitchEffect = 0.0f;
        [SerializeField] private bool _canPlayMultipleSFX = true;

		// properties

		#region Unity Methods
		private void OnDestroy()
		{
			if (_loopingSfx != null)
			{
				AudioManager.Instance.StopLoopingSFX(_loopingSfx.name);
			}
		}
		#endregion

		#region Public Methods
		public void Play()
		{
			if (_sfx != null)
			{
				AudioManager.Instance.PlaySFX(_sfx, _volume, _canPlayMultipleSFX, _panStereo, _randomPitchEffect);
			}
			if (_randomSfx != null && _randomSfx.Count > 0)
			{
				AudioManager.Instance.PlaySFX(_randomSfx, _volume, _canPlayMultipleSFX, _panStereo, _randomPitchEffect);
			}
			if (_jingle != null)
			{
				AudioManager.Instance.PlayJingle(_jingle, _volume);
			}
			if (_music != null)
			{
				AudioManager.Instance.PlayMusic(_music, _volume);
			}	
			if (_loopingSfx != null)
			{
				AudioManager.Instance.PlayLoopingSFX(_loopingSfx.name, _loopingSfx, _volume);
			}
		}

		public void StopLoopingSfx()
		{
            if (_loopingSfx != null)
            {
                AudioManager.Instance.StopLoopingSFX(_loopingSfx.name);
            }
        }
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		#endregion
	}
}
