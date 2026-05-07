using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Rendering
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class CameraResolution : MonoBehaviour
	{
		// const

		// public

		// protected

		// private
		[SerializeField] private int _minScreenWidth = 1080;
		[SerializeField] private float _minOrthographicSize = 960;
		[SerializeField] private Transform _movementAnchor = null;
		[SerializeField] private bool _isActive = true;
		[SerializeField] private bool _forceUpdate = false;

		private bool _isInitialized = false;
		private Camera _camera = null;

		private Vector2 _targetAnchorOffset = Vector2.zero;

		private int _currentMinScreenWidth = 1080;
		private float _currentMinOrthographicSize = 960;
		private Vector2 _currentAnchorOffset = Vector2.zero;

		private int _previousMinScreenWidth = 0;
		private float _previousMinOrthographicSize = 0;
		private Vector2 _previousAnchorOffset = Vector2.zero;

		private float _currentTime = 0.0f;
		private float _currentDuration = 0.0f;
		private AnimationCurve _currentAnimationCurve = null;

		// properties
		public bool IsActive
		{
			get { return _isActive; }
			set { _isActive = value; }
		}

		public bool ForceUpdate
		{
			get { return _forceUpdate; }
			set { _forceUpdate = value; }
		}

		public int InitialMinScreenWidth
		{
			get; private set;
		} = 0;

		public float InitialMinOrthographicSize
		{
			get; private set;
		} = 0;

		public int MinScreenWidth
		{
			get { return _minScreenWidth; }
		}

		#region Unity Methods
		public void Awake()
		{
			Initialize();

			InitialMinScreenWidth = _minScreenWidth;
			InitialMinOrthographicSize = _minOrthographicSize;

			_previousMinScreenWidth = _minScreenWidth;
			_previousMinOrthographicSize = _minOrthographicSize;

			RefreshResolution();
		}

		private void Update()
		{
			if (_currentTime > 0.0f)
			{
				_currentTime -= Time.deltaTime;
				if (_currentTime < 0.0f)
				{
					_currentTime = 0.0f;
				}

				float ratio = 1.0f - (_currentTime / _currentDuration);
				if (_currentAnimationCurve != null)
				{
					ratio = _currentAnimationCurve.Evaluate(ratio);
				}

				_currentMinScreenWidth = Mathf.CeilToInt(Mathf.Lerp(_previousMinScreenWidth, _minScreenWidth, ratio));
				_currentMinOrthographicSize = Mathf.Lerp(_previousMinOrthographicSize, _minOrthographicSize, ratio);
				_currentAnchorOffset = Vector3.Lerp(_previousAnchorOffset, _targetAnchorOffset, ratio);

				RefreshResolution();
			}
#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
			else
#else
			else if (ForceUpdate)
#endif
			{
				RefreshResolution();
			}
		}
		#endregion

		#region Public Methods
		public void RefreshResolution(int minScreenWidth, float minOrthographicSize, Vector2 offset, float duration, AnimationCurve animationCurve = null)
		{
			Initialize();

			_previousMinScreenWidth = _minScreenWidth;
			_previousMinOrthographicSize = _minOrthographicSize;
			_previousAnchorOffset = _movementAnchor != null ? _movementAnchor.localPosition : transform.localPosition;

			_currentMinScreenWidth = _previousMinScreenWidth;
			_currentMinOrthographicSize = _previousMinOrthographicSize;
			_currentAnchorOffset = _previousAnchorOffset;

			_minScreenWidth = minScreenWidth;
			_minOrthographicSize = minOrthographicSize;
			_targetAnchorOffset = offset;

			_currentDuration = duration;
			_currentTime = _currentDuration;
			_currentAnimationCurve = animationCurve;

			RefreshResolution();
		}

		public void RefreshResolution()
		{
			Initialize();

			if (_isActive)
			{
				if (_camera != null)
				{
					_camera.orthographicSize = Mathf.Max(_currentMinOrthographicSize, _currentMinScreenWidth / _camera.aspect * 0.5f);
				}

				if (_movementAnchor != null)
				{
					_movementAnchor.localPosition = _currentAnchorOffset;
				}
			}
		}

		public void ResetResolution()
		{
			Initialize();

			_minScreenWidth = InitialMinScreenWidth;
			_minOrthographicSize = InitialMinOrthographicSize;

			_currentMinScreenWidth = _minScreenWidth;
			_currentMinOrthographicSize = _minOrthographicSize;

			_currentTime = 0.0f;

			RefreshResolution();
		}
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		private void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;

				InitialMinScreenWidth = _minScreenWidth;
				InitialMinOrthographicSize = _minOrthographicSize;

				_currentMinScreenWidth = _minScreenWidth;
				_currentMinOrthographicSize = _minOrthographicSize;
				_currentAnchorOffset = _movementAnchor != null ? _movementAnchor.localPosition : transform.localPosition;

				_currentTime = 0.0f;

				if (_camera == null)
				{
					_camera = GetComponent<Camera>();
				}
			}
		}
		#endregion
	}
}
