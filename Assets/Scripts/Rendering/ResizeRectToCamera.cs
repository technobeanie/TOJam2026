using System.Collections.Generic;
using UnityEngine;

namespace Common.Rendering
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ResizeRectToCamera : MonoBehaviour
	{
		// class.
		private struct PositionAndRectData
		{
			public RectTransform OriginalTransform;
			public Vector3 OriginalPosition;
			public Rect OriginalRect;

			public Vector2 OriginalAnchoredPosition;
			public Vector2 OriginalAnchorMax;
			public Vector2 OriginalAnchorMin;
			public Vector2 OriginalSizeDelta;

			public PositionAndRectData(RectTransform originalTransform, Vector3 originalPosition, Rect originalRect)
			{
				OriginalTransform = originalTransform;
				OriginalPosition = originalPosition;
				OriginalRect = originalRect;

				OriginalAnchoredPosition = originalTransform.anchoredPosition;
				OriginalAnchorMax = originalTransform.anchorMax;
				OriginalAnchorMin = originalTransform.anchorMin;
				OriginalSizeDelta = originalTransform.sizeDelta;
			}
		}

		// const

		// public
		[Header("Setup")]
		[SerializeField] private string _cameraName = "GUICamera";
		[SerializeField] private bool _forceUpdate = false;

		[Header("Ignored In Safe Area")]
		[SerializeField] private bool _ignoreSafeArea = false;
		[SerializeField] private List<RectTransform> _ignoredInSafeArea = new List<RectTransform>();

		// protected

		// private
		private RectTransform _rectTransform = null;
		private Camera _camera = null;
		private RatioCamera _ratioCamera = null;
		private float _cameraRatio = 0.0f;

		private Rect _safeArea;
		private List<PositionAndRectData> _ignoredInSafeAreaRectData = new List<PositionAndRectData>();

		// properties
		public bool ForceUpdate
		{
			get { return _forceUpdate; }
			set { _forceUpdate = value; }
		}

		#region Unity Methods
		private void Awake()
		{
			if (_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}

			GameObject cameraObj = GameObject.Find(_cameraName);
			if (cameraObj != null)
			{
				_camera = cameraObj.GetComponent<Camera>();
				if (_camera != null)
				{
					_ratioCamera = _camera.GetComponentInChildren<RatioCamera>();
				}
			}

			if (enabled)
			{
				RefreshRect(true);
			}
		}

		private void Update()
		{
#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
			{
#else
			if (ForceUpdate)
			{
#endif
				RefreshRect();
			}
		}
		#endregion

		#region Public Methods
		public void RefreshRect(bool force = false)
		{
			if (_rectTransform != null && _camera != null)
			{
				Rect previousSafeArea = _safeArea;
				float previousCameraRatio = _cameraRatio;

				_cameraRatio = _ratioCamera != null ? _ratioCamera.TargetRatio : 0;

				// Detect safe zone (for iPhone X and other new devices).
				_safeArea = Screen.safeArea;
				if (_ignoreSafeArea)
				{
					_safeArea = new Rect(0, 0, Screen.width, Screen.height);
				}
#if UNITY_EDITOR
				else
				{
					_safeArea = GetDeviceResolutionSafeArea();
				}
#endif

				bool refreshRect = force || previousSafeArea != _safeArea || previousCameraRatio != _cameraRatio;
				bool refreshIgnoredInFadeArea = TryBuildIgnoredInSafeAreaData(refreshRect);

				if (refreshIgnoredInFadeArea || refreshRect)
				{
					ResizeRect(_safeArea);
					RefreshIgnoredInSafeArea();
				}
			}
		}
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		private void ResizeRect(Rect safeArea)
		{
			float gameHeight = _camera.orthographicSize * 2.0f;
			float gameWidth = gameHeight * _camera.aspect;

			Vector3 localPosition = Vector3.zero;
			localPosition.x = (float)gameWidth * (safeArea.center.x / (float)Screen.width);
			localPosition.y = (float)gameHeight * (safeArea.center.y / (float)Screen.height);

			Vector3 offsetPosition = Vector3.zero;
			offsetPosition.x = gameWidth * 0.5f;
			offsetPosition.y = gameHeight * 0.5f;

			if (!double.IsNaN(localPosition.x) && !double.IsNaN(localPosition.y) && !double.IsNaN(offsetPosition.x) && !double.IsNaN(offsetPosition.y))
			{
				_rectTransform.localPosition = localPosition - offsetPosition;
				_rectTransform.sizeDelta = new Vector2(Mathf.RoundToInt(gameWidth * (safeArea.width / Screen.width)), Mathf.RoundToInt(gameHeight * (safeArea.height / Screen.height)));
			}
		}

		private bool TryBuildIgnoredInSafeAreaData(bool force = false)
		{
			// Determine if we should refresh or not.
			bool refreshData = _ignoredInSafeAreaRectData.Count != _ignoredInSafeArea.Count;
			for (int i = 0; i < _ignoredInSafeArea.Count && !refreshData; ++i)
			{
				if (_ignoredInSafeArea[i] != _ignoredInSafeAreaRectData[i].OriginalTransform)
				{
					refreshData = true;
				}
			}

			if (force || refreshData)
			{
				// Clean first.
				ResizeRect(new Rect(0, 0, Screen.width, Screen.height));
				ResetIgnoredInSafeArea();

				// Then, store the data.
				_ignoredInSafeAreaRectData.Clear();
				for (int i = 0; i < _ignoredInSafeArea.Count; ++i)
				{
					if (_ignoredInSafeArea[i] != null)
					{
						_ignoredInSafeAreaRectData.Add(new PositionAndRectData(_ignoredInSafeArea[i], _ignoredInSafeArea[i].position, _ignoredInSafeArea[i].rect));
					}
				}

				return true;
			}

			return false;
		}

		private void RefreshIgnoredInSafeArea()
		{
			for (int i = 0; i < _ignoredInSafeAreaRectData.Count; ++i)
			{
				if (_ignoredInSafeAreaRectData[i].OriginalTransform != null)
				{
					_ignoredInSafeAreaRectData[i].OriginalTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _ignoredInSafeAreaRectData[i].OriginalRect.width);
					_ignoredInSafeAreaRectData[i].OriginalTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _ignoredInSafeAreaRectData[i].OriginalRect.height);
					_ignoredInSafeAreaRectData[i].OriginalTransform.position = _ignoredInSafeAreaRectData[i].OriginalPosition;
				}
			}
		}

		private void ResetIgnoredInSafeArea()
		{
			for (int i = 0; i < _ignoredInSafeAreaRectData.Count; ++i)
			{
				if (_ignoredInSafeAreaRectData[i].OriginalTransform != null)
				{
					_ignoredInSafeAreaRectData[i].OriginalTransform.anchoredPosition = _ignoredInSafeAreaRectData[i].OriginalAnchoredPosition;
					_ignoredInSafeAreaRectData[i].OriginalTransform.anchorMax = _ignoredInSafeAreaRectData[i].OriginalAnchorMax;
					_ignoredInSafeAreaRectData[i].OriginalTransform.anchorMin = _ignoredInSafeAreaRectData[i].OriginalAnchorMin;
					_ignoredInSafeAreaRectData[i].OriginalTransform.sizeDelta = _ignoredInSafeAreaRectData[i].OriginalSizeDelta;
				}
			}
		}
		#endregion

#if UNITY_EDITOR
		#region Editor Methods
		private const float IPHONE_X_LANDSCAPE_RATIO = 2.1653f;
		private static readonly Vector2 IPHONE_X_PORTRAIT_HEIGHT_UNSAFE_ZONE_PERCENTAGE = new Vector2(44.0f / 812.0f, 34.0f / 812.0f);
		private static readonly Vector2 IPHONE_X_PORTRAIT_WIDTH_UNSAFE_ZONE_PERCENTAGE = new Vector2(16.0f / 375f, 16.0f / 375.0f);

		private Rect GetDeviceResolutionSafeArea()
		{
			int leftUnsafeZone = 0;
			int rightUnsafeZone = 0;
			int topUnsafeZone = 0;
			int bottomUnsafeZone = 0;

			// iPhone X.
			if (Mathf.Abs(((float)Screen.width / Screen.height) - (1.0f / IPHONE_X_LANDSCAPE_RATIO)) < 0.01f)
			{
				leftUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_WIDTH_UNSAFE_ZONE_PERCENTAGE.x * Screen.width);
				rightUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_WIDTH_UNSAFE_ZONE_PERCENTAGE.y * Screen.width);
				topUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_HEIGHT_UNSAFE_ZONE_PERCENTAGE.x * Screen.height);
				bottomUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_HEIGHT_UNSAFE_ZONE_PERCENTAGE.y * Screen.height);
			}
			else if (Mathf.Abs(((float)Screen.width / Screen.height) - (IPHONE_X_LANDSCAPE_RATIO)) < 0.01f)
			{
				leftUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_HEIGHT_UNSAFE_ZONE_PERCENTAGE.x * Screen.width);
				rightUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_HEIGHT_UNSAFE_ZONE_PERCENTAGE.y * Screen.width);
				topUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_WIDTH_UNSAFE_ZONE_PERCENTAGE.x * Screen.height);
				bottomUnsafeZone = Mathf.RoundToInt(IPHONE_X_PORTRAIT_WIDTH_UNSAFE_ZONE_PERCENTAGE.y * Screen.height);
			}

			return new Rect(
				leftUnsafeZone,
				bottomUnsafeZone,
				Screen.width - leftUnsafeZone - rightUnsafeZone,
				Screen.height - topUnsafeZone - bottomUnsafeZone);
		}
		#endregion
#endif
	}
}
