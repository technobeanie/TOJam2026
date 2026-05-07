using UnityEngine;

namespace Common.Rendering
{
	[ExecuteInEditMode]
	public class RatioCamera : MonoBehaviour
	{
		public float m_MinScreenGameRatio = 1.7778f;
		public float m_MaxScreenGameRatio = 1.7778f;
		public bool m_DrawGizmos;
		public Color m_GizmosBorderColor = Color.white;

		private float m_LastWidth;
		private float m_LastHeight;
		private float m_LastOrthographicSize;

		private Camera m_Camera = null;

		public float TargetRatio
		{
			get; private set;
		}

		private void Awake()
		{
			m_Camera = GetComponent<Camera>();
			ApplyRatio();
		}

		// Update is called once per frame
		protected virtual void Update()
		{
			// Make sure the camera is always widescreen.
			if (m_Camera != null && (m_LastWidth != Screen.width || m_LastHeight != Screen.height || m_LastOrthographicSize != m_Camera.orthographicSize))
			{
				ApplyRatio();
			}
		}

		public void OnDrawGizmos()
		{
			if (m_DrawGizmos && m_Camera != null && m_Camera.orthographic)
			{
				var ratio = GetRatio();
				if (ratio == 0.0f)
				{
					ratio = m_Camera.aspect;
				}

				float extentY = m_Camera.orthographicSize;

				// TODO: This is a temp. hack. To fix.
				Vector3 topLeft = Vector3.zero;
				Vector3 topRight = Vector3.zero;
				Vector3 bottomLeft = Vector3.zero;
				Vector3 bottomRight = Vector3.zero;
				if (transform.localEulerAngles.x == 90.0f)
				{
					topLeft = new Vector3(transform.position.x - (extentY * ratio), transform.position.y, transform.position.z + extentY);
					topRight = new Vector3(transform.position.x + (extentY * ratio), transform.position.y, transform.position.z + extentY);
					bottomLeft = new Vector3(transform.position.x - (extentY * ratio), transform.position.y, transform.position.z - extentY);
					bottomRight = new Vector3(transform.position.x + (extentY * ratio), transform.position.y, transform.position.z - extentY);
				}
				else
				{
					topLeft = new Vector3(transform.position.x - (extentY * ratio), transform.position.y + extentY, transform.position.z);
					topRight = new Vector3(transform.position.x + (extentY * ratio), transform.position.y + extentY, transform.position.z);
					bottomLeft = new Vector3(transform.position.x - (extentY * ratio), transform.position.y - extentY, transform.position.z);
					bottomRight = new Vector3(transform.position.x + (extentY * ratio), transform.position.y - extentY, transform.position.z);
				}

				/*topLeft = transform.TransformPoint(topLeft);
				topRight = transform.TransformPoint(topRight);
				bottomLeft = transform.TransformPoint(bottomLeft);
				bottomRight = transform.TransformPoint(bottomRight);*/

				Gizmos.color = m_GizmosBorderColor;
				//Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawLine(topLeft, topRight);
				Gizmos.DrawLine(topRight, bottomRight);
				Gizmos.DrawLine(bottomRight, bottomLeft);
				Gizmos.DrawLine(bottomLeft, topLeft);
			}
		}

		private float GetRatio()
		{
			return Mathf.Clamp((float)Screen.width / (float)Screen.height, m_MinScreenGameRatio, m_MaxScreenGameRatio);
		}

		private void ApplyRatio()
        {
			m_LastWidth = Screen.width;
			m_LastHeight = Screen.height;
			m_LastOrthographicSize = m_Camera.orthographicSize;

			float realRatio = m_LastWidth / m_LastHeight;
			float targetRatio = GetRatio();

			float targetWidth = m_LastWidth;
			float targetHeight = m_LastHeight;
			if (realRatio > targetRatio)
			{
				targetWidth = m_LastHeight * targetRatio;
				targetHeight = targetWidth / targetRatio;
			}
			else if (realRatio < targetRatio)
			{
				targetHeight = m_LastWidth / targetRatio;
				targetWidth = targetHeight * targetRatio;
			}

			float targetWidthRatio = targetWidth / m_LastWidth;
			float targetLeftRatio = (1.0f - targetWidthRatio) / 2.0f;

			float targetHeightRatio = targetHeight / m_LastHeight;
			float targetTopRatio = (1.0f - targetHeightRatio) / 2.0f;

			m_Camera.rect = new Rect(targetLeftRatio, targetTopRatio, targetWidthRatio, targetHeightRatio);
		}
	}
}
