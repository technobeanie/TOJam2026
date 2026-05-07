using System.Collections.Generic;
using UnityEngine;

namespace Common.Rendering
{
	public class CameraManager : Singleton<CameraManager>
	{
		// const

		// public

		// protected

		// private
		private Dictionary<string, Camera> _cameras = new Dictionary<string, Camera>();

		// properties

		#region Unity Methods
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			var cameras = GetComponentsInChildren<Camera>();
			for (int i = 0; i < cameras.Length; ++i)
			{
				if (!_cameras.ContainsKey(cameras[i].name))
				{
					_cameras.Add(cameras[i].name, cameras[i]);
				}
				else
				{
					Debug.LogWarning($"CameraManager: Can't have 2 cameras with the same name, '{cameras[i].name}'");
				}
			}
		}
		#endregion

		#region Public Methods
		public void Initialize()
		{
			// Nothing to do here.
		}

		public Camera GetCamera(string cameraName)
		{
			if (_cameras.ContainsKey(cameraName))
			{
				return _cameras[cameraName];
			}

			return null;
		}
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		#endregion
	}
}
