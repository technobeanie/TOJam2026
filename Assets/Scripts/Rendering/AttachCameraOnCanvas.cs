using System.Collections.Generic;
using UnityEngine;

namespace Common.Rendering
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Canvas))]
	public class AttachCameraOnCanvas : MonoBehaviour
	{
		// const

		// public
		[Header("Setup")]
		[SerializeField] private string _cameraName = "GUICamera";

		// protected

		// private
		private Camera _camera = null;

		// properties

		#region Unity Methods
		private void Awake()
		{
            var canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                GameObject cameraObj = GameObject.Find(_cameraName);
                if (cameraObj != null)
                {
                    _camera = cameraObj.GetComponent<Camera>();
                    if (_camera != null)
                    {
                        canvas.worldCamera = _camera;
                    }
                }
            }
		}
        #endregion
    }
}
